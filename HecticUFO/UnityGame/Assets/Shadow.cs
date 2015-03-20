using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HecticUFO
{
    public class Shadow
    {
        public static IEnumerator Create(GameObject target)
        {
            var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.transform.localRotation = Quaternion.AngleAxis(90f, Vector3.right);
            quad.GetComponent<Renderer>().material.color = Color.black;
            var rigidbody = target.GetComponent<Rigidbody>();

            while(true)
            {
                if (target.activeSelf)
                {
                    if (!quad.activeSelf)
                        quad.SetActive(true);

                    if (rigidbody == null || !rigidbody.IsSleeping())
                    {
                        quad.transform.position = new Vector3(target.transform.position.x, 0.05f, target.transform.position.z);
                        quad.transform.localScale = new Vector3(target.transform.localScale.x, 1f, target.transform.localScale.z);
                    }
                }
                else
                {
                    if (quad.activeSelf)
                        quad.SetActive(false);
                }
                yield return null;
            }
        }
    }
}
