using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnnaturalSelection.Player;
using UnnaturalSelection.Character;
using UnnaturalSelection.Weapons;
using UnitySharpNEAT;
using UnityEngine.InputSystem;

namespace UnnaturalSelection.UI
{
    public class GameplayUIManager : MonoBehaviour
    {
        [SerializeField] private Slider healthSlider = null;
        [SerializeField] private TextMeshProUGUI ammoText;

        [SerializeField] private TextMeshProUGUI fitnessScoreText = null;

        [SerializeField] private PlayerController playerController = null;
        [SerializeField] private HealthController healthController = null;
        [SerializeField] private WeaponManager weaponManager = null;

        [SerializeField] private NeatSupervisor neatSupervisor = null;

        [SerializeField] private InputActionMap inputActionMap = null;
        [SerializeField] private InputAction escapeAction = null;
        [SerializeField] private InputAction captureScreenshotAction = null;

        [SerializeField] private Transform pauseMenu = null;

        private void Awake()
        {
            inputActionMap = GameplayManager.Instance.GetActionMap("UI");
            escapeAction = inputActionMap.FindAction("Escape");
            captureScreenshotAction = inputActionMap.FindAction("Screenshot");
        }

        private void Update()
        {
            healthSlider.value = healthController.HealthPercent;
            ammoText.text = weaponManager.EquippedWeapons[0].CurrentRounds + " / " + weaponManager.EquippedWeapons[0].RoundsPerMagazine;
            if(neatSupervisor.EvolutionAlgorithm != null)
                fitnessScoreText.text = "Max Fitness: " + neatSupervisor.EvolutionAlgorithm.Statistics._maxFitness.ToString("0.00");

            if (escapeAction.triggered && !pauseMenu.gameObject.activeSelf)
                ShowPauseMenu();

            if(captureScreenshotAction.triggered)
                CaptureScreenShot();
        }

        void ShowPauseMenu()
        {

        }

        int numScreenshots = 0;
        void CaptureScreenShot()
        {
            ScreenCapture.CaptureScreenshot("ScreenShot" + numScreenshots.ToString() + ".png");
        }
    }
}
