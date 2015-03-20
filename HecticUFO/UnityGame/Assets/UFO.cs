using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityTools_4_6;

namespace HecticUFO
{
    public class UFO : UnityObject
    {
        public Vector3 MouseTarget;
        public UFOCamera Camera;
        public float Speed = 5;
        public float CollectRadius = 2f;
        Collider GroundColllider;
        Transform CollectDest;
        public float CollectForce = 40f;
        public UFO()
            : base(Assets.Prefabs.UFOPrefab)
        {
            GroundColllider = GameObject.Find("Ground").GetComponent<Collider>();
            Camera = new UFOCamera();
            Camera.Parent = this;

            UnityUpdate += HandleInput;
            UnityDrawGizmos += (me) =>
            {
                Gizmos.color = Input.GetMouseButton(0) ? Color.red : Color.white;
                Gizmos.DrawWireSphere(MouseTarget, CollectRadius);
            };

            //Whobble
            var mesh = FindChild("Mesh");
            var meshStartPos = mesh.transform.localPosition;
            var whobbleAmount = Vector3.up * 0.25f;
            UnityUpdate += (me) => mesh.transform.localPosition = meshStartPos + (Mathf.Sin(Time.time * 10f) * whobbleAmount);

            //TODO, not whobbly dest
            CollectDest = mesh.transform; 
        }

        void HandleInput(UnityObject me)
        {
            WorldPosition += Vector3.right * Input.GetAxis("Horizontal") * Speed * Time.deltaTime;
            WorldPosition += Vector3.forward * Input.GetAxis("Vertical") * Speed * Time.deltaTime;

            var mouseRay = Camera.UnityCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit mouseHit;
            if (GroundColllider.Raycast(mouseRay, out mouseHit, 1000))
            {
                MouseTarget = mouseHit.point;
                Debug.DrawLine(MouseTarget, MouseTarget + Vector3.up, Color.red);
                Camera.WorldPosition = Vector3.Lerp(WorldPosition, mouseHit.point, 0.25f);
            }

            if(Input.GetMouseButton(0))
            {
                var rays = new List<Ray>();
                for (float x = -CollectRadius; x <= CollectRadius; x += CollectRadius / 5f)
                {
                    for (float z = -CollectRadius; z <= CollectRadius; z += CollectRadius / 5f)
                    {
                        if ((x * x) + (z * z) > (CollectRadius * CollectRadius))
                            continue;
                        var diff = new Vector3(MouseTarget.x + x, 0f, MouseTarget.z + z) - CollectDest.position;
                        var ray = new Ray(CollectDest.position, diff.normalized);
                        rays.Add(ray);
                        Debug.DrawLine(ray.origin, ray.origin + diff, Color.blue);
                    }
                }
                
                foreach(var prop in HecticUFOGame.S.Props)
                {
                    var collider = prop.GameObject.GetComponent<Collider>();
                    var rigidbody = prop.GameObject.GetComponent<Rigidbody>();

                    RaycastHit hit;
                    if(rays.Any((r) => collider.Raycast(r, out hit, 10000)))
                    {
                        rigidbody.AddForce((CollectDest.position - prop.WorldPosition) * Time.deltaTime * CollectForce);
                        rigidbody.useGravity = false;
                    }
                    else
                    {
                        rigidbody.useGravity = true;
                    }
                }
            }
            else
            {
                foreach(var prop in HecticUFOGame.S.Props)
                {
                    var rigidbody = prop.GameObject.GetComponent<Rigidbody>();
                    rigidbody.useGravity = true;
                }
            }


        }
    }
}
