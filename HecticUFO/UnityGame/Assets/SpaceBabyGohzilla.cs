using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityTools_4_6;

namespace HecticUFO
{
    class SpaceBabyGohzilla : UnityObject
    {
        Rigidbody Rigid;
        public SpaceBabyGohzilla(float babyScale, Vector3 babyPosition)
            : base(Assets.Prefabs.BabyRampagePrefab)
        {
            Rigid = GameObject.GetComponent<Rigidbody>();
            Transform.localScale *= babyScale;
            WorldPosition = babyPosition + (Vector3.up * 0.75f * babyScale);
            UnityUpdate += (me) => Transform.localRotation = Quaternion.Slerp(Transform.localRotation, Quaternion.identity, 6 * Time.deltaTime);

            TinyCoro.SpawnNext(BeGohzilla);
            TinyCoro.SpawnNext(() => Shadow.Create(GameObject));

            MusicAudio.S.Play(MusicAudio.S.BabyRoar, WorldPosition, AudioStackRule.OneShot, 10f);
        }
 
        IEnumerator BeGohzilla()
        {
            float jumpMultiplier = 1.5f;
            while (true)
            {
                var angle = UnityEngine.Random.Range(0f, Mathf.PI * 2);
                var direction = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));

                var ufoFlee = new Vector3(WorldPosition.x - HecticUFOGame.S.UFO.WorldPosition.x, 0, WorldPosition.z - HecticUFOGame.S.UFO.WorldPosition.z);
                direction += ufoFlee * -5f; //REVERSE, FARMER ATTACKS UFO

                //Go up down more
                direction.x *= 0.5f;

                direction.Normalize();

                Rigid.AddForce(new Vector3(direction.x, 2f, direction.z) * 200f * jumpMultiplier);

                yield return TinyCoro.Wait(0.1f);

                Rigid.AddForce(Vector3.down * 800f);

                yield return TinyCoro.Wait(0.25f * jumpMultiplier);
            }
        }
    }
}
