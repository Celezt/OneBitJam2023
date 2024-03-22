using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Celezt.MeshGeneration
{
    public interface IMeshGenerator
    {
        public int VertexCount { get; }
        public int IndexCount { get; }
        public int JobLength {  get; }

        public void Execute<TStream>(int index, TStream stream) where TStream : struct, IMeshStream;
    }
}
