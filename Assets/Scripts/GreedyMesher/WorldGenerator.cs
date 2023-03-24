using UnityEngine;

namespace GreedyMesher
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class WorldGenerator : MonoBehaviour
    {
        private void Start()
        {
            var chunk = new Chunk(new Vector3Int(0, 0, 0));
            var data = chunk.GenerateMesh();

            var filter = GetComponent<MeshFilter>();
            filter.ApplyMeshData(data);
        }
    }
}
