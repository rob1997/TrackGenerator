using Unity.Mathematics;

namespace Voronoi
{
    /// <summary>
    /// A segment containing intersecting and adjacent segments.
    /// </summary>
    public struct BisectorSegment
    {
        public Intersection Start { get; private set; }

        public Intersection End { get; private set; }

        public Segment Segment { get; private set; }

        public BisectorSegment(Intersection start, Intersection end)
        {
            Start = Intersection.FromStartIntersection(start);

            End = Intersection.FromEndIntersection(end);

            Segment = new Segment(Start.Point, End.Point);
        }

        // Check if bisector segment is oriented correct.
        // Checks If Center is on the same side of the bisector.
        public bool Verify(float3 center)
        {
            Segment segment = new Segment(End.Point, End.Segment.End);

            return Utils.Cross(Segment.Direction, center - Start.Point).Normalize()
                .Equals(Utils.Cross(segment.Direction, center - segment.Start).Normalize());
        }
    }
}
