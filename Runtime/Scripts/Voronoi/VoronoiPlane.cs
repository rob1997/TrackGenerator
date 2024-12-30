using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Voronoi
{
    [Serializable]

    public class VoronoiPlane : IDisposable
    {
        [SerializeField] private int planeWidth = 5;

        [SerializeField] private int planeHeight = 5;

        [SerializeField] private float cellSize = 5f;

        [SerializeField] private float padding = 1f;

        public Cell[] Cells { get; private set; }

        private Rect _boundingRect;

        public VoronoiPlane(int width, int height, float size)
        {
            planeWidth = width;
            
            planeHeight = height;
            
            cellSize = size;
        }
        
        public void Generate(Transform transform)
        {
            Vector3 position = transform.position;
            
            position += transform.right * (planeWidth * cellSize) / 2f;
            
            position -= transform.forward * (planeHeight * cellSize) / 2f;
            
            Generate(position, transform.forward, transform.up);
        }
        
        public void Generate(Vector3 origin, Vector3 forward, Vector3 up)
        {
            int arrayLength = planeWidth * planeHeight;
            
            var centers = new NativeArray<float3>(arrayLength, Allocator.TempJob);
            
            // Get random voronoi cell center points
            new GetRandomCenterJob
            {
                Centers = centers,
                
                Width = planeWidth,
                
                Height = planeHeight,
                
                CellSize = cellSize,
                
                Seed = Random.Range(0, int.MaxValue)
            }.Schedule(arrayLength, 16).Complete();

            _boundingRect = GetBoundingRect(forward, up);

            var segmentsArray = new NativeList<Segment>[arrayLength];
            
            var allJobs = new NativeArray<JobHandle>(arrayLength, Allocator.Temp);
            
            for (int i = 0; i < arrayLength; i++)
            {
                segmentsArray[i] = new NativeList<Segment>(0, Allocator.TempJob);
                
                allJobs[i] = new CellJob
                {
                    Centers = centers,
                    
                    Segments = segmentsArray[i],
                    
                    Index = i,
                    
                    BoundingRect = _boundingRect,
                }.Schedule();
            }
            
            JobHandle.CompleteAll(allJobs);

            // Project and translate center points
            new ProjectAndTranslateCenterJob
            {
                Centers = centers,
                
                Forward = forward,
                
                Up = up,
                
                Origin = origin
            }.Schedule(arrayLength, 8).Complete();
            
            for (int i = 0; i < arrayLength; i++)
            {
                allJobs[i] = new ProjectAndTranslateSegmentsJob
                {
                    Segments = segmentsArray[i],

                    Forward = forward,
                    
                    Up = up,

                    Origin = origin
                }.Schedule();
            }

            JobHandle.CompleteAll(allJobs);
            
            allJobs.Dispose();

            _boundingRect.ProjectAndTranslate(forward, up, origin);
            
            Cells = new Cell[arrayLength];
            
            for (int i = 0; i < arrayLength; i++)
            {
                Cells[i] = new Cell(centers[i], segmentsArray[i].AsArray().ToArray());

                segmentsArray[i].Dispose();
            }
            
            centers.Dispose();
            
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

        private Rect GetBoundingRect(float3 forward, float3 up)
        {
            float minX = - padding;
            float minY = - padding;

            float maxX = (planeWidth * cellSize) + padding;
            float maxY = (planeHeight * cellSize) + padding;

            return new Rect(new float2(minX, minY), new float2(maxX, maxY), forward, up);
        }

        #region Gizmo

        [Space] [SerializeField] private bool drawGizmos = true;

        [SerializeField] private float centerRadius = .25f;

        public void Draw()
        {
            if (!drawGizmos)
            {
                return;
            }

            if (Cells != null)
            {
                // Draw bounding box
                Gizmos.color = Color.white;

                Gizmos.DrawLine(_boundingRect.Min, _boundingRect.TopLeft);

                Gizmos.DrawLine(_boundingRect.TopLeft, _boundingRect.Max);

                Gizmos.DrawLine(_boundingRect.Max, _boundingRect.BottomRight);

                Gizmos.DrawLine(_boundingRect.BottomRight, _boundingRect.Min);

                foreach (var cell in Cells)
                {
                    Gizmos.color = Color.green;

                    // Draw cell center
                    Gizmos.DrawSphere(cell.Center, centerRadius);
                    
                    Gizmos.color = Color.white;
                    
                    // Draw cell sizes/segments
                    foreach (var segment in cell.Segments)
                    {
                        Gizmos.DrawLine(segment.Start, segment.End);
                    }
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
