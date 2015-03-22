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
        public float ScaleStart = 0.75f;
        public float ScaleEnd = 5f;
        float Scale = 0;

        UnityObject Cord;
        private UnityObject Baby1;
        private UnityObject Baby2;
        private UnityObject Baby3;
        private int _food = 0;
        
        private const int MaxFood = 30;
        int Baby2At = 10;
        int Baby3At = 20;

        private UnityEngine.Transform AttachAt;

        public SpaceBaby()
        {
            Scale = ScaleStart;
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
            var progress = (float)Food / MaxFood;
            var targetScale = Mathf.Lerp(ScaleStart, ScaleEnd, progress);
            Scale = Mathf.Lerp(Scale, targetScale, Time.deltaTime);

            Cord.Transform.localScale = Vector3.one * Scale;
            Baby1.Transform.localScale = Cord.Transform.localScale;
            Baby1.WorldPosition = AttachAt.position + offset;
            Baby2.Transform.localScale = Cord.Transform.localScale;
            Baby2.WorldPosition = AttachAt.position + offset;
            Baby3.Transform.localScale = Cord.Transform.localScale;
            Baby3.WorldPosition = AttachAt.position;
        }
        
        public int Food
        {
            get { return _food; }
            set
            {
                _food = value;
                if (_food > MaxFood)
                    _food = MaxFood;

                Debug.Log("Food set to " + Food);

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

                if((Food - 1) % 5 == 0)
                    MusicAudio.S.Play(MusicAudio.S.BabyGrow, WorldPosition, AudioStackRule.OneShot);
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
