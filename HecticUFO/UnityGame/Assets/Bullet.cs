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
        const float Speed = 5f;
        float DieAt;
        public Bullet(Vector3 start, Vector3 dest)
            : base(Assets.Prefabs.BulletPrefab)
        {
            GameObject.layer = Layers.Bullets;
            WorldPosition = start;
            GameObject.GetComponent<Rigidbody>().velocity = (dest - start).normalized * Speed;
            DieAt = Time.time + 10f;

            TinyCoro.SpawnNext(() => Shadow.Create(GameObject));
            UnityUpdate += OnUpdate;
            UnityOnCollisionEnter += OnCollision;
        }
        bool used = false;
        void OnCollision(UnityObject me, Collision colision)
        {
            if (used)
                return;

            if (colision.gameObject == HecticUFOGame.S.UFO.Mesh
                && this.GameObject != null)
            {
                HecticUFOGame.S.UFO.Health--;
                used = true;
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
