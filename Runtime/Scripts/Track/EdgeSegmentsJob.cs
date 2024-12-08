using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Voronoi;

namespace Track
{
    [BurstCompile]
    public struct EdgeSegmentsJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeList<Segment> AllSegments;

        public NativeArray<int> EdgeArray;
        
        public void Execute(int index)
        {
            for (int i = 0; i < AllSegments.Length; i++)
            {
                if (i == index)
                {
                    continue;
                }
                
                Segment segment = AllSegments[index];

                if (segment.SameAs(AllSegments[i]))
                {
                    EdgeArray[index] = 0;
                    
                    return;
                }
            }
            
            EdgeArray[index] = 1;
        }
    }
}