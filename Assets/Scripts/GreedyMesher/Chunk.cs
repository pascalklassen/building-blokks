
using System;
using Mono.Cecil;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace GreedyMesher
{
    public class Chunk
    {
        public static readonly Vector3Int Dimensions = new Vector3Int(32, 128, 32);
        
        public Vector3Int Position { get; }

        private Block[] _blocks;

        public Chunk(Vector3Int position)
        {
            Position = position;
            _blocks = new Block[Dimensions.x * Dimensions.y * Dimensions.z];
        }

        public Block this[Vector3Int index]
        {
            get => GetBlock(index);
            set => SetBlock(index, value);
        }

        private Block GetBlock(Vector3Int index) => 
            IsValidIndex(index) ? _blocks[FlattenIndex(index)] : new Block(0, 0, 0, 0);

        private void SetBlock(Vector3Int index, Block block)
        {
            if (!IsValidIndex(index))
            {
                throw new IndexOutOfRangeException($"Chunk does not contain index: {index}");
            }

            _blocks[FlattenIndex(index)] = block;
        }

        private bool IsBlockFaceVisible(Vector3Int position, int axis, bool backFace)
        {
            position[axis] += backFace ? -1 : 1;
            return !this[position].IsSolid();
        }

        private bool CompareStep(Vector3Int a, Vector3Int b, int direction, bool backFace)
        {
            var blockA = this[a];
            var blockB = this[b];

            return Equals(blockA, blockB) && 
                   blockB.IsSolid() && 
                   IsBlockFaceVisible(b, direction, backFace);
        }

        public MeshData GenerateMesh()
        {
            var builder = new MeshBuilder();

            bool[,] merged;

            Vector3Int startPos;
            Vector3Int currPos;
            Vector3Int quadSize;
            Vector3Int m;
            Vector3Int n;
            Vector3Int offsetPos;

            Vector3[] vertices;

            Block startBlock;

            var direction = 0;
            var workAxis1 = 0;
            var workAxis2 = 0;
            
            for (var face = 0; face < 6; face++)
            {
                var isBackFace = face > 2;
                
                direction = face % 3;
                
                workAxis1 = (direction + 1) % 3;
                workAxis2 = (direction + 2) % 3;

                startPos = new Vector3Int();
                currPos = new Vector3Int();

                for (startPos[direction] = 0; startPos[direction] < Dimensions[direction]; startPos[direction]++)
                {
                    merged = new bool[Dimensions[workAxis1], Dimensions[workAxis2]];

                    for (startPos[workAxis1] = 0;
                         startPos[workAxis1] < Dimensions[workAxis1];
                         startPos[workAxis1]++)
                    {
                        for (startPos[workAxis2] = 0;
                             startPos[workAxis2] < Dimensions[workAxis2];
                             startPos[workAxis2]++)
                        {
                            startBlock = this[startPos];

                            if (merged[startPos[workAxis1], startPos[workAxis2]] || 
                                !startBlock.IsSolid() || 
                                IsBlockFaceVisible(startPos, direction, isBackFace))
                            {
                                continue;
                            }
                            
                            quadSize = new Vector3Int();

                            for (currPos = startPos, currPos[workAxis2]++;
                                 currPos[workAxis2] < Dimensions[workAxis2] &&
                                 CompareStep(startPos, currPos, direction, isBackFace) &&
                                 !merged[currPos[workAxis1], currPos[workAxis2]];
                                 currPos[workAxis2]++)
                            {
                                quadSize[workAxis2] = currPos[workAxis2] - startPos[workAxis2];
                            }
                            
                            for (currPos = startPos, currPos[workAxis1]++; currPos[workAxis1] < Dimensions[workAxis1] && CompareStep(startPos, currPos, direction, isBackFace) && !merged[currPos[workAxis1], currPos[workAxis2]]; currPos[workAxis1]++) {
                                for (currPos[workAxis2] = startPos[workAxis2]; currPos[workAxis2] < Dimensions[workAxis2] && CompareStep(startPos, currPos, direction, isBackFace) && !merged[currPos[workAxis1], currPos[workAxis2]]; currPos[workAxis2]++) { }
                                
                                if (currPos[workAxis2] - startPos[workAxis2] < quadSize[workAxis2]) {
                                    break;
                                } else {
                                    currPos[workAxis2] = startPos[workAxis2];
                                }
                            }
                            quadSize[workAxis1] = currPos[workAxis1] - startPos[workAxis1];

                            m = new Vector3Int
                            {
                                [workAxis1] = quadSize[workAxis1]
                            };

                            n = new Vector3Int()
                            {
                                [workAxis2] = quadSize[workAxis2]
                            };

                            offsetPos = startPos;
                            offsetPos[direction] += isBackFace ? 0 : 1;

                            vertices = new Vector3[]
                            {
                                offsetPos,
                                offsetPos + m,
                                offsetPos + m + n,
                                offsetPos + n
                            };
                            
                            builder.AddQuad(vertices, startBlock.Color(), isBackFace);
                            
                            for (var f = 0; f < quadSize[workAxis1]; f++) {
                                for (var g = 0; g < quadSize[workAxis2]; g++) {
                                    merged[startPos[workAxis1] + f, startPos[workAxis2] + g] = true;
                                }
                            }
                        }
                    }
                }
            }

            return builder.ToMeshData();
        }

        private static bool IsValidIndex(Vector3Int index) =>
            index.x >= 0 && index.x < Dimensions.x &&
            index.y >= 0 && index.y < Dimensions.y &&
            index.z >= 0 && index.z < Dimensions.z;

        private static int FlattenIndex(Vector3Int index) =>
            (index.z * Dimensions.x * Dimensions.y) +
            (index.y * Dimensions.x) +
            index.x;
    }
}
