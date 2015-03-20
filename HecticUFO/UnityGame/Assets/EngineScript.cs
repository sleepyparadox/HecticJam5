using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HecticUFO;
using UnityTools_4_6;

class EngineScript : MonoBehaviour
{
    void Awake()
    {
        new HecticUFOGame();
    }

    void Update()
    {
        TinyCoro.StepAllCoros();
    }
}
