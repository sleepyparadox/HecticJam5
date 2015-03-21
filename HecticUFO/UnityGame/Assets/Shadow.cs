﻿using System;
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
            var quad = new UnityObject(Assets.Prefabs.ShadowPrefab);
            var rigidbody = target.GetComponent<Rigidbody>();
            while(true)
            {
                if (target.activeSelf)
                {
                    if (!quad.GameObject.activeSelf)
                        quad.SetActive(true);

                    if (rigidbody == null || !rigidbody.IsSleeping())
                    {
                        quad.Transform.position = new Vector3(target.transform.position.x, 0.05f, target.transform.position.z);
                        quad.Transform.localScale = new Vector3(target.transform.localScale.x, 1f, target.transform.localScale.z);
                    }
                }
                else
                {
                    if (quad.GameObject.activeSelf)
                        quad.SetActive(false);
                }
                yield return null;
            }
        }
    }
}