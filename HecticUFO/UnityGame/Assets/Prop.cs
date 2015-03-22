﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityTools_4_6;

namespace HecticUFO
{
    public class Prop : UnityObject
    {
        public float LastAbductedAt = -1000;
        public int FoodValue;
        public bool NeedsShadow = true;
        public Rigidbody Rigid;
        float OrigonalDrag;
        float OrigonalAngularDrag;
        float OrigonalMass;
        public Prop(PrefabAsset prefab)
            :base (prefab)
        {
            Rigid = GameObject.GetComponent<Rigidbody>();
            OrigonalDrag = Rigid.drag;
            OrigonalAngularDrag = Rigid.angularDrag;
            OrigonalMass = Rigid.mass;

            UnityUpdate += CheckForStickySpawningPool;
            UnityOnCollisionEnter += OnAnyCollide;

            TinyCoro.SpawnNext(() => Shadow.Create(GameObject));
        }

        void CheckForStickySpawningPool(UnityObject me)
        {
            var dif = HecticUFOGame.S.SpawningPool.WorldPosition - WorldPosition;
            if ((dif.x * dif.x) + (dif.z * dif.z) < SpawningPool.RadiusSqrd)
            {
                if (GameObject.layer != Layers.PropStuck)
                    WillStick();
                if(!HecticUFOGame.S.UFO.Collecting.Contains(this)
                    && !HecticUFOGame.S.UFO.Collected.Contains(this)
                    && Rigid.velocity.sqrMagnitude < 0.2f)
                {
                    UnityUpdate = null;
                    UnityOnCollisionEnter = null;
                    HecticUFOGame.S.Props.Remove(this);
                    TinyCoro.SpawnNext(DoConsume);
                }
            }
            else
            {
                if (GameObject.layer != Layers.PropBounce)
                    WillBounce();
            }
        }

        IEnumerator DoConsume()
        {
            NeedsShadow = false;
            GameObject.Destroy(Rigid);
            GameObject.Destroy(GameObject.GetComponent<Collider>());

            var start = WorldPosition;
            var dest = start - new Vector3(0, Mathf.Max(Transform.localScale.x, Transform.localScale.y), 0);
            var duration = 1f;
            var elaped = 0f;
            int foodToGive = FoodValue;

            while(elaped < duration)
            {
                WorldPosition = Vector3.Lerp(start, dest, elaped / duration);

                if(foodToGive > 0
                    && elaped >= duration / 2f)
                {
                    HecticUFOGame.S.SpaceBaby.Food += foodToGive;
                    foodToGive = 0;
                }

                yield return null;
                elaped += Time.deltaTime ;
            }

            SetActive(false);
        }

        public void WillStick()
        {
            GameObject.layer = Layers.PropStuck;
            UnityOnCollisionEnter -= OnStickyCollide;
            UnityOnCollisionEnter += OnStickyCollide;
        }

        public void WillBounce()
        {
            GameObject.layer = Layers.PropBounce;
            UnityOnCollisionEnter -= OnStickyCollide;
        }

        void OnAnyCollide(UnityObject me, Collision col)
        {
            if(col.gameObject.layer == Layers.GroundBounce
                && Rigid != null
                && Rigid.velocity.y < -0f
                 && !StunWearsOff())
                MusicAudio.S.Play(MusicAudio.S.Thump, WorldPosition, AudioStackRule.Replace, Rigid.velocity.magnitude / 3f);
                
            //No more random shit
            //if (col.transform.gameObject.layer == Layers.GroundBounce)
            //{
            //    //Debug.Log(GameObject.name + " hit ground");
            //    if (Time.time > 20
            //        && UnityEngine.Random.Range(0, 100) <= 2)
            //    {
            //        //Debug.Log(GameObject.name + " will stick!");
            //        WillStick();
            //    }
            //}
        }

        void OnStickyCollide(UnityObject me, Collision col)
        {
            if(col.transform.gameObject.layer == Layers.GroundStuck)
            {
                //Debug.Log(GameObject.name + " hit " + col.transform.gameObject.name);
                Rigid.velocity = Vector3.zero;
                Rigid.angularVelocity = Vector3.zero;
                //Rigid.angularVelocity = Vector3.zero;
                Rigid.drag = OrigonalDrag * 100;
                Rigid.angularDrag = OrigonalAngularDrag * 100;
                Rigid.mass = OrigonalMass * 100;
                Rigid.constraints = RigidbodyConstraints.FreezeRotation;
            }
            else
            {
                //Restore drag outside the pool
                var dif = HecticUFOGame.S.SpawningPool.WorldPosition - WorldPosition;
                if ((dif.x * dif.x) + (dif.z * dif.z) > SpawningPool.RadiusSqrd)
                    RestoreDrag();
            }
        }

        public void RestoreDrag()
        {
            Rigid.freezeRotation = false;
            Rigid.angularDrag = OrigonalAngularDrag;
            Rigid.mass = OrigonalMass;
            Rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        }

        public bool StunWearsOff()
        {
            return Time.time >= LastAbductedAt + 3;
        }
    }
}