using SharpNeat.Phenomes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySharpNEAT;
using UnnaturalSelection.Character;
using UnnaturalSelection.Player;
using UnnaturalSelection.Weapons;

namespace UnnaturalSelection.AI
{
    [RequireComponent(typeof(MovementController)), RequireComponent(typeof(HealthController))]
    public class NEATCharacterController : UnitController
    {
        [SerializeField] private MovementController movementController;
        [SerializeField] private HealthController healthController;
        [SerializeField] private PlayerController playerController;

        [SerializeField] private LayerMask playerCheckLayers;

        [SerializeField] private WeaponManager weaponManager;

        [SerializeField]
        private Transform[] raycasts = new Transform[0];
        private float[] raycastDistances = new float[9];
        // private RaycastHit playerRay;
        // private Vector3 playerVector;
        private float PlayerDistance = Mathf.Infinity;
        private bool PlayerRayHit = false;
        private float PlayerAngle = 0;
        private bool PlayerInFov = false;

        Quaternion characterTargetRot;

        float accumulatedFitness = 0;

        [SerializeField] private Vector4 worldBounds = new Vector4(-50, 50, -50, 50);

        private void Awake()
        {
            if(movementController == null)
                movementController = GetComponent<MovementController>();
            if(healthController == null)
                healthController = GetComponent<HealthController>();
            if(weaponManager == null)
                weaponManager = GetComponent<WeaponManager>();

            if (playerController == null && !GameObject.Find("Player").TryGetComponent<PlayerController>(out playerController))
            {
                throw new System.Exception("No Player Controller Found/Attached!");
            }

            characterTargetRot = transform.localRotation;

            healthController.DeathEvent += OnCharacterDeath;
            movementController.fallDeathEvent += OnCharacterDeath;

            (weaponManager.DefaultWeapon as ZombieMelee).OnDamageDealt += OnDamageDealt;
        }

        private void Start()
        {
            movementController.OnStart();
        }

        public void Update()
        {
            Vector3 direction = transform.forward;

            for (int i = 0; i < raycasts.Length; i++)
            {
                Transform t = raycasts[i];

                RaycastHit hit;
                var ray = transform.TransformDirection(t.forward);
                Physics.Raycast(t.position, ray, out hit, 100);
                Debug.DrawLine(t.position, t.position + t.forward, Color.red, 1);

                raycastDistances[i] = hit.distance;
            }

            Vector3 playerVector = playerController.transform.position - transform.position;
            PlayerDistance = playerVector.magnitude;

            RaycastHit playerRayHit;
            PlayerRayHit = Physics.Raycast(transform.position, playerVector, out playerRayHit, ~playerCheckLayers.value);

            accumulatedFitness += Time.deltaTime * (1 / Mathf.Max(1, playerVector.magnitude));
            
            float playerAngle;
            if (playerVector.z > 0)
            {
                playerAngle = Mathf.Atan(playerVector.z / playerVector.x);

            } 
            else
            {
                playerAngle = Mathf.Atan2(playerVector.z, playerVector.x);
            }
            PlayerInFov = PlayerRayHit && (
                    (playerAngle >= 30)
                    &&
                    (playerAngle <= 150)
                );

            if (PlayerInFov)
            {
                accumulatedFitness += 0.5f * Time.deltaTime;
            }

            movementController.OnUpdate();
        }

        protected override void UpdateBlackBoxInputs(ISignalArray inputSignalArray)
        {
            // Called by the base class on FixedUpdate

            // Feed inputs into the Neural Net (IBlackBox) by modifying its InputSignalArray
            // The size of the input array corresponds to NeatSupervisor.NetworkInputCount

            for (int i = 0; i < 9; i++)
            {
                inputSignalArray[i] = raycastDistances[i];
            }

            inputSignalArray[9] = PlayerDistance;
            inputSignalArray[10] = PlayerRayHit ? 1.0 : 0.0;
            inputSignalArray[12] = transform.position.x;
            inputSignalArray[13] = transform.position.y;
            inputSignalArray[14] = transform.position.z;
            inputSignalArray[15] = transform.rotation.eulerAngles.y;
            inputSignalArray[16] = PlayerAngle;
            inputSignalArray[17] = healthController.Vitality;
        }

        protected override void UseBlackBoxOutpts(ISignalArray outputSignalArray)
        {
            // Called by the base class after the inputs have been processed

            // Read the outputs and do something with them
            // The size of the array corresponds to NeatSupervisor.NetworkOutputCount

            float xAction = (float) outputSignalArray[0];
            float yAction = (float) outputSignalArray[1];
            float headingAction = (float) outputSignalArray[2];
            bool attackAction = outputSignalArray[3] < 1;
            bool jumpAction = outputSignalArray[4] < 1;
            bool sprintAction = outputSignalArray[5] < 1;

            movementController.MoveInput = new Vector2(xAction, yAction);
            movementController.ShouldJump = jumpAction;
            movementController.ShouldRun = sprintAction;

            (weaponManager.DefaultWeapon as ZombieMelee).AttackInput = attackAction;
            UpdateRotation(headingAction);
        }

        public override float GetFitness()
        {
            // Called during the evaluation phase (at the end of each trail)

            // The performance of this unit, i.e. it's fitness, is retrieved by this function.
            // Implement a meaningful fitness function here

            return accumulatedFitness;
        }

        protected override void HandleIsActiveChanged(bool newIsActive)
        {
            // Called whenever the value of IsActive has changed

            // Since NeatSupervisor.cs is making use of Object Pooling, this Unit will never get destroyed. 
            // Make sure that when IsActive gets set to false, the variables and the Transform of this Unit are reset!
            // Consider to also disable MeshRenderers until IsActive turns true again.

            if (newIsActive == true)
            {
                gameObject.SetActive(true);
                transform.position = new Vector3(Random.Range(worldBounds.x, worldBounds.y), 1, Random.Range(worldBounds.z, worldBounds.w));

                accumulatedFitness = 0f;
            }
            else
            {
                gameObject.SetActive(false);

                for (int i = 0; i < 9; i++)
                {
                    raycastDistances[i] = 0f;
                }
                PlayerDistance = Mathf.Infinity;
                PlayerRayHit = false;
                PlayerAngle = 0f;
                PlayerInFov = false;
            }
        }

        private void UpdateRotation(float input)
        {
            // Avoids the mouse looking if the game is effectively paused.
            if (Mathf.Abs(Time.timeScale) < float.Epsilon)
                return;

            characterTargetRot *= Quaternion.Euler(0f, input, 0f);

            transform.localRotation = characterTargetRot;
        }

        private void OnCharacterDeath()
        {
            Debug.Log("Zombie Died!");
            IsActive = false;
        }

        private void OnDamageDealt()
        {
            Debug.Log("Zombied Dealt Damage");

            accumulatedFitness += 50;
        }
    }
}
