using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityTools_4_6;

namespace HecticUFO
{
    public class SpaceBaby : UnityObject
    {
        public float Scale = 1f;
        int Baby2At = 10;
        int Baby3At = 20;

        UnityObject Cord;
        private UnityObject Baby1;
        private UnityObject Baby2;
        private UnityObject Baby3;
        private int _food = 0;
        private UnityEngine.Transform AttachAt;

        public SpaceBaby()
        {
            Cord = new UnityObject(Assets.Prefabs.BabyCordPrefab);
            Cord.Parent = this;
            AttachAt = Cord.FindChild("AttachAt").transform;

            Baby1 = new UnityObject(Assets.Prefabs.Baby1Prefab);
            Baby1.Parent = this;
            Baby2 = new UnityObject(Assets.Prefabs.Baby2Prefab);
            Baby2.Parent = this;
            Baby2.SetActive(false);
            Baby3 = new UnityObject(Assets.Prefabs.Baby3Prefab);
            Baby3.Parent = this;
            Baby3.SetActive(false);

            UnityUpdate += UpdateBaby;
            //TinyCoro.SpawnNext(DoFattenBaby);
        }

        void UpdateBaby(UnityObject me)
        {
            var offset = new Vector3(0, 0, 0.05f);
            var targetScale = Vector3.one * (1f + (Food * 0.1f));
            Cord.Transform.localScale = Vector3.Lerp(Cord.Transform.localScale, targetScale, Time.deltaTime);
            Baby1.Transform.localScale = Cord.Transform.localScale;
            Baby1.WorldPosition = AttachAt.position + offset;
            Baby2.Transform.localScale = Cord.Transform.localScale;
            Baby2.WorldPosition = AttachAt.position + offset;
            Baby3.Transform.localScale = Cord.Transform.localScale;
            Baby3.WorldPosition = AttachAt.position + offset;
        }
        
        public int Food
        {
            get { return _food; }
            set
            {
                _food = value;
                if (Food == Baby2At)
                {
                    Baby1.SetActive(false);
                    Baby2.SetActive(true);
                }
                if (Food == Baby3At)
                {
                    Baby2.SetActive(false);
                    Baby3.SetActive(true);
                }
            }
        }

        //IEnumerator DoFattenBaby()
        //{
        //    var elapsed = 0f;
        //    while(true)
        //    {
        //        if(elapsed > 1)
        //        {
        //            Food++;
        //            elapsed -= 1f;
        //        }
        //        yield return null;
        //        elapsed += Time.deltaTime;
        //    }
        //}
    }
}
