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

            var randRadius = 15f;
            for (var i = 0; i < 40; i++ )
            {
                var tree = new UnityObject(Assets.Prefabs.Tree1Prefab);
                Props.Add(tree);
                tree.GameObject.name = "Tree " + i;
                tree.Transform.position = new Vector3(UnityEngine.Random.Range(-randRadius, randRadius), tree.Transform.localScale.y / 2f, UnityEngine.Random.Range(-randRadius, randRadius));
            }
        }
    }
}
