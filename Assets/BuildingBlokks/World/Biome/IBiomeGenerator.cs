using BuildingBlokks.Noise;
using UnityEngine;

namespace BuildingBlokks.World.Biome
{
    public interface IBiomeGenerator
    {
        void GenerateBiome(ChunkData data, World world, NoiseGenerator noise);
    }
}