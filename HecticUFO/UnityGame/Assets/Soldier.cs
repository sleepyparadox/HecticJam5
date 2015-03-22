using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HecticUFO
{
    public class Soldier : Farmer
    {
        public Soldier()
            : base(UnityEngine.Random.Range(0, 100) < 50 ? Assets.Prefabs.Soldier1Prefab : Assets.Prefabs.Soldier2Prefab)
        {
            ShootCooldown = 0.5f;
            ThreatRange *= 2f;
            ShootRange *= 4f;
            AimRand = 1f;
            AimFollow = UnityEngine.Random.Range(0, 100) < 50 ? 1 : UnityEngine.Random.Range(3f, 5f);
        }
    }
}
