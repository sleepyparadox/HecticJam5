using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityTools_4_6
{
    public class MaterialAsset : Asset
    {
        public MaterialAsset(string path)
            : base(path)
        {

        }

        public Material Material
        {
            get
            {
                return (Material)GetObject(typeof(Material));
            }
        }
    }
}
