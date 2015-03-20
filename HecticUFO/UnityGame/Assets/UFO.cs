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
        Collider GroundCollider;
        Transform CollectDest;
        public float CollectForce = 200f;
        public float ShootVelocity = 20f;
        public List<UnityObject> Collecting = new List<UnityObject>();
        public List<UnityObject> Collected = new List<UnityObject>();
        public UFO()
            : base(Assets.Prefabs.UFOPrefab)
        {
            GroundCollider = GameObject.Find("Ground").GetComponent<Collider>();
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
            if (GroundCollider.Raycast(mouseRay, out mouseHit, 1000))
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
                    if (Collecting.Contains(prop)
                        || Collected.Contains(prop))
                        continue;

                    var collider = prop.GameObject.GetComponent<Collider>();
                    var rigidbody = prop.GameObject.GetComponent<Rigidbody>();

                    RaycastHit hit;
                    if(rays.Any((r) => collider.Raycast(r, out hit, 10000)))
                    {
                        var temp = prop;
                        Debug.Log("Spawn collecting " + temp.GameObject.name);
                        Collecting.Add(temp);
                        TinyCoro.SpawnNext(() => PreformAbduct(temp));
                    }
                }
            }

            if(Input.GetMouseButtonUp(1)
                && Collected.Any())
            {
                var projectile = Collected[0];
                Collected.RemoveAt(0);
                projectile.Transform.position = CollectDest.position;
                projectile.SetActive(true);
                var rigidbody = projectile.GameObject.GetComponent<Rigidbody>();
                rigidbody.velocity = (MouseTarget - projectile.Transform.position).normalized * ShootVelocity;
            }
        }

        IEnumerator PreformAbduct(UnityObject prop)
        {
            Debug.Log("Start collecting " + prop.GameObject.name);
            var rigid = prop.GameObject.GetComponent<Rigidbody>();
            rigid.useGravity = false;
            var origonalScale = rigid.transform.localScale;
            var scale = 1f;
            var shrinkAt = 3f;
            var collectAtDist = 1f;
            while(Input.GetMouseButton(0))
            {
                var dist = CollectDest.position - prop.Transform.position;
                var distMag = dist.magnitude;
                
                //Collect
                if (distMag < collectAtDist)
                {
                    Debug.Log("Collect " + prop.GameObject.name);
                    rigid.useGravity = true;
                    rigid.transform.localScale = origonalScale;
                    Collecting.Remove(prop);
                    prop.SetActive(false);
                    Collected.Add(prop);
                    yield break;
                }
                
                rigid.AddForce(dist.normalized * CollectForce * Time.deltaTime);

                scale = Math.Min(1f, distMag / shrinkAt);
                prop.Transform.localScale = origonalScale * scale;

                yield return null;
            }

            Debug.Log("Drop " + prop.GameObject.name);

            //Dropped
            rigid.useGravity = true;

            //Pop back to origonal sice
            var popElapsed = 0f;
            var popDur = (1f - scale) / 2f;
            while(popElapsed < popDur)
            {
                var popScale = Mathf.Lerp(scale, 1, popElapsed / popDur);
                rigid.transform.localScale = origonalScale * popScale;
                yield return null;
                popElapsed += Time.deltaTime;
            }
            rigid.transform.localScale = origonalScale;
            Collecting.Remove(prop);
        }
    }
}
