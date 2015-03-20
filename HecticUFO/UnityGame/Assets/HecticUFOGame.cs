using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HecticUFO
{
    public class HecticUFOGame : UnityObject
    {
        public static HecticUFOGame S;
        public UFO UFO;
        public List<UnityObject> Props;
        public HecticUFOGame()
        {
            S = this;
            
            UFO = new UFO();
            UFO.Parent = this;

            Props = new List<UnityObject>();
            Props.Add(new UnityObject(GameObject.Find("Tree1")));
        }
    }
}
