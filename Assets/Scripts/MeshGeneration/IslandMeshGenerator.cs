using Celezt.MeshGeneration;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[HideMonoScript, ExecuteAlways]
public class IslandMeshGenerator : MonoBehaviour
{
    [SerializeField]
    private SplineContainer _splineContainer;

    private Mesh _mesh;

    public struct Generator : IMeshGenerator
    {
        public Spline Spline { get; set; }

        public int VertexCount => throw new System.NotImplementedException();

        public int IndexCount => throw new System.NotImplementedException();

        public int JobLength => throw new System.NotImplementedException();

        public Bounds Bounds => throw new System.NotImplementedException();

        public void Execute<TStream>(int index, TStream stream) where TStream : struct, IMeshStream
        {
            
        }
    }

    private void Awake()
    {
        _mesh = new Mesh
        {
            name = "Island Mesh",
        };

        GetComponent<MeshFilter>().mesh = _mesh;
    }

    private void Start()
    {
        GenerateMesh();
    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        UnityEditor.Splines.EditorSplineUtility.AfterSplineWasModified += OnSplineChanged;
#endif
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        UnityEditor.Splines.EditorSplineUtility.AfterSplineWasModified -= OnSplineChanged;
#endif
    }

    private void GenerateMesh()
    {
        if (!_splineContainer)
            return;

        Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
        Mesh.MeshData meshData = meshDataArray[0];

        //MeshJob<SquareGridMeshGenerator, SingleMeshStream>
        //    .ScheduleParallel(_mesh, meshData, new SquareGridMeshGenerator
        //    {
        //        Resolution = _resolution,
        //    }).Complete();

        Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, _mesh);
    }

#if UNITY_EDITOR
    private void OnSplineChanged(Spline spline)
    {
        if (!_splineContainer)
            return;

        if (spline != _splineContainer.Spline)
            return;

        GenerateMesh();
    }
#endif
}
