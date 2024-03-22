using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Celezt.MeshGeneration
{
    public struct SingleMeshStream : IMeshStream
    {
        [NativeDisableContainerSafetyRestriction]
        private NativeArray<Stream0> _stream0;
        [NativeDisableContainerSafetyRestriction]
        private NativeArray<int3> _triangles;

        [StructLayout(LayoutKind.Sequential)]
        private struct Stream0
        {
            public float3 Position, Normal;
            public float4 Tangent;
            public float2 TexCoord0;
        }

        public void Setup(Mesh.MeshData meshData, int vertexCount, int indexCount)
        {
            var descriptors = new NativeArray<VertexAttributeDescriptor>(
                4, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            descriptors[0] = new VertexAttributeDescriptor(
               VertexAttribute.Position, dimension: 3);
            descriptors[1] = new VertexAttributeDescriptor(
                VertexAttribute.Normal, dimension: 3);
            descriptors[2] = new VertexAttributeDescriptor(
                VertexAttribute.Tangent, dimension: 4);
            descriptors[3] = new VertexAttributeDescriptor(
                VertexAttribute.TexCoord0, dimension: 2);

            meshData.SetVertexBufferParams(vertexCount, descriptors);
            descriptors.Dispose();

            meshData.SetIndexBufferParams(indexCount, IndexFormat.UInt32);

            meshData.subMeshCount = 1;
            meshData.SetSubMesh(0, new SubMeshDescriptor(0, indexCount),
                MeshUpdateFlags.DontRecalculateBounds |
                MeshUpdateFlags.DontValidateIndices);

            _stream0 = meshData.GetVertexData<Stream0>();
            _triangles = meshData.GetIndexData<int>().Reinterpret<int3>(4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetTriangle(int index, int3 triangle) => _triangles[index] = triangle;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertex(int index, Vertex vertex) => _stream0[index] = new Stream0
        {
            Position = vertex.Position,
            Normal = vertex.Normal,
            Tangent = vertex.Tangent,
            TexCoord0 = vertex.TexCoord0,
        };
    }
}
