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

            UnityOnCollisionEnter += OnCollisonEnter;

            MusicAudio.S.Play(MusicAudio.S.BabyRoar, WorldPosition, AudioStackRule.OneShot, 10f);
        }

        void OnCollisonEnter(UnityObject me, Collision col)
        {
            var radius = 5f;
            if(col.gameObject.layer == Layers.GroundBounce
                && Rigid.velocity.y < 0.5f)
            {
                for(var a = 0f; a < Mathf.PI * 2; a += Mathf.PI * 0.2f)
                {
                    var point = new Vector3(Mathf.Sin(a), 0, Mathf.Cos(a)) * radius;
                    point.x += WorldPosition.x;
                    point.z += WorldPosition.z;
                    Debug.DrawLine(point, point + Vector3.up, Color.red, 0.5f);
                }
                var propsToCheck = HecticUFOGame.S.Props.Where(p => 
                {
                    return !HecticUFOGame.S.UFO.Collected.Contains(p) 
                        && !HecticUFOGame.S.UFO.Collecting.Contains(p);
                }).ToList();
                foreach (var prop in propsToCheck)
                {
                    var direction = prop.WorldPosition - WorldPosition;
                    direction.y = 0;

                    if (direction.sqrMagnitude >= (radius * radius))
                        continue;

                    direction.Normalize();
                    direction.y = 1;
                    prop.Gib(direction);
                }
            }
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

                Rigid.AddForce(new Vector3(direction.x * 1.5f, 2f, direction.z * 1.5f) * 200f * jumpMultiplier);

                yield return TinyCoro.Wait(0.1f);

                Rigid.AddForce(Vector3.down * 800f);

                yield return TinyCoro.Wait(0.25f * jumpMultiplier);
            }
        }
    }
}
