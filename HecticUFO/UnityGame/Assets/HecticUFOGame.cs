using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityTools_4_6;

namespace HecticUFO
{
    public class HecticUFOGame : UnityObject
    {
        public static HecticUFOGame S;
        public UFO UFO;
        public Map Map;
        public SpawningPool SpawningPool;
        public SpaceBaby SpaceBaby;
        public List<Prop> Props;
        public UnityObject PropParent;
        public UnityObject ShadowParent;

        public HecticUFOGame()
        {
            S = this;

            var csv = MapLoader.Load();
            Map = new Map(csv);
            Map.Parent = this;
            for (var x = 0; x < Map.MapSize; ++x)
            {
                for (var z = 0; z < Map.MapSize; ++z)
                {
                    var cell = Map.CurrentCells[x, z];
                    var xDist = ((float)x / Map.MapSize) - 0.5f;
                    var zDist = ((float)z / Map.MapSize) - 0.5f;
                    if ((xDist * xDist) + (zDist * zDist) < (0.5f * 0.5f))
                    {
                        cell.Apply(Brush.Grass);
                    }
                }
            }
            Map.RegenerateMesh();

            var mapCenter = new Vector3(Map.MapSize / 2f, 0, Map.MapSize / 2f) * Map.CellScale;

            SpawningPool = new SpawningPool(mapCenter);

            SpaceBaby = new SpaceBaby();
            SpaceBaby.Parent = this;
            SpaceBaby.WorldPosition = mapCenter;

            UFO = new UFO();
            UFO.Parent = this;
            UFO.WorldPosition += mapCenter;

            ShadowParent = new UnityObject("ShadowParent");
            ShadowParent.Parent = this;

            PropParent = new UnityObject("PropParent");
            PropParent.Parent = this;

            Props = new List<Prop>();

            for (var x = 0; x < Map.MapSize; ++x)
            {
                for (var z = 0; z < Map.MapSize; ++z)
                {
                    var cell = Map.CurrentCells[x, z];
                    var props = new List<Prop>();

                    if(cell.Contains(Brush.Trees))
                    {
                        var count = UnityEngine.Random.Range(0, 3);
                        for (var i = 0; i < count; ++i)
                        {
                            var prop = new Prop(Assets.Prefabs.Tree1Prefab);
                            prop.Transform.localScale *= UnityEngine.Random.Range(1, 2f);
                            props.Add(prop);
                        }
                    }
                    if (cell.Contains(Brush.Cows))
                    {
                        var count = UnityEngine.Random.Range(0, 5);
                        for (var i = 0; i < count; ++i)
                            props.Add(new Cow { FoodValue = 1});
                        props.Add(new Farmer() { FoodValue = 2});
                    }

                    foreach (var prop in props)
                    {
                        prop.Parent = PropParent;
                        Props.Add(prop);
                        prop.Transform.position = new Vector3((x + UnityEngine.Random.Range(0f, 1)) * Map.CellScale, prop.Transform.localScale.y / 2f, (z + UnityEngine.Random.Range(0f, 1)) * Map.CellScale);
                    }
                }
            }
        }
    }
}
