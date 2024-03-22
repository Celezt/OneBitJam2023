using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;

namespace Celezt.MeshGeneration
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TriangleUInt16
    {
        public ushort P0, P1, P2;

        public static implicit operator TriangleUInt16(int3 triangle) => new TriangleUInt16
        {
            P0 = (ushort)triangle.x,
            P1 = (ushort)triangle.y,
            P2 = (ushort)triangle.z,
        };
    }
}
