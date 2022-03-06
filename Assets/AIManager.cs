using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySharpNEAT;

public class AIManager : MonoBehaviour
{
    [SerializeField]
    private NeatSupervisor _neatSupervisor;

    private void OnEnable()
    {
        _neatSupervisor.StartEvolution();
    }

    private void OnDisable()
    {
        _neatSupervisor.StopEvolution();
    }
}
