using System;
using UnityEngine;

namespace BuildingBlokks
{
    public enum Direction
    {
        North,
        East,
        South,
        West,
        Up,
        Down,
    }

    public static class DirectionExts
    {
        public static Vector3 AsVector(this Direction direction) => direction switch
        {
            Direction.North => Vector3.forward,
            Direction.East => Vector3.right,
            Direction.South => Vector3.back,
            Direction.West => Vector3.left,
            Direction.Up => Vector3.up,
            Direction.Down => Vector3.down,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
}