using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySharpNEAT;

namespace UnnaturalSelection.AI
{
    public class AIManager : MonoBehaviour
    {
        [SerializeField]
        private NeatSupervisor _neatSupervisor = null;

        IEnumerator DelayStart()
        {
            yield return new WaitForSeconds(1);
            _neatSupervisor.StartEvolution();
        }

        private async void Start()
        {
            StartCoroutine("DelayStart");
        }

        private void OnDisable()
        {
            _neatSupervisor.StopEvolution();
        }
    }
}
