using System.Linq;
using Unity.Mathematics;
using Voronoi;

namespace Track
{
    public static class Utils
    {
        public const float Tolerance = 0.05f;
        
        public static bool Approximately(this float3 value, float3 other)
        {
            float tolerance = Tolerance;
            
            float3 delta = math.abs(value - other);
            
            return delta.x <= tolerance && delta.y <= tolerance && delta.z <= tolerance;
        }
        
        public static bool Approximately(this Segment segment, Segment other)
        {
            return segment.Start.Approximately(other.Start) && segment.End.Approximately(other.End);
        }
        
        public static Segment Reverse(this Segment segment)
        {
            return new Segment(segment.End, segment.Start);
        }
        
        public static bool SameAs(this Segment segment, Segment other)
        {
            return segment.Approximately(other) || segment.Approximately(other.Reverse());
        }
        
        public static float3[] Vertices(this Segment segment)
        {
            return new []{ segment.Start, segment.End };
        }
        
        public static bool IsAdjacentTo(this Cell cell, Cell other)
        {
            return cell.Segments.Any(s => other.Segments.Any(same => s.SameAs(same)));
        }
    }
}