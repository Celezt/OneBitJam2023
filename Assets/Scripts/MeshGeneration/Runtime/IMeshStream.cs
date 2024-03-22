using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Celezt.MeshGeneration
{
    public interface IMeshStream
    {
        public void Setup(Mesh.MeshData meshData, int vertexCount, int indexCount);
        public void SetVertex(int index, Vertex vertex);
        public void SetTriangle(int index, int3 triangle);
    }
}
