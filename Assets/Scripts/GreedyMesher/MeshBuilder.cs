
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GreedyMesher
{
    public class MeshBuilder
    {
        private readonly List<Vector3> _vertices;
        private readonly List<int> _triangles;
        private readonly List<Color32> _colors;

        public MeshBuilder()
        {
            _vertices = new List<Vector3>();
            _triangles = new List<int>();
            _colors = new List<Color32>();
        }

        public void AddQuad(Vector3[] vertices, Color32 color, bool isBackFace)
        {
            if (vertices.Length != 4)
            {
                throw new ArgumentException("A quad required 4 vertices");
            }

            foreach (var vertex in vertices)
            {
                _vertices.Add(vertex);
                _colors.Add(color);
            }

            if (!isBackFace)
            {
                _triangles.Add(_vertices.Count - 4);
                _triangles.Add(_vertices.Count - 3);
                _triangles.Add(_vertices.Count - 2);
                
                _triangles.Add(_vertices.Count - 4);
                _triangles.Add(_vertices.Count - 2);
                _triangles.Add(_vertices.Count - 1);
            }
            else
            {
                _triangles.Add(_vertices.Count - 2);
                _triangles.Add(_vertices.Count - 3);
                _triangles.Add(_vertices.Count - 4);
                
                _triangles.Add(_vertices.Count - 1);
                _triangles.Add(_vertices.Count - 2);
                _triangles.Add(_vertices.Count - 4);
            }
        }

        public MeshData ToMeshData()
        {
            var data = new MeshData(
                _vertices.ToArray(),
                _triangles.ToArray(),
                _colors.ToArray()
            );
            
            _vertices.Clear();
            _triangles.Clear();
            _colors.Clear();

            return data;
        }
    }
}
