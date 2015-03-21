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
        const float ThreatRange = 15f;
        public Farmer()
            : base(Assets.Prefabs.CowPrefab)
        {
            Transform.localScale *= 2f;
            TinyCoro.SpawnNext(BeFarmer);
            TinyCoro.SpawnNext(BeViolent);
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
                if (ufoFlee.sqrMagnitude < ThreatRange * ThreatRange)
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

        float nextShotAfter;

        IEnumerator BeViolent()
        {
            while (AIAlive)
            {
                yield return TinyCoro.WaitUntil(() =>
                {
                    if (Time.time <= nextShotAfter
                        || HecticUFOGame.S.UFO.Collecting.Contains(this)
                        || HecticUFOGame.S.UFO.Collected.Contains(this))
                        return false;

                    var ufoFlee = new Vector3(WorldPosition.x - HecticUFOGame.S.UFO.WorldPosition.x, 0, WorldPosition.z - HecticUFOGame.S.UFO.WorldPosition.z);
                    return (ufoFlee.sqrMagnitude < ThreatRange * ThreatRange);
                });
                //yield return TinyCoro.WaitUntil(StunWearsOff);
                //if(AIAlive)
                {
                    var random = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f))
                                    * 0.5f;
                    new Bullet(WorldPosition, HecticUFOGame.S.UFO.Mesh.transform.position + random);
                    nextShotAfter = Time.time + UnityEngine.Random.Range(2, 3);
                }
            }
        }

    }
}
