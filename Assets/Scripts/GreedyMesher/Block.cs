using UnityEngine;

namespace GreedyMesher
{
    public struct Block
    {
        public byte R { get; }
        public byte G { get; }
        public byte B { get; }
        public byte A { get; }

        public Block(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public bool IsSolid() => A == byte.MaxValue;
        public Color32 Color() => new Color32(R, G, B, A);
    }
}
