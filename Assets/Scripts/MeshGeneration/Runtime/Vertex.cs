using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Celezt.MeshGeneration
{
    public struct Vertex
    {
        public float3 Position, Normal;
        public float4 Tangent;
        public float2 TexCoord0;
    }
}
