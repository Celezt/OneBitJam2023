using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;

using static Unity.Mathematics.math;

namespace Celezt.MeshGeneration
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    [HideMonoScript, ExecuteAlways]
    public class IslandGenerator : MonoBehaviour
    {
        private readonly half _h0 = half(0f);
        private readonly half _h1 = half(1f);

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
                name = "Island Mesh",
            };

            GenerateMesh();

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

            Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
            Mesh.MeshData meshData = meshDataArray[0];

            MeshJob<SquareGridMeshGenerator, SingleMeshStream>.ScheduleParallel(meshData).Complete();

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
}
