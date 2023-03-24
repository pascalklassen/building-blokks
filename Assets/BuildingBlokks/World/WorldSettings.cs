using BuildingBlokks.MeshBuilder;
using UnityEngine;

namespace BuildingBlokks.World
{
    [CreateAssetMenu(fileName = "worldSettings", menuName = "Data/WorldSettings")]
    public class WorldSettings : ScriptableObject
    {
        public Material blockMaterial;
        public IMeshBuilder.Type meshBuildType;
    }
}