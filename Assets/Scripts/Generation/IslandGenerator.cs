using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[HideMonoScript, ExecuteAlways]
public class IslandGenerator : MonoBehaviour
{
    [SerializeField]
    private SplineContainer _splineContainer;
    [SerializeField, Space(4)]
    private int _resolution = 2;

    private List<UnityEngine.Vector3> _vertices;
    private List<int> _indices;

    private Mesh _mesh;
    private MeshFilter _meshFilter;

    //[BurstCompile]
    public struct Generator : IJob
    {
        public Spline Spline;
        public NativeList<float3> Vertices;
        public NativeList<int> Indices;

        public void Execute()
        {
            for (int i = 0, indices = 0; i < Spline.Count; i++)
            {
                BezierKnot bezierKnot = Spline[i];
                Vertices.Add(bezierKnot.Position);

                Indices.Add(indices++);
            }
        }
    }

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();

        _mesh = new Mesh 
        {
            name = "Island Mesh"
        };

        _meshFilter.mesh = _mesh;
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

        _vertices = new();
        _indices = new();

        var spline = _splineContainer.Spline;

        for (int i = 0, indices = 0; i < spline.Count; i++)
        {
            BezierKnot bezierKnot = spline[i];
            _vertices.Add(bezierKnot.Position);

            _indices.Add(indices++);
        }

        _indices.Add(0);

        //int worldX = 10;
        //int worldZ = 10;

        //for (int z = 0; z <= worldZ; z++)
        //{
        //    for (int x = 0; x <= worldX; x++)
        //    {
        //        _vertices.Add(new Vector3(x, 0, z));
        //    }
        //}

        //for (int vertex = 0, z = 0; z < worldZ; z++)
        //{
        //    for (int x = 0; x < worldX; x++)
        //    {
        //        _indices.Add(vertex);
        //        _indices.Add(vertex + worldZ + 1);
        //        _indices.Add(vertex + 1);

        //        _indices.Add(vertex + 1);
        //        _indices.Add(vertex + worldZ + 1);
        //        _indices.Add(vertex + worldZ + 2);
        //        vertex++;
        //    }

        //    vertex++;
        //}

        _mesh.Clear();
        _mesh.SetVertices(_vertices);
        _mesh.SetIndices(_indices, MeshTopology.LineStrip, 0);

        //_mesh.RecalculateNormals();
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
