using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityTools_4_6
{
    public class Texture2dAsset : Asset
    {
        public Texture2dAsset(string path)
            : base(path)
        {

        }

        public Texture2D Texture
        {
            get
            {
                return (Texture2D)GetObject(typeof(Texture2D));
            }
        }
    }
}
