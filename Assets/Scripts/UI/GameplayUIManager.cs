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
using UnityEngine.SceneManagement;

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
        [SerializeField] private Button mainMenuButton = null;
        [SerializeField] private Button pauseQuitButton = null;

        private void Awake()
        {
            inputActionMap = GameplayManager.Instance.GetActionMap("UI");
            escapeAction = inputActionMap.FindAction("Escape");
            captureScreenshotAction = inputActionMap.FindAction("Screenshot");

            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            pauseQuitButton.onClick.AddListener(OnPauseQuit);
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
            pauseMenu.gameObject.SetActive(!pauseMenu.gameObject.activeSelf);

            if (pauseMenu.gameObject.activeSelf)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        void OnPauseQuit()
        {
            Application.Quit();
        }

        void OnMainMenuClicked()
        {
            SceneManager.LoadScene(0);
        }

        int numScreenshots = 0;
        void CaptureScreenShot()
        {
            ScreenCapture.CaptureScreenshot("ScreenShot" + numScreenshots.ToString() + ".png");
        }
    }
}
