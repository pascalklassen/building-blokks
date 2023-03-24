
using UnityEngine;

namespace GreedyMesher
{
    public static class MeshFilterExts
    {
        public static void ApplyMeshData(this MeshFilter filter, MeshData data)
        {
            var mesh = filter.mesh;
            
            mesh.Clear();
            
            mesh.vertices = data.Vertices;
            mesh.triangles = data.Triangles;
            mesh.colors32 = data.Colors;
            
            mesh.RecalculateNormals();
        }
    }
}
