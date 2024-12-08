using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Voronoi;

namespace Track
{
    // TODO: re-implement with Job system
    [Serializable]
    public class RectPath : Path
    {
        [SerializeField] private int width = 3;

        [SerializeField] private int height = 3;
        
        [SerializeField] private float scale = 5f;
        
        [SerializeField, Range(0f, 1f)] private float complexity = .5f;

        public override float Scale => scale;

        public override float NormalizedComplexity => complexity;

        public RectPath(int width, int height, float scale, float complexity)
        {
            this.width = width;
            
            this.height = height;
            
            this.scale = scale;
            
            this.complexity = complexity;
        }
        
        public override List<Cell> GetCells(Transform transform)
        {
            List<Cell> interior = new List<Cell>();

            List<Cell> exterior = new List<Cell>();
            
            using (VoronoiPlane plane = new VoronoiPlane(width + 2, height + 2, scale))
            {
                plane.Generate(transform);

                Cell[] cells = plane.Cells;
                
                foreach (Cell cell in cells)
                {
                    if (cell.Verify(transform.up))
                    {
                        if (FilterCell(cell.Segments, cells))
                        {
                            interior.Add(cell);
                        }
                        else
                        {
                            exterior.Add(cell);
                        }
                    }
                }
            }
            
            if (interior.Count == 0)
            {
                interior.Add(exterior[0]);
                    
                exterior.RemoveAt(0);
            }

            List<List<Cell>> clusters = GetClusters(interior);
            
            while (clusters.Count > 1)
            {
                clusters = MergeClusters(clusters, exterior);
            }

            return clusters[0];
        }
        
        private bool FilterCell(Segment[] segments, Cell[] cells)
        {
            foreach (Segment segment in segments)
            {
                bool interior = false;

                float3 previous = Voronoi.Utils.Cross(cells[0].Center - segment.Start, segment.Direction).Normalize();
                
                for (int i = 1; i < cells.Length; i++)
                {
                    float3 next = Voronoi.Utils.Cross(cells[i].Center - segment.Start, segment.Direction)
                        .Normalize();

                    if (!previous.Equals(next))
                    {
                        interior = true;

                        break;
                    }

                    previous = next;
                }
                
                if (!interior)
                {
                    return false;
                }
            }
            
            return true;
        }
        
        private List<List<Cell>> GetClusters(List<Cell> interior)
        {
            List<List<Cell>> groups = new List<List<Cell>>();
            
            HashSet<int> visited = new HashSet<int>();

            void DepthFirstSearch(int index, List<Cell> group)
            {
                Stack<int> stack = new Stack<int>();
                
                stack.Push(index);

                while (stack.Count > 0)
                {
                    int current = stack.Pop();

                    if (!visited.Add(current))
                    {
                        continue;
                    }

                    group.Add(interior[current]);

                    for (int i = 0; i < interior.Count; i++)
                    {
                        if (!visited.Contains(i) && interior[current].IsAdjacentTo(interior[i]))
                        {
                            stack.Push(i);
                        }
                    }
                }
            }

            for (int i = 0; i < interior.Count; i++)
            {
                if (!visited.Contains(i))
                {
                    List<Cell> group = new List<Cell>();
                    
                    DepthFirstSearch(i, group);
                    
                    groups.Add(group);
                }
            }

            return groups;
        }
        
        private List<List<Cell>> MergeClusters(List<List<Cell>> clusters, List<Cell> exterior)
        {
            List<Cell> clusterA = new List<Cell>(clusters[0]);

            List<Cell> clusterB;
            
            float minimumDistance = float.MaxValue;
            
            int index = 0;
            
            for (var i = 1; i < clusters.Count; i++)
            {
                clusterB = clusters[i];
                
                foreach (Cell cellA in clusterA)
                {
                    foreach (Cell cellB in clusterB)
                    {
                        foreach (float3 vertexA in cellA.Vertices)
                        {
                            foreach (float3 vertexB in cellB.Vertices)
                            {
                                float distance = math.distance(vertexA, vertexB);
                            
                                if (distance < minimumDistance)
                                {
                                    minimumDistance = distance;
                                
                                    index = i;
                                }
                            }
                        }
                    }
                }
            }

            clusterB = clusters[index];
            
            clusters.RemoveAt(index);
            
            while (true)
            {
                minimumDistance = float.MaxValue;

                index = - 1;

                for (int i = 0; i < exterior.Count; i++)
                {
                    Cell cell = exterior[i];
                    
                    foreach (Cell cellA in clusterA)
                    {
                        if (cell.IsAdjacentTo(cellA))
                        {
                            foreach (Cell cellB in clusterB)
                            {
                                if (cell.IsAdjacentTo(cellB))
                                {
                                    clusterA.AddRange(clusterB);
                                    
                                    clusterA.Add(cell);
                                    
                                    clusters[0] = clusterA;
                                    
                                    return clusters;
                                }

                                foreach (float3 vertex in cell.Vertices)
                                {
                                    foreach (float3 vertexB in cellB.Vertices)
                                    {
                                        float distance = math.distance(vertex, vertexB);

                                        if (distance < minimumDistance)
                                        {
                                            minimumDistance = distance;
                                            
                                            index = i;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                clusterA.Add(exterior[index]);

                exterior.RemoveAt(index);
            }
        }
    }
}