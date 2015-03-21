using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HecticUFO
{
    public class UFOBeam : UnityObject
    {
        const int NumRayPoints = 24;
        private MeshFilter Filter;
        public UFOBeam()
        {
            GameObject.layer = Layers.UFOBeam;
            Filter = GameObject.AddComponent<MeshFilter>();
            var renderer = GameObject.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = Assets.MaterialsParticles.greenpixelMaterial.Material;

            var verts = new List<Vector3>();
            for (var i = 0; i < NumRayPoints; i++)
                verts.Add(new Vector3((float)i / 5f, -10, 0));
            verts.Add(new Vector3(0, 10, 0));
            
            var uvs = new List<Vector2>();
            for (var i = 0; i < NumRayPoints; i++)
                uvs.Add(new Vector2((float)i / 5f, 1f));
            uvs.Add(new Vector2(0.5f, 0f));
            
            var tris = new List<int>();
            for (var i = 0; i < NumRayPoints - 1; i++)
            {
                tris.Add(NumRayPoints);
                tris.Add(i);
                tris.Add(i + 1);
            }

            var mesh = new Mesh();
            mesh.vertices = verts.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.triangles = tris.ToArray();
            mesh.RecalculateNormals();

            Filter.mesh = mesh;

            UnityUpdate += UpdateBeamEffect;
        }

        void UpdateBeamEffect(UnityObject u)
        {
            WorldPosition = Vector3.zero;
            var points = new List<Vector3>();
            for (var a = 0f; a <= Mathf.PI * 2f; a += Mathf.PI * 2f / (NumRayPoints - 1))
            {
                var point = HecticUFOGame.S.UFO.MouseTarget + (new Vector3(Mathf.Sin(a), 0, Mathf.Cos(a)) * UFO.CollectRadius);
                points.Add(point);
                Debug.DrawLine(point, point + Vector3.up, Color.red);
            }
            points.Add(HecticUFOGame.S.UFO.CollectDest.position);

            Filter.mesh.vertices = points.ToArray();
            Filter.mesh.RecalculateNormals();
            Filter.mesh.RecalculateBounds();
            Filter.mesh = Filter.mesh;
        }
    }
}
