using Unity.Mathematics;

namespace Voronoi
{
    public struct Segment
    {
        public float3 Start { get; private set; }
        
        public float3 End { get; private set; }
        
        public float3 Direction => End - Start;

        public float3 Center { get; private set; }

        public Segment(float3 start, float3 end)
        {
            Start = start;

            End = end;

            Center = (start + end) / 2f;
        }
    }
}
