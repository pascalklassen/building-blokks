using System;
using BuildingBlokks.MeshBuilder;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace BuildingBlokks.World
{
    public class Chunk
    {
        public event Action<Chunk, bool> OnVisibilityChanged; 

        public const int Size = 16;
        public const int Height = 64;
        public const int Visible = 5;
        public const int MaxViewDistance = Visible * Size;
        
        public static readonly Vector3Int Dimensions = new(Size, Height, Size);

        private static int _chunks;
        
        public Vector2Int Coordinate { get; }

        public ChunkData Data { get; }

        private Bounds _bounds;

        private readonly Transform _player;

        private IMeshBuilder _meshBuilder;
        
        private GameObject _object;
        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;
        private MeshCollider _meshCollider;

        public Chunk(Vector2Int coordinate, Transform player, IMeshBuilder meshBuilder, Material material, Transform parent)
        {
            Coordinate = coordinate;
            Data = new ChunkData();

            var pos = (Vector2)Coordinate * Size;
            _bounds = new Bounds(pos, Vector2.one * Size);

            _player = player;
            _meshBuilder = meshBuilder;

            _object = new GameObject($"Chunk #{++_chunks}")
            {
                transform =
                {
                    parent = parent,
                    position = new Vector3(Coordinate.x * Size, 0, Coordinate.y * Size)
                }
            };

            _meshRenderer = _object.AddComponent<MeshRenderer>();
            _meshRenderer.sharedMaterial = material;
            
            _meshFilter = _object.AddComponent<MeshFilter>();
            _meshCollider = _object.AddComponent<MeshCollider>();

            SetVisible(false);
        }

        public void UpdateMesh()
        {
            var data = _meshBuilder.Build(Data);
            var mesh = new Mesh()
            {
                vertices = data.Vertices.ToArray(),
                triangles = data.Triangles.ToArray()
            };
            mesh.RecalculateNormals();

            _meshFilter.sharedMesh = mesh;
        }

        public void Update()
        {
            var position = _player.position;
            var playerPos = new Vector2(position.x, position.z);
            var distanceToPlayer = Mathf.Sqrt(_bounds.SqrDistance(playerPos));

            var wasVisible = IsVisible();
            var visible = distanceToPlayer <= MaxViewDistance;

            if (wasVisible == visible) return;
            
            SetVisible(visible);
            OnVisibilityChanged?.Invoke(this, visible);
        }

        private bool IsVisible() => _object.activeSelf;

        private void SetVisible(bool active)
        {
            _object.SetActive(active);
        }

        public static class Index
        {
            public static bool Check(Vector3Int index) =>
                index.x >= 0 && index.x < Dimensions.x &&
                index.y >= 0 && index.y < Dimensions.y &&
                index.z >= 0 && index.z < Dimensions.z;
        }
    }
}
