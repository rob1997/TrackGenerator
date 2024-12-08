using System;
using System.Collections.Generic;
using UnityEngine;
using Voronoi;
using Random = UnityEngine.Random;

namespace Track
{
    [Serializable]
    public class RandomPath : Path
    {
        [SerializeField] private int cellCount = 10;
        
        [SerializeField] private float scale = 5;
        
        [SerializeField, Range(0f, 1f)] private float complexity = .5f;

        public override float Scale => scale;

        public override float NormalizedComplexity => complexity;
        
        public RandomPath(int cellCount, float scale, float complexity)
        {
            this.cellCount = cellCount;
            
            this.scale = scale;
            
            this.complexity = complexity;
        }

        public override List<Cell> GetCells(Transform transform)
        {
            int size = (int) Math.Sqrt(cellCount);

            // 1 for the discarded floating points and 2 for the border cells
            size += 3;
            
            List<Cell> cells = new List<Cell>();

            using (var plane = new VoronoiPlane(size, size, scale))
            {
                plane.Generate(transform);

                for (int i = 0; i < plane.Cells.Length; i++)
                {
                    Cell cell = plane.Cells[i];
                    
                    if (IsInnerCell(i, size) && cell.Verify(transform.up))
                    {
                        cells.Add(cell);
                    }
                }
            }

            int index = cells.Count / 2;

            List<int> indexed = new List<int>();
                
            List<Cell> interior = new List<Cell>();
                
            HashSet<int> adjacencyHashSet = new HashSet<int>();
                
            while (!AddCell())
            {
                index++;
            }

            for (int i = 1; i < cellCount; i++)
            {
                while (!AddCell())
                {
                    index = GetFromHashSet(Random.Range(0, adjacencyHashSet.Count), adjacencyHashSet);
                }
            }

            return interior;
            
            bool AddCell()
            {
                index %= cells.Count;

                if (indexed.Contains(index))
                {
                    return false;
                }
            
                Cell cell = cells[index];
            
                interior.Add(cell);
                
                indexed.Add(index);

                AddAdjacentCells();

                if (adjacencyHashSet.Contains(index))
                {
                    adjacencyHashSet.Remove(index);
                }
                
                return true;
            }
            
            void AddAdjacentCells()
            {
                Cell cell = cells[index];
            
                for (int i = 0; i < cells.Count; i++)
                {
                    if (i == index || indexed.Contains(i))
                    {
                        continue;
                    }

                    if (cell.IsAdjacentTo(cells[i]))
                    {
                        adjacencyHashSet.Add(i);
                    }
                }
            }
        }

        private int GetFromHashSet(int index, HashSet<int> adjacencyHashSet)
        {
            int count = 0;
            
            foreach (int value in adjacencyHashSet)
            {
                if (count == index)
                {
                    return value;
                }
                
                count++;
            }
            
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        
        // Get cell coordinates then check if it's an inner cell
        private bool IsInnerCell(int index, int size)
        {
            int row = index / size;
            
            int column = index % size;

            return row > 0 && row < size - 1 && column > 0 && column < size - 1;
        }
    }
}