using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityTools_4_6;

namespace HecticUFO
{
    public class Prop : UnityObject
    {
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

            UnityOnCollisionEnter += OnAnyCollide;
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
            if (col.transform.gameObject.layer == Layers.GroundBounce)
            {
                Debug.Log(GameObject.name + " hit ground");
                if (UnityEngine.Random.Range(0, 100) <= 15)
                {
                    Debug.Log(GameObject.name + " will stick!");
                    WillStick();
                }
            }
        }

        void OnStickyCollide(UnityObject me, Collision col)
        {
            if(col.transform.gameObject.layer == Layers.GroundStuck)
            {
                Debug.Log(GameObject.name + " hit " + col.transform.gameObject.name);
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

    }
}