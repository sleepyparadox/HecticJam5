using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityTools_4_6;

namespace HecticUFO
{
    public class Bullet : UnityObject
    {
        const float Speed = 20f;
        float DieAt;
        public Bullet(Vector3 start, Vector3 dest)
            : base(Assets.Prefabs.BulletPrefab)
        {
            GameObject.layer = Layers.Bullets;
            WorldPosition = start;
            GameObject.GetComponent<Rigidbody>().velocity = (dest - start).normalized;
            DieAt = Time.time + 10f;

            TinyCoro.SpawnNext(() => Shadow.Create(GameObject));
            UnityUpdate += OnUpdate;
            UnityOnCollisionEnter += OnCollision;
        }

        void OnCollision(UnityObject me, Collision colision)
        {
            if (colision.gameObject == HecticUFOGame.S.UFO.Mesh)
            {
                HecticUFOGame.S.UFO.Health--;
                Dispose();
            }
        }

        void OnUpdate(UnityObject me)
        {
            if(Time.time >= DieAt)
            {
                Dispose();
                return;
            }
        }
    }
}
