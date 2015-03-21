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
        public List<Prop> Props;
        public UnityObject PropParent;
        public HecticUFOGame()
        {
            S = this;
            
            UFO = new UFO();
            UFO.Parent = this;

            PropParent = new UnityObject("PropParent");
            PropParent.Parent = this;

            Props = new List<Prop>();

            var randRadius = 35f;
            for (var i = 0; i < 160; i++ )
            {
                var tree = new Prop(Assets.Prefabs.Tree1Prefab);
                tree.Parent = PropParent;
                tree.Transform.localScale *= UnityEngine.Random.Range(1, 2f);
                Props.Add(tree);
                tree.GameObject.name = "Tree " + i;
                tree.Transform.position = new Vector3(UnityEngine.Random.Range(-randRadius, randRadius), tree.Transform.localScale.y / 2f, UnityEngine.Random.Range(-randRadius, randRadius));
                TinyCoro.SpawnNext(() => Shadow.Create(tree.GameObject));
            }
        }
    }
}
