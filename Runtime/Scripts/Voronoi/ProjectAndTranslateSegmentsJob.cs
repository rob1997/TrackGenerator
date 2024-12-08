using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Voronoi
{
    [BurstCompile]
    public struct ProjectAndTranslateSegmentsJob : IJob
    {
        public NativeList<Segment> Segments;

        public float3 Forward;
        
        public float3 Up;
        
        public float3 Origin;
        
        public void Execute()
        {
            for (int i = 0; i < Segments.Length; i++)
            {
                Segment segment = Segments[i];
                
                Segments[i] = new Segment(ProjectAndTranslate(segment.Start), ProjectAndTranslate(segment.End));
            }
        }
        
        private float3 ProjectAndTranslate(float3 value)
        {
            return Utils.ProjectAndTranslate(value, Forward, Up, Origin);
        }
    }
}
