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
        public float ThreatRange = 15f;
        public float ShootRange = 5f;
        public float ShootCooldown = 2.5f;
        public float AimRand = 0.5f;
        public float AimFollow = 1f;
        public int ShotCount = 0;
        public Farmer()
            :this(Assets.Prefabs.FarmerPrefab)
        {
        }

        public Farmer(PrefabAsset prefab)
            : base(prefab)
        {
            FarmerCoro = TinyCoro.SpawnNext(BeFarmer);
            ShootCoro = TinyCoro.SpawnNext(BeViolent);
            UnityUpdate += (me) => Transform.localRotation = Quaternion.Slerp(Transform.localRotation, Quaternion.identity, 100 * Time.deltaTime);
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

                var home = new Vector3(WorldPosition.x - SpawnPoint.x, 0, WorldPosition.z - SpawnPoint.z);
                if (home.sqrMagnitude > 20 * 20)
                    direction -= home * Mathf.Min(10, home.sqrMagnitude / 10f);

                var poolFlee = new Vector3(WorldPosition.x - HecticUFOGame.S.SpawningPool.WorldPosition.x, 0, WorldPosition.z - HecticUFOGame.S.SpawningPool.WorldPosition.z);
                if (poolFlee.sqrMagnitude < 9 * 9)
                    direction += poolFlee * 100f; //REVERSE, FARMER ATTACKS UFO

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
        public TinyCoro FarmerCoro;
        public TinyCoro ShootCoro;

        IEnumerator BeViolent()
        {
            while (AIAlive)
            {
                yield return TinyCoro.WaitUntil(() =>
                {
                    if (Time.time <= nextShotAfter
                        || !StunWearsOff()
                        || HecticUFOGame.S.UFO.Collecting.Contains(this)
                        || HecticUFOGame.S.UFO.Collected.Contains(this))
                        return false;

                    var ufoFlee = new Vector3(WorldPosition.x - HecticUFOGame.S.UFO.WorldPosition.x, 0, WorldPosition.z - HecticUFOGame.S.UFO.WorldPosition.z);
                    return (ufoFlee.sqrMagnitude < ShootRange * ShootRange);
                });
                //yield return TinyCoro.WaitUntil(StunWearsOff);
                //if(AIAlive)
                {
                    var aimRand = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0)
                                    * AimRand;
                    var aimFollow = (Vector3.right * Input.GetAxis("Horizontal")) + (Vector3.right * Input.GetAxis("Vertical") ) 
                        * HecticUFOGame.S.UFO.ModifiedSpeed
                        * AimFollow;

                    Bullet bullet;
                    if (Bullet.Pool.Count > 0)
                    {
                        Debug.Log("Used pool");
                        bullet = Bullet.Pool[0];
                        Bullet.Pool.RemoveAt(0);
                    }
                    else
                    {
                        bullet = new Bullet();
                    }
                    bullet.Init(WorldPosition, HecticUFOGame.S.UFO.Mesh.transform.position + aimRand + aimFollow);
                    ShotCount++;
                    nextShotAfter = Time.time + ShootCooldown;
                }
            }
        }

    }
}
