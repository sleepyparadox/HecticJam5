using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HecticUFO.UnityGame.Assets
{
    public class UFO : UnityObject
    {
        public float Speed;
        public UFO()
        {
            UnityUpdate += HandleInput;
        }
        
        void HandleInput(UnityObject me)
        {
            WorldPosition += Vector3.right * Input.GetAxis("Horizontal");
            WorldPosition += Vector3.forward * Input.GetAxis("Vertical");
        }
    }
}
