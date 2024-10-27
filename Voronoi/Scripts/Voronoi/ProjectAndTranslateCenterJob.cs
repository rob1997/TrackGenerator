using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Voronoi
{
    [BurstCompile]
    public struct ProjectAndTranslateCenterJob : IJobParallelFor
    {
        public NativeArray<float3> Centers;

        public float3 Forward;
        
        public float3 Up;
        
        public float3 Origin;
        
        public void Execute(int index)
        {
            float3 center = Centers[index];
            
            Centers[index] = Utils.ProjectAndTranslate(center, Forward, Up, Origin);
        }
    }
}
