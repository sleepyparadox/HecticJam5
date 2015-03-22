using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityTools_4_6;

namespace HecticUFO
{
    public class Building : UnityObject
    {
        public bool Destroyed;
        public bool Finished;
        public Building(PrefabAsset prefab)
            : base(prefab)
        {
            GameObject.layer = Layers.PropBounce;
        }

        public IEnumerator DoDestroy()
        {
            var elapsed = 0f;
            var duration = 3f;
            var start = WorldPosition;
            var renderer = GameObject.GetComponent<Renderer>();
            var angle = UnityEngine.Random.Range(-90f, 90f);
            while(elapsed < duration)
            {
                var nt = elapsed / duration;

                WorldPosition = Vector3.Lerp(start, start + new Vector3(0, Transform.localScale.y * -2f, 0), nt);
                Transform.localRotation = Quaternion.Slerp(Quaternion.identity, Quaternion.AngleAxis(angle, Vector3.forward), nt);
                renderer.material.color = Color.Lerp(Color.white, Color.black, nt);

                yield return null;
                elapsed += Time.deltaTime;
            }

            Destroyed = true;

            if(HecticUFOGame.S.Buildings.All(b => b.Destroyed && b.Finished))
            {
                HecticUFOGame.S.UFO.Camera.FeedText.enabled = false;
                HecticUFOGame.S.UFO.Camera.DestroyText.enabled = false;
                HecticUFOGame.S.UFO.Camera.RestartText.enabled = true;
                HecticUFOGame.S.UFO.Camera.DefeatText.enabled = false;
                HecticUFOGame.S.UFO.Camera.WinText.enabled = true;
                HecticUFOGame.S.UFO.Camera.CreditsText.enabled = true;
                HecticUFOGame.S.RestartOnSpace();
            }
        }
    }
}
