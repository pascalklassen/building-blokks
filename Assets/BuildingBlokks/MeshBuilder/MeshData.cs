using System.Collections.Generic;
using UnityEngine;

namespace BuildingBlokks.MeshBuilder
{
    public class MeshData
    {
        public List<Vector3> Vertices { get; }
        public List<int> Triangles { get; }

        public MeshData()
        {
            Vertices = new List<Vector3>();
            Triangles = new List<int>();
        }
    }
}
