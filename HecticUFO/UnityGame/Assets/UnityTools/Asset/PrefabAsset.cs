using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityTools_4_6
{
    public class PrefabAsset : Asset
    {
        public PrefabAsset(string path)
            : base(path)
        {

        }

        public GameObject Prefab
        {
            get
            {
                return (GameObject)GetObject(typeof(GameObject));
            }
        }
    }
}
