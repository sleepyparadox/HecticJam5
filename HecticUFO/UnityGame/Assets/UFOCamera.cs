using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HecticUFO
{
    public class UFOCamera : UnityObject
    {
        public readonly Camera UnityCamera; 
        public UFOCamera()
            : base (Assets.Prefabs.CameraPrefab)
        {
            UnityCamera = FindChildComponent<Camera>("UnityCamera");
            UnityCamera.transform.LookAt(WorldPosition);
        }
    }
}
