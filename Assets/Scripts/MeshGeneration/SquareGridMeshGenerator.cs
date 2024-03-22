using Celezt.MeshGeneration;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[HideMonoScript, ExecuteAlways]
public class SquareGridMeshGenerator : MonoBehaviour
{
    [SerializeField, Min(0), OnValueChanged(nameof(GenerateMesh))]
    private int _resolution = 2;
    [SerializeField, OnValueChanged(nameof(GenerateMesh))]
    private float _padding = 0f;

    private Mesh _mesh;

    private struct Generator : IMeshGenerator
    {
        public int Resolution { get; set; }
        public float Padding { get; set; }

        public int VertexCount => 4 * Resolution * Resolution;
        public int IndexCount => 6 * Resolution * Resolution;
        public int JobLength => Resolution; // Job for each row.
        public Bounds Bounds => new Bounds(
            new Vector3(Resolution / 2f, 0, Resolution / 2f),
            new Vector3(Resolution, 0f, Resolution));

        public void Execute<TStream>(int z, TStream stream) where TStream : struct, IMeshStream
        {
            int vertexIndex = 4 * Resolution * z;
            int triangleIndex = 2 * Resolution * z;

            for (int x = 0; x < Resolution; x++, vertexIndex += 4, triangleIndex += 2)
            {
                float2 xCoordinates = float2(x, x + 1f - Padding);
                float2 zCoordinates = float2(z, z + 1f - Padding);

                var vertex = new Vertex { };
                vertex.Normal.y = 1f;
                vertex.Tangent.xw = float2(1f, -1f);

                vertex.Position.x = xCoordinates.x;
                vertex.Position.z = zCoordinates.x;
                stream.SetVertex(vertexIndex, vertex);

                vertex.Position.x = xCoordinates.y;
                vertex.TexCoord0 = float2(1f, 0f);
                stream.SetVertex(vertexIndex + 1, vertex);

                vertex.Position.x = xCoordinates.x;
                vertex.Position.z = zCoordinates.y;
                vertex.TexCoord0 = float2(0f, 1f);
                stream.SetVertex(vertexIndex + 2, vertex);

                vertex.Position.x = xCoordinates.y;
                vertex.TexCoord0 = 1f;
                stream.SetVertex(vertexIndex + 3, vertex);

                stream.SetTriangle(triangleIndex, vertexIndex + int3(0, 2, 1));
                stream.SetTriangle(triangleIndex + 1, vertexIndex + int3(1, 2, 3));
            }
        }
    }

    private void Awake()
    {
        _mesh = new Mesh
        {
            name = "Square Grid Mesh",
        };

        GetComponent<MeshFilter>().mesh = _mesh;
    }

    private void Start()
    {
        GenerateMesh();
    }

    private void GenerateMesh()
    {
        Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
        Mesh.MeshData meshData = meshDataArray[0];

        MeshJob<Generator, SingleMeshStream>
            .ScheduleParallel(_mesh, meshData, new Generator
            {
                Resolution = _resolution,   // Will freeze if negative.
                Padding = _padding,
            }).Complete();

        Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, _mesh);
    }
}
