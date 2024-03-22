using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

namespace Celezt.MeshGeneration
{
    public struct SquareGridMeshGenerator : IMeshGenerator
    {
        public int VertexCount => 4;

        public int IndexCount => 6;

        public int JobLength => 1;

        public void Execute<TStream>(int index, TStream stream) where TStream : struct, IMeshStream
        {
            var vertex = new Vertex { };
            vertex.Normal.z = -1f;
            vertex.Tangent.xw = float2(1f, -1f);

            stream.SetVertex(0, vertex);

            vertex.Position = right();
            vertex.TexCoord0 = float2(1f, 0f);
            stream.SetVertex(1, vertex);

            vertex.Position = up();
            vertex.TexCoord0 = float2(0f, 1f);
            stream.SetVertex(2, vertex);

            vertex.Position = float3(1f, 1f, 0f);
            vertex.TexCoord0 = 1f;
            stream.SetVertex(3, vertex);

            stream.SetTriangle(0, int3(0, 2, 1));
            stream.SetTriangle(1, int3(1, 2, 3));
        }
    }
}
