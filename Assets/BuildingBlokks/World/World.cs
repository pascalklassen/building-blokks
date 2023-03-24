using System.Collections.Generic;
using BuildingBlokks.MeshBuilder;
using BuildingBlokks.Noise;
using BuildingBlokks.World.Biome;
using UnityEngine;

namespace BuildingBlokks.World
{
    public class World : MonoBehaviour
    {
        public WorldSettings settings;
        
        public Transform player;

        private NoiseGenerator _generator;

        private readonly Dictionary<Vector2Int, Chunk> _chunks = new();
        private readonly List<Chunk> _visibleChunks = new();
        
        private Dictionary<BiomeType, IBiomeGenerator> _biomes;

        private void Start()
        {
            _generator = FindObjectOfType<NoiseGenerator>();
            
            _biomes = new Dictionary<BiomeType, IBiomeGenerator>
            {
                { BiomeType.Hills, new HillsBiomeGenerator() }
            };
        }
        
        private void Update()
        {
            UpdateChunks();
        }

        private void UpdateChunks()
        {
            var updated = new HashSet<Vector2>();

            for (var i = 0; i < _visibleChunks.Count - 1; i++)
            {
                var chunk = _visibleChunks[i];
                chunk.Update();
                updated.Add(chunk.Coordinate);
            }

            var position = player.position;
            var playerPos = new Vector2(position.x, position.z);

            var currentChunkCoordX = Mathf.RoundToInt(playerPos.x / Chunk.Size);
            var currentChunkCoordY = Mathf.RoundToInt(playerPos.y / Chunk.Size);

            for (var yOffset = -Chunk.Visible; yOffset <= Chunk.Visible; yOffset++)
            {
                for (var xOffset = -Chunk.Visible; xOffset <= Chunk.Visible; xOffset++)
                {
                    var viewed = new Vector2Int(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                    if (updated.Contains(viewed)) continue;

                    if (_chunks.ContainsKey(viewed))
                    {
                        _chunks[viewed].Update();
                    }
                    else
                    {
                        SpawnChunkAt(viewed);
                    }
                }
            }
        }
        
        private BlockType GetBlock(Vector3Int index)
        {
            if (!IsChunkLoaded(GetChunkCoord(index)))
            {
                return BlockType.Nothing;
            }

            var data = GetChunkAt(index).Data;
            return data[ToChunkSpace(index)];
        }
        
        private void SetBlock(Vector3Int index, BlockType value)
        {
            var data = GetChunkAt(index).Data;
            data[ToChunkSpace(index)] = value;
        }

        private void SpawnChunkAt(Vector2Int pos)
        {
            var chunk = new Chunk(pos, player, IMeshBuilder.FromType(settings.meshBuildType), settings.blockMaterial, transform);
            chunk.OnVisibilityChanged += OnChunkVisibilityChanged;
            
            _chunks.Add(pos, chunk);
            
            GetBiomeGenerator(pos).GenerateBiome(chunk.Data, this, _generator);
            chunk.UpdateMesh();
        }
        
        private void OnChunkVisibilityChanged(Chunk chunk, bool isVisible)
        {
            if (isVisible)
            {
                _visibleChunks.Add(chunk);
            }
            else
            {
                _visibleChunks.Remove(chunk);
            }
        }

        private bool IsChunkLoaded(Vector2Int coordinate) => _chunks.ContainsKey(coordinate);

        private Chunk GetChunkAt(Vector3 pos)
        {
            return _chunks[GetChunkCoord(pos)];
        }

        private IBiomeGenerator GetBiomeGenerator(Vector2Int pos) => _biomes[BiomeType.Hills];

        public static Vector2Int GetChunkCoord(Vector3 pos)
        {
            var relocated = ToChunkSpace(pos);
            return new Vector2Int((int)Mathf.Floor(relocated.x), (int)Mathf.Floor(relocated.z));
        }

        public static Vector3 ToChunkSpace(Vector3 pos) => new(pos.x / Chunk.Size, pos.y, pos.z / Chunk.Size);
        public static Vector3Int ToChunkSpace(Vector3Int pos) => new(pos.x / Chunk.Size, pos.y, pos.z / Chunk.Size);

        public static Vector3 ToWorldSpace(Vector3 pos) => new(pos.x * Chunk.Size, pos.y, pos.z * Chunk.Size);
        public static Vector3Int ToWorldSpace(Vector3Int pos) => new(pos.x * Chunk.Size, pos.y, pos.z * Chunk.Size);

        public static Vector3 ToWorldPosition(Vector2 coord, Vector2 pos)
        {
            var relocated = coord * Chunk.Size;
            return new Vector3(relocated.x + pos.x, 0, relocated.y + pos.y);
        }
        public static Vector3Int ToWorldPosition(Vector2Int coord, Vector2Int pos)
        {
            var relocated = coord * Chunk.Size;
            return new Vector3Int(relocated.x + pos.x, 0, relocated.y + pos.y);
        }
    }
}