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
#if UNITY_STANDALONE
        public static IEnumerator Create(GameObject target)
        {
            //Renderers already cast shadows
            yield break;
        }
#else
        public static IEnumerator Create(GameObject target)
        {
            if (target == null)
                yield break;
            DisableMaterialShadowsRecursive(target.transform);

            var quad = new UnityObject(Assets.Prefabs.ShadowPrefab);
            quad.Parent = HecticUFOGame.S.ShadowParent;
            var rigidbody = target.GetComponent<Rigidbody>();
            while (target != null)
            {
                if (target.activeSelf)
                {
                    if (!quad.GameObject.activeSelf)
                        quad.SetActive(true);

                    if (rigidbody == null || !rigidbody.IsSleeping())
                    {
                        quad.Transform.position = new Vector3(target.transform.position.x, HecticUFOGame.S.Map.ShadowHeight, target.transform.position.z);
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
            quad.Dispose();
        }
        static void DisableMaterialShadowsRecursive(Transform parent)
        {
            var renderer = parent.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.receiveShadows = false;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            for (var i = 0; i < parent.childCount; ++i)
                DisableMaterialShadowsRecursive(parent.GetChild(i));
        }
#endif
    }
}
