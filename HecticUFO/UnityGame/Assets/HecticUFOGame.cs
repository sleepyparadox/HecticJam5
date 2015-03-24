using System;
using System.Collections;
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
        public List<Building> Buildings;
        public UnityObject PropParent;
        public UnityObject ShadowParent;
        public Vector3 MapCenter;

        public HecticUFOGame()
        {
            TinyCoro.SpawnNext(() => Shadow.Create(GameObject));
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

            MapCenter = new Vector3(Map.MapSize / 2f, 0, Map.MapSize / 2f) * Map.CellScale;

            SpawningPool = new SpawningPool(MapCenter);

            SpaceBaby = new SpaceBaby();
            SpaceBaby.Parent = this;
            SpaceBaby.WorldPosition = MapCenter;

            UFO = new UFO();
            UFO.Parent = this;
            UFO.WorldPosition += MapCenter + new Vector3(0, 0, -25f);

            var tut = new UnityObject(Assets.Prefabs.ControlsPrefab);
            tut.WorldPosition = UFO.WorldPosition + new Vector3(0, 1f, 0f);
            var startScale = tut.Transform.localScale;
            tut.UnityUpdate += (u) => tut.Transform.localScale = startScale * (1f + (Mathf.Sin(Time.time * 10f) * 0.05f));

            ShadowParent = new UnityObject("ShadowParent");
            ShadowParent.Parent = this;

            PropParent = new UnityObject("PropParent");
            PropParent.Parent = this;

            Props = new List<Prop>();
            Buildings = new List<Building>();

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
                        var count = UnityEngine.Random.Range(1, 6);
                        for (var i = 0; i < count; ++i)
                            props.Add(new Cow { FoodValue = 1});
                    }
                    if (cell.Contains(Brush.Farmer))
                    {
                        var count = UnityEngine.Random.Range(1, 4);
                        for (var i = 0; i < count; ++i)
                            props.Add(new Farmer { FoodValue = 1 });
                    }
                    if (cell.Contains(Brush.Soldier))
                    {
                        var count = UnityEngine.Random.Range(1, 6);
                        for (var i = 0; i < count; ++i)
                            props.Add(new Soldier { FoodValue = 1 });
                    }

                    if (cell.Contains(Brush.Barracks))
                    {
                        var count = UnityEngine.Random.Range(1, 4);
                        var building = new Building(UnityEngine.Random.Range(0, 100) < 60 ? Assets.Prefabs.BarracksPrefab : Assets.Prefabs.TowerPrefab);
                        building.Parent = PropParent;
                        building.WorldPosition = new Vector3((x + 0.5f) * Map.CellScale, building.Transform.localScale.y / 2f, (z + 0.5f) * Map.CellScale);
                        Buildings.Add(building);
                    }

                    if (cell.Contains(Brush.Barns))
                    {
                        var count = UnityEngine.Random.Range(1, 4);
                        var building = new Building(Assets.Prefabs.BarnPrefab);
                        building.Parent = PropParent;
                        building.WorldPosition = new Vector3((x + 0.5f) * Map.CellScale, building.Transform.localScale.y / 2f, (z + 0.5f) * Map.CellScale);
                        building.Finished = true; //Who even cares about barns
                        Buildings.Add(building);
                    }

                    foreach (var prop in props)
                    {
                        prop.Parent = PropParent;
                        Props.Add(prop);
                        prop.WorldPosition = new Vector3((x + UnityEngine.Random.Range(0f, 1)) * Map.CellScale, prop.Transform.localScale.y / 2f, (z + UnityEngine.Random.Range(0f, 1)) * Map.CellScale);
                        prop.SpawnPoint = prop.WorldPosition;
                    }
                }
            }

            MusicAudio.S.Play(MusicAudio.S.Song, null, AudioStackRule.Repeat);
        }

        public void RestartOnSpace()
        {
            UnityUpdate += (me) =>
            {
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    Restart();
                }
            };
        }

        public void Restart()
        {
            HecticUFOGame.S.Dispose();
            foreach (var coro in TinyCoro.AllCoroutines)
                coro.Kill();
            foreach (var b in Bullet.Pool)
                b.Dispose();
            Bullet.Pool.Clear();

            Application.LoadLevel(0);
        }
    }
}
