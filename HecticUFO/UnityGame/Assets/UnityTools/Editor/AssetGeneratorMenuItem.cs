using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

public class AssetGeneratorMenuItem
{
    [MenuItem("UnityTools/Generate Assets.cs")]
    static void GenerateAssetsCodeFile()
    {
        UnityTools_4_6.AssetGenerator.GenerateAssetCodeFile("Assets", "Assets/Assets.cs", "Assets");
    }
}
