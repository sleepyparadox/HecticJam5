using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HecticUFO
{
    class SpaceBabyGohzilla : Prop
    {
        public SpaceBabyGohzilla(float babyScale, Vector3 babyPosition)
            : base(Assets.Prefabs.CowPrefab)
        {
            Transform.localScale *= babyScale;
            WorldPosition = babyPosition + (Vector3.up * 0.75f * babyScale);
        }
    }
}
