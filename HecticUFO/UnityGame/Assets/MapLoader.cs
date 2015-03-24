using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HecticUFO
{
    public class MapLoader
    {
        public static string Load()
        {
#if UNITY_STANDALONE
            return System.IO.File.ReadAllText(Application.streamingAssetsPath + "/Level1.csv");
#else
            return Resources.Load<TextAsset>("StreamingAssetsCopy/Level1").text;
#endif
        }

        public static void Save(string csv)
        {
#if UNITY_STANDALONE
            System.IO.File.WriteAllText(Application.streamingAssetsPath + "/Level1.csv", csv);
#endif
        }
    }
}
