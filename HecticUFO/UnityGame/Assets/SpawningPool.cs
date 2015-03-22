using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HecticUFO
{
    public class SpawningPool : UnityObject
    {
        public float Radius = 7;
        public float RadiusSqrd { get { return Radius * Radius; } }
        public SpawningPool(Vector3 mapCenter)
            : base(Assets.Prefabs.SpawningPoolPrefab)
        {
            WorldPosition = mapCenter + HecticUFOGame.S.Map.Layers.First(l => l.Brush == Brush.SpawningPool).WorldPosition;
            UnityDrawGizmos += (me) =>
            {
                Gizmos.color = Color.Lerp(Color.blue, Color.red, 0.5f);
                Gizmos.DrawWireSphere(WorldPosition, Radius);
            };
        }
    }
}
