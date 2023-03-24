
using System;
using BuildingBlokks.World;

namespace BuildingBlokks.MeshBuilder
{
    public interface IMeshBuilder
    {
        public enum Type
        {
            Greedy
        }
        
        MeshData Build(ChunkData data);

        public static IMeshBuilder FromType(Type type) => type switch
        {
            Type.Greedy => new GreedyMeshBuilder(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
