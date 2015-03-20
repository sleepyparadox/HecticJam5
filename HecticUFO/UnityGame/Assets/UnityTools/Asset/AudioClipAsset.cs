using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityTools_4_6
{
    public class AudioClipAsset : Asset
    {
        public AudioClipAsset(string path)
            : base(path)
        {

        }

        public AudioClip AudioClip
        {
            get
            {
                return (AudioClip)GetObject(typeof(AudioClip));
            }
        }
    }
}
