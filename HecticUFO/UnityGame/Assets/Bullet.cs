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
        static public List<Bullet> Pool = new List<Bullet>();
        const float Speed = 10f;
        float DieAt;
        public Bullet()
            : base(Assets.Prefabs.BulletPrefab)
        {
            TinyCoro.SpawnNext(() => Shadow.Create(GameObject));
            GameObject.layer = Layers.Bullets;
        }

        public void Init(Vector3 start, Vector3 dest)
        {
            WorldPosition = start;
            GameObject.GetComponent<Rigidbody>().velocity = (dest - start).normalized * Speed;
            DieAt = Time.time + 5f;
            used = false;

            UnityUpdate += OnUpdate;
            UnityOnCollisionEnter += OnCollision;
         
            SetActive(true);
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

                KillBullet();
            }
        }

        void OnUpdate(UnityObject me)
        {
            if(Time.time >= DieAt)
            {
                KillBullet();
                return;
            }
        }

        void KillBullet()
        {
            UnityUpdate = null;
            UnityOnCollisionEnter = null;
            SetActive(false);
            Pool.Add(this);
        }
    }
}
