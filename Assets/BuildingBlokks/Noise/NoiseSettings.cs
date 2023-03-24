using UnityEngine;

namespace BuildingBlokks.Noise
{
    [CreateAssetMenu(fileName = "noiseSettings", menuName = "Data/NoiseSettings")]
    public class NoiseSettings : ScriptableObject
    {
        public float zoom;
        public int octaves;
        public float persistance;
        public Vector2Int offset;
        public Vector2Int worldOffset;
        public float redistributionModifier;
        public float exponent;
    }
}