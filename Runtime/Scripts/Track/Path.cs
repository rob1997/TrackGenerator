using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Voronoi;

namespace Track
{
    [Serializable]
    public abstract class Path : IDisposable
    {
        public List<float3> Vertices { get; private set; } = new List<float3>();

        private Transform _transform;

        public abstract float Scale { get; }
        
        // 0 - 1 range
        public abstract float NormalizedComplexity { get; }
        
        public void Generate(Transform transform)
        {
            _transform = transform;
            
            List<Cell> cells = GetCells(transform);
            
            List<Segment> segments = GetEdgeSegments(cells);
            
            GetVertices(segments);

            for (int i = 1; i < Vertices.Count; i++)
            {
                float3 current = Vertices[i];
                
                float3 next = Vertices[(i + 1) % Vertices.Count];

                float minDistance = Scale - (Scale * NormalizedComplexity);
                
                if (math.distance(current, next) < minDistance)
                {
                    Vertices.RemoveAt(i);
                    
                    i--;
                }
            }
            
#if UNITY_EDITOR
            if (!_drawing && Drawer.Instance != null)
            {
                Drawer.Instance.OnDraw += Draw;

                _drawing = true;
            }
        }

        private bool _drawing;
        
#else
        }
#endif

        public abstract List<Cell> GetCells(Transform transform);
        
        private List<Segment> GetEdgeSegments(List<Cell> interior)
        {
            NativeList<Segment> allSegments = new NativeList<Segment>(Allocator.TempJob);

            foreach (Cell cell in interior)
            {
                NativeArray<Segment> array = new NativeArray<Segment>(cell.Segments, Allocator.Temp);

                allSegments.AddRange(array);

                array.Dispose();
            }

            int length = allSegments.Length;

            NativeArray<int> edgeArray = new NativeArray<int>(length, Allocator.TempJob);

            new EdgeSegmentsJob { AllSegments = allSegments, EdgeArray = edgeArray }.Schedule(length, 16).Complete();

            List<Segment> segments = new List<Segment>();

            for (int i = 0; i < edgeArray.Length; i++)
            {
                if (edgeArray[i] == 1)
                {
                    segments.Add(allSegments[i]);
                }
            }

            allSegments.Dispose();

            edgeArray.Dispose();

            return segments;
        }

        private void GetVertices(List<Segment> segments)
        {
            Vertices.Clear();
            
            int first = GetLeftMostSegment(segments);
            
            Vertices.AddRange(segments[first].Vertices());

            List<int> visited = new List<int>{ first };
            
            int last = GetClosestVertex(false, false);
            
            while (true)
            {
                int index = GetClosestVertex();

                if (index == last)
                {
                    break;
                }
                
                Vertices.Add(segments[index].End);
                
                visited.Add(index);
            }
            
            int GetClosestVertex(bool clockwise = true, bool visit = true)
            {
                float3 vertex = clockwise ? Vertices[^1] : Vertices[0];
                
                float minimumDistance = float.MaxValue;
                
                int closest = - 1;
                
                for (int i = 0; i < segments.Count; i++)
                {
                    if (visited.Contains(i))
                    {
                        continue;
                    }
                    
                    Segment segment = segments[i];
                    
                    float distance = math.distance(vertex, clockwise ? segment.Start : segment.End);
                    
                    if (distance < minimumDistance)
                    {
                        minimumDistance = distance;
                        
                        closest = i;
                    }
                }

                if (visit)
                {
                    visited.Add(closest);
                }
                
                return closest;
            }
        }

        private int GetLeftMostSegment(List<Segment> segments)
        {
            float minimum = float.MaxValue;
            
            int leftMost = - 1;
            
            float3 left = - _transform.right;
            
            for (int i = 0; i < segments.Count; i++)
            {
                Segment segment = segments[i];
                
                foreach (float3 vertex in segment.Vertices())
                {
                    float3 direction = vertex - (float3) _transform.position;
                    
                    float dot = math.dot(left, direction);
                    
                    if (dot < minimum)
                    {
                        minimum = dot;
                        
                        leftMost = i;
                    }
                }
            }

            return leftMost;
        }

        #region Gizmo

        [Space] [SerializeField] private bool drawGizmos = true;

        [SerializeField] private float centerRadius = .125f;
        
        private void Draw()
        {
            if (!drawGizmos)
            {
                return;
            }
            
            if (Vertices != null)
            {
                for (int i = 0; i < Vertices.Count; i++)
                {
                    Gizmos.color = Color.green;
                    
                    Gizmos.DrawSphere(Vertices[i], centerRadius);

                    Gizmos.color = Color.red;
                    
                    Gizmos.DrawLine(Vertices[i], Vertices[(i + 1) % Vertices.Count]);
                }
            }
        }

        #endregion
        
        public void Dispose()
        {
#if UNITY_EDITOR
            if (_drawing && Drawer.Instance != null)
            {
                Drawer.Instance.OnDraw -= Draw;
            }
#endif
        }
    }
}