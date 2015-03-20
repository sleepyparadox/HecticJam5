using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityTools_4_6
{
    public class Asset
    {
        public readonly string AssetPath;
        public bool Loaded { get; private set; }
        public string Name 
        {
            get
            {
                if (!Loaded)
                    throw new Exception("Failed to get name, Asset not loaded! path: " + AssetPath);
                return _obj.name;
            }
        }

        private UnityEngine.Object _obj;

        public Asset(string path)
        {
            AssetPath = path;
        }

        public UnityEngine.Object GetObject(Type type)
        {
            if (Loaded
                && _obj.GetType() != type)
                throw new Exception(AssetPath + " is already loaded as " + _obj.GetType() + "not a " + type);

            if (!Loaded)
            {
                _obj = UnityEngine.Resources.Load(AssetPath, type);
                Loaded = true;
                //Debug.Log("Loaded " + type + " at " + AssetPath);
            }

            //Debug.Log("Returning " + (_obj != null ? _obj.ToString() : "null"));

            return _obj;
        }
    }
}
