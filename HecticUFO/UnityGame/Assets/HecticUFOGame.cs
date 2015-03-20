using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityTools_4_6;

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

            var randRadius = 35f;
            for (var i = 0; i < 160; i++ )
            {
                var tree = new UnityObject(Assets.Prefabs.Tree1Prefab);
                tree.Transform.localScale *= UnityEngine.Random.Range(1, 2f);
                Props.Add(tree);
                tree.GameObject.name = "Tree " + i;
                tree.Transform.position = new Vector3(UnityEngine.Random.Range(-randRadius, randRadius), tree.Transform.localScale.y / 2f, UnityEngine.Random.Range(-randRadius, randRadius));
                TinyCoro.SpawnNext(() => Shadow.Create(tree.GameObject));
            }
        }
    }
}
