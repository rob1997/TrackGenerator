using Unity.Mathematics;

namespace Voronoi
{
    public struct Cell
    {
        public float3 Center { get; private set; }

        public Segment[] Segments { get; private set; }

        public float3[] Vertices { get; private set; }
        
        public Cell(float3 center, Segment[] segments)
        {
            Center = center;
            
            Segments = segments;
            
            Vertices = new float3[segments.Length];
            
            for (int i = 0; i < segments.Length; i++)
            {
                Vertices[i] = segments[i].Start;
            }
        }
    }
}
