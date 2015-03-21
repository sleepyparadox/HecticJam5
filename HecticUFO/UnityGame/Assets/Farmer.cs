using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityTools_4_6;

namespace HecticUFO
{
    public class Farmer : Prop
    {
        public Farmer()
            : base(Assets.Prefabs.CowPrefab)
        {
            Transform.localScale *= 2f;
            TinyCoro.SpawnNext(BeFarmer);
            UnityUpdate += (me) => Transform.localRotation = Quaternion.Slerp(Transform.localRotation, Quaternion.identity, 6 * Time.deltaTime);
        }

        bool AIAlive { get { return NeedsShadow && Rigid != null; } }

        IEnumerator BeFarmer()
        {
            while (AIAlive)
            {
                var angle = UnityEngine.Random.Range(0f, Mathf.PI * 2);
                var direction = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));

                var ufoFlee = new Vector3(WorldPosition.x - HecticUFOGame.S.UFO.WorldPosition.x, 0, WorldPosition.z - HecticUFOGame.S.UFO.WorldPosition.z);
                if (ufoFlee.sqrMagnitude < 30 * 30)
                    direction += ufoFlee * -3f; //REVERSE, FARMER ATTACKS UFO

                //Go up down more
                direction.x *= 0.5f;

                direction.Normalize();

                Rigid.AddForce(new Vector3(direction.x, 2f, direction.z) * 200f);

                yield return TinyCoro.Wait(0.1f);
                yield return TinyCoro.WaitUntil(StunWearsOff);
                if (!AIAlive)
                    yield break;

                Rigid.AddForce(Vector3.down * 800f);

                yield return TinyCoro.Wait(0.25f);
                yield return TinyCoro.WaitUntil(StunWearsOff);
            }
        }

    }
}
