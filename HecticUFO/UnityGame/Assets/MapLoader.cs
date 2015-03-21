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
            return System.IO.File.ReadAllText(Application.streamingAssetsPath + "/Level1.csv");
        }

        public static void Save(string csv)
        {
            System.IO.File.WriteAllText(Application.streamingAssetsPath + "/Level1.csv", csv);
        }
    }
}
