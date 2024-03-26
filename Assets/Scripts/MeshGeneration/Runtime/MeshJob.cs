using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Celezt.MeshGeneration
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    public struct MeshJob<TGenerator, TStream> : IJobFor
        where TGenerator : struct, IMeshGenerator 
        where TStream : struct, IMeshStream
    {
        public TGenerator Generator;
        [WriteOnly]
        public TStream Stream;

        public void Execute(int index)
        {
            Generator.Execute(index, Stream);
        }

        public static JobHandle ScheduleParallel(Mesh mesh, Mesh.MeshData meshData, IMeshGenerator generator = default, IMeshStream stream = default, JobHandle dependency = default)
            => ScheduleParallel(mesh, meshData, (TGenerator)generator, (TStream)stream, dependency);
        public static JobHandle ScheduleParallel(Mesh mesh, Mesh.MeshData meshData, TGenerator generator = default, TStream stream = default, JobHandle dependency = default)
        {
            var job = new MeshJob<TGenerator, TStream>()
            {
                Generator = generator,
                Stream = stream,
            };

            job.Stream.Setup(
                meshData, 
                mesh.bounds = job.Generator.Bounds, 
                job.Generator.VertexCount, 
                job.Generator.IndexCount);

            return job.ScheduleParallel(job.Generator.JobLength, 1, dependency);
        }
    }

    public delegate JobHandle MeshJobScheduleDelegate(
        Mesh mesh, Mesh.MeshData meshData, IMeshGenerator generator = default, IMeshStream stream = default, JobHandle dependency = default);
}
