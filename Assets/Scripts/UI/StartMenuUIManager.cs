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
        [SerializeField] private Button arenaButton;

        private void Awake()
        {
            startButton.onClick.AddListener(OnClickClicked);
            quitButton.onClick.AddListener(OnQuitClicked);
            arenaButton.onClick.AddListener(OnArenaClicked);
        }

        void OnClickClicked()
        {
            SceneManager.LoadScene(1);
        }

        void OnArenaClicked()
        {
            SceneManager.LoadScene(2);
        }

        void OnQuitClicked()
        {
            Application.Quit();
        }
    }
}
