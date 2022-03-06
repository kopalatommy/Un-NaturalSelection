using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UnnaturalSelection.UI
{
    public class StartMenuUIManager : MonoBehaviour
    {
        [SerializeField] private Button startButton;
        [SerializeField] private Button quitButton;

        private void Awake()
        {
            startButton.onClick.AddListener(OnClickStart);
            quitButton.onClick.AddListener(OnQuitClicked);
        }

        void OnClickStart()
        {
            SceneManager.LoadScene(1);
        }

        void OnQuitClicked()
        {
            Application.Quit();
        }
    }
}
