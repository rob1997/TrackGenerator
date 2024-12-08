using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Voronoi
{
    [BurstCompile]
    public struct CellJob : IJob
    {
        [ReadOnly]
        public NativeArray<float3> Centers;

        public NativeList<Segment> Segments;

        public int Index;
        
        public Rect BoundingRect;
        
        public void Execute()
        {
            float3 center = Centers[Index];

            GetSegmentsFromRect();

            for (int i = 0; i < Centers.Length; i++)
            {
                if (i == Index)
                {
                    continue;
                }

                float3 other = Centers[i];

                Segment connectingSegment = new Segment(center, other);

                float3 bisectorDirection = Utils.Cross(connectingSegment.Direction, new float3(0, 0, 1)).Normalize() * BoundingRect.Diagonal;

                float3 bisectorEnd = bisectorDirection + connectingSegment.Center;

                Segment bisector = new Segment(connectingSegment.Center - bisectorDirection, bisectorEnd);

                if (GetIntersections(bisector, out NativeHashSet<Intersection> intersections))
                {
                    NativeArray<Intersection> intersectionArray = intersections.ToNativeArray(Allocator.Temp);
                    
                    intersections.Dispose();
                    
                    GetSegmentsFromIntersections(intersectionArray[0], intersectionArray[1]);
                    
                    intersectionArray.Dispose();
                }
            }
        }

        private void GetSegmentsFromRect()
        {
            Segments.ResizeUninitialized(4);
            
            Segment segment = Segments[0] = new Segment(BoundingRect.Min, new float3(BoundingRect.MinX, BoundingRect.MaxY, 0));
        
            segment = Segments[1] = new Segment(segment.End, BoundingRect.Max);
        
            segment = Segments[2] = new Segment(segment.End, new float3(BoundingRect.MaxX, BoundingRect.MinY, 0));
        
            Segments[3] = new Segment(segment.End, BoundingRect.Min);
        }

        private bool GetIntersections(Segment bisector, out NativeHashSet<Intersection> intersections)
        {
            intersections = new NativeHashSet<Intersection>(0, Allocator.Temp);

            foreach (var segment in Segments)
            {
                if (GetIntersection(bisector, segment, out Intersection intersection))
                {
                    intersections.Add(intersection);
                }
            }

            return intersections.Count == 2;
        }
        
        private bool GetIntersection(Segment bisector, Segment segment, out Intersection intersection)
        {
            intersection = default;

            // Segment AB
            float3 a = bisector.Start;
            float3 b = bisector.End;

            // Segment CD
            float3 c = segment.Start;
            float3 d = segment.End;

            float t = ((a.x - c.x) * (c.y - d.y) - (a.y - c.y) * (c.x - d.x)) /
                      ((a.x - b.x) * (c.y - d.y) - (a.y - b.y) * (c.x - d.x));

            float u = -((a.x - b.x) * (a.y - c.y) - (a.y - b.y) * (a.x - c.x)) /
                      ((a.x - b.x) * (c.y - d.y) - (a.y - b.y) * (c.x - d.x));

            // Both t & u are between 0 and 1
            if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
            {
                var point = a + t * (b - a);

                intersection = new Intersection(point, segment);

                return true;
            }

            return false;
        }

        private void GetSegmentsFromIntersections(Intersection first, Intersection second)
        {
            BisectorSegment bisector = GetBisectorSegment(first, second);

            NativeList<Segment> segments = new NativeList<Segment>(Allocator.Temp)
            {
                bisector.Segment,
                
                bisector.End.Segment
            };
        
            Segment segment = segments[1];
            
            while (!segment.End.Equals(bisector.Start.Point))
            {
                segment = Next(segment);

                // If last segment
                if (bisector.Start.Segment.Start.Equals(segment.Start))
                {
                    segment = bisector.Start.Segment;
                }
            
                segments.Add(segment);
            }

            Segments.CopyFrom(segments);

            segments.Dispose();
        }
        
        // Get the correct bisector segment from two intersections.
        private BisectorSegment GetBisectorSegment(Intersection first, Intersection second)
        {
            float3 center = Centers[Index];
            
            BisectorSegment segment = new BisectorSegment(first, second);
            
            return segment.Verify(center) ? segment : new BisectorSegment(second, first);
        }

        private Segment Next(Segment segment)
        {
            for (int i = 0; i < Segments.Length; i++)
            {
                Segment current = Segments[i];
                
                if (segment.End.Equals(current.Start))
                {
                    return current;
                }
            }

            throw new Exception("Next segment not found");
        }
        
        private Segment NextUnique(Segment segment)
        {
            int index = - 1;
            
            for (int i = 0; i < Segments.Length; i++)
            {
                Segment current = Segments[i];
                
                if (segment.End.Equals(current.Start))
                {
                    if (index != - 1)
                    {
                        // TODO: this keeps happening sometimes
                        throw new Exception("Next segment not unique");
                    }
                    
                    index = i;
                }
            }

            if (index == - 1)
            {
                throw new Exception("Next segment not found");
            }

            return Segments[index];
        }
    }
}