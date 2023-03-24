using System;
using UnityEngine;

namespace BuildingBlokks.World
{
    public class ChunkData
    {
        private readonly BlockType[] _blocks = new BlockType[Chunk.Size * Chunk.Height * Chunk.Size];

        public BlockType this[int x, int y, int z]
        {
            get => this[new Vector3Int(x, y, z)];
            set => this[new Vector3Int(x, y, z)] = value;
        }

        public BlockType this[Vector3Int index]
        {
            get => GetBlock(index);
            set => SetBlock(index, value);
        }

        private BlockType GetBlock(Vector3Int index)
        {
            return !Chunk.Index.Check(index) ? BlockType.Air : _blocks[FlattenIndex(index)];
        }

        private void SetBlock(Vector3Int index, BlockType value)
        {
            if (!Chunk.Index.Check(index))
            {
                throw new ArgumentOutOfRangeException();
            }

            _blocks[FlattenIndex(index)] = value;
        }
        
        private static int FlattenIndex(Vector3Int index) =>
            (index.z * Chunk.Dimensions.x * Chunk.Dimensions.y) +
            (index.y * Chunk.Dimensions.x) +
            index.x;
    }
}