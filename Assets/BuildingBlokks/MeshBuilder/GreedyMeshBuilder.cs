using System.Collections.Generic;
using BuildingBlokks.World;
using UnityEngine;

namespace BuildingBlokks.MeshBuilder
{
    public class GreedyMeshBuilder : IMeshBuilder
    {
        private struct FaceMask
        {
            public BlockType Block { get; }
            public int Normal { get; }

            public FaceMask(BlockType block, int normal)
            {
                Block = block;
                Normal = normal;
            }
        }
        
        public MeshData Build(ChunkData data)
        {
            var mesh = new MeshData();
            var dim = Chunk.Dimensions;
            const int dimensions = 3;
            
            // iterate over the three axis (x y z)
            for (var axis = 0; axis < dimensions; axis++)
            {
                // calculate the two remaining axis perpendicular to the main axis
                var axis1 = (axis + 1) % 3;
                var axis2 = (axis + 2) % 3;

                var mainAxisDim = dim[axis];
                var axis1Dim = dim[axis1];
                var axis2Dim = dim[axis2];

                // growth of the faces in the perpendicular axis
                var deltaAxis1 = Vector3Int.zero;
                var deltaAxis2 = Vector3Int.zero;

                // next iterating block
                var chunkItr = Vector3Int.zero;
                
                // vector that represents the axis we are currently working on
                var axisMask = Vector3Int.zero;

                // point the vector in the direction of the current axis
                axisMask[axis] = 1;

                // this mask represents the plane of the two perpendicular axis
                var mask = new FaceMask[axis1Dim * axis2Dim];

                // we iterate over each slice
                for (chunkItr[axis] = -1; chunkItr[axis] < mainAxisDim;)
                {
                    // index inside the mask
                    var n = 0;
                    
                    // iterate over the perpendicular directions
                    for (chunkItr[axis2] = 0; chunkItr[axis2] < axis2Dim; chunkItr[axis2]++)
                    {
                        for (chunkItr[axis1] = 0; chunkItr[axis1] < axis1Dim; chunkItr[axis1]++)
                        {
                            var currentBlock = data[chunkItr];
                            var compareBlock = data[chunkItr + axisMask];

                            var currentBlockOpaque = currentBlock != BlockType.Air;
                            var compareBlockOpaque = compareBlock != BlockType.Air;

                            if (currentBlockOpaque == compareBlockOpaque)
                            {
                                mask[n++] = new FaceMask(BlockType.Nothing, 0);
                            }
                            else if (currentBlockOpaque)
                            {
                                mask[n++] = new FaceMask(currentBlock, -1);
                            }
                            else
                            {
                                mask[n++] = new FaceMask(compareBlock, 1);
                            }
                        }
                    }
                    
                    // now that we filled the mask with the face values, we need to build
                    // the quads of that face data
                    chunkItr[axis]++;
                    n = 0;
                    
                    // generate mesh from mask data
                    for (var j = 0; j < axis2Dim; j++)
                    {
                        for (var i = 0; i < axis1Dim;)
                        {
                            // if the border blocks are transparent or opaque
                            var currentMask = mask[n];
                            
                            if (currentMask.Normal != 0)
                            {
                                // build up the quad position vector
                                chunkItr[axis1] = i;
                                chunkItr[axis2] = j;
                                
                                // width of quad to generate
                                int width;

                                // build up the quad as long as the two masks are identical and we dont hit
                                // the chunk border
                                for (width = 1; 
                                     i + width < axis1Dim && CompareMask(mask[n + width], currentMask); 
                                     width++)
                                {
                                }
                                
                                int height;
                                var done = false;

                                for (height = 1; j + height < axis2Dim; height++)
                                {
                                    for (var k = 0; k < width; k++)
                                    {
                                        if (CompareMask(mask[n + k + height * axis1Dim], currentMask))
                                            continue;

                                        done = true;
                                        break;
                                    }

                                    if (done)
                                        break;
                                }

                                deltaAxis1[axis1] = width;
                                deltaAxis2[axis2] = height;
                                
                                var vertices = new Vector3[]
                                {
                                    chunkItr,
                                    chunkItr + deltaAxis1,
                                    chunkItr + deltaAxis2,
                                    chunkItr + deltaAxis1 + deltaAxis2
                                };

                                // creates a quad at the 4 vertices specified
                                CreateQuad(mesh, currentMask, vertices);

                                // clean up for next iteration
                                deltaAxis1 = Vector3Int.zero;
                                deltaAxis2 = Vector3Int.zero;

                                for (var l = 0; l < height; l++)
                                {
                                    for (var k = 0; k < width; k++)
                                    {
                                        mask[n + k + l * axis1Dim] = new FaceMask(BlockType.Nothing, 0);
                                    }
                                }

                                i += width;
                                n += width;
                            }
                            else
                            {
                                i++;
                                n++;
                            }
                        }
                    }
                }
            }

            return mesh;
        }

        private static bool CompareMask(FaceMask m1, FaceMask m2)
        {
            return m1.Block == m2.Block && m1.Normal == m2.Normal;
        }

        private static void CreateQuad(MeshData data, FaceMask mask, IEnumerable<Vector3> vertices)
        {
            var count = data.Vertices.Count;
            
            foreach (var vertex in vertices)
            {
                data.Vertices.Add(vertex);
            }

            data.Triangles.Add(count);
            data.Triangles.Add(count + 2 + mask.Normal);
            data.Triangles.Add(count + 2 - mask.Normal);
            
            data.Triangles.Add(count + 3);
            data.Triangles.Add(count + 1 - mask.Normal);
            data.Triangles.Add(count + 1 + mask.Normal);

            // todo: UV0! 
        }
    }
}
