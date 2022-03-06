using System.Collections;
using UnityEngine;
using UnnaturalSelection.Character;
using UnityEngine.InputSystem;
using System;
using UnnaturalSelection.Animation;

namespace UnnaturalSelection.Weapons
{
    public class ZombieMelee : MeleeWeapon
    {
        private System.Random random = new System.Random();

        public bool AttackInput { get; set; }

        /// <summary>
        /// Method used to verify the actions the player wants to execute.
        /// </summary>
        protected override void HandleInput()
        {
            bool canAttack = fPController.State != MotionState.Running && nexAttackTime < Time.time && nextInteractTime < Time.time;

            if (canAttack)
            {
                if(AttackInput)
                {
                    AttackInput = false;
                    if(random.Next() % 2 == 0)
                    {

                        armsAnimator.LeftAttack();
                        StartCoroutine(Attack());
                    }
                    else
                    {
                        armsAnimator.RightAttack();
                        StartCoroutine(Attack());
                    }
                }
            }
        }
    }
}
