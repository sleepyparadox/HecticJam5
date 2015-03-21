using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HecticUFO
{
    public class Map : UnityObject
    {
        public Cell[,] CurrentCells;
        public const float CellScale = 10f;
        public int MapSize;
        public List<MapLayer> Layers;
        public float ShadowHeight;
        public Map(string csv)
        {
            Layers = new List<MapLayer>()
            {
                new MapLayer(Brush.Water, Assets.MaterialsGround.BlueMaterial.Material) {Editable = false},
                new MapLayer(Brush.Grass, Assets.MaterialsGround.GrassBaseMaterial.Material),
                new MapLayer(Brush.SpawningPool, null){ Editable = false },
                new MapLayer(Brush.Shadow, null){ Editable = false },
                new MapLayer(Brush.Trees, Assets.Materials.tree1Material.Material){ DebugOnly = true },
                new MapLayer(Brush.Cows, Assets.Materials.CowMaterial.Material){ DebugOnly = true },
                new MapLayer(Brush.Farm, Assets.Materials.BarnMaterial.Material){ DebugOnly = true },
            };

            float height = 0f;
            foreach(var layer in Layers.Where(l => !l.DebugOnly))
            {
                layer.Parent = this;
                layer.WorldPosition = Vector3.up * height;
                height += 0.005f;
            }
            foreach (var layer in Layers.Where(l => l.DebugOnly))
            {
                layer.Parent = this;
                layer.WorldPosition = Vector3.up * height;
                layer.SetActive(false);
            }

            ShadowHeight = Layers.First(l => l.Brush == Brush.Shadow).WorldPosition.y;

            CurrentCells = Cell.ParseCells(csv, out MapSize);
            RegenerateMesh();
        }

        public void BrushChanged(Brush currentBrush)
        {
            foreach(var layer in Layers.Where(l => l.DebugOnly))
            {
                layer.SetActive(layer.Brush == currentBrush);
            }
        }

        public void RegenerateMesh()
        {
            foreach (var layer in Layers.Where(l => l.Editable && l.Filter != null))
            {
                Debug.Log("Regenerate " + layer);
                layer.Filter.mesh = FromCells(CurrentCells, layer.Brush, MapSize);
            }

            var csv = "Map Size, " + MapSize + "\n";
            for (var z = 0; z < MapSize; ++z)
            {
                for (var x = 0; x < MapSize; ++x)
                {
                    csv += CurrentCells[x, z].ToCsvCell();
                }
                csv += "\n";
            }
            MapLoader.Save(csv);
        }

        public void Apply(Vector3 worldPos, Brush brush)
        {
            var layer = Layers.FirstOrDefault(l => l.Brush == brush);
            if (layer == null || !layer.Editable)
                return;
            var x = Mathf.FloorToInt(worldPos.x / CellScale);
            var z = Mathf.FloorToInt(worldPos.z / CellScale);

            CurrentCells[x, z].Apply(brush);
            RegenerateMesh();
        }

        public void Clear(Vector3 worldPos, Brush brush)
        {
            var layer = Layers.FirstOrDefault(l => l.Brush == brush);
            if (layer == null || !layer.Editable)
                return; 
            var x = Mathf.FloorToInt(worldPos.x / CellScale);
            var z = Mathf.FloorToInt(worldPos.z / CellScale);

            CurrentCells[x, z].Clear(brush);
            RegenerateMesh();
        }

        private static Mesh FromCells(Cell[,] cells, Brush brush, int mapSize)
        {
            var verts = new List<Vector3>();
            var uvs = new List<Vector2>();
            var tris = new List<int>();

            var currentIndex = 0;
            for (var x = 0; x < mapSize; ++x)
            {
                for (var z = 0; z < mapSize; ++z)
                {
                    if(cells[x, z].Contains(brush))
                    {
                        //Debug.Log(x + ", " + z + " contains " + brushMaterial);
                        verts.Add(new Vector3(x, 0, z));
                        verts.Add(new Vector3(x + 1, 0, z));
                        verts.Add(new Vector3(x + 1, 0, z + 1));
                        verts.Add(new Vector3(x, 0, z + 1));

                        uvs.Add(new Vector3(x, z));
                        uvs.Add(new Vector3(x + 1, z));
                        uvs.Add(new Vector3(x + 1, z + 1));
                        uvs.Add(new Vector3(x, z + 1));

                        tris.Add(currentIndex + 0);
                        tris.Add(currentIndex + 2);
                        tris.Add(currentIndex + 1);

                        tris.Add(currentIndex + 0);
                        tris.Add(currentIndex + 3);
                        tris.Add(currentIndex + 2);

                        currentIndex += 4;
                    }
                }
            }

            float uvScale = 1f;

            var mesh = new Mesh();
            mesh.vertices = verts.Select(vert => vert * CellScale).ToArray();
            mesh.uv = uvs.Select(uv => uv * uvScale).ToArray();
            mesh.triangles = tris.ToArray();

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
    }
    public class MapLayer : UnityObject
    {
        public MapLayer(Brush brush, Material sharedMaterial)
            :base(brush.ToString())
        {
            GameObject.layer = Layers.GroundMesh;
            Brush = brush;
            if (sharedMaterial == null)
                return;
            Filter = GameObject.AddComponent<MeshFilter>();
            var renderer = GameObject.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = sharedMaterial;
        }

        public Brush Brush;
        public MeshFilter Filter;
        public bool DebugOnly;
        public bool Editable = true;
    }
}
