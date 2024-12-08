using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace Track
{
    [BurstCompile]
    public struct CalculateTrianglesJob : IJobParallelFor
    {
        public NativeArray<int> Triangles;
        
        public int VerticesLength;
        
        public void Execute(int i)
        {
            int remainder = i % 6;
            
            int vIndex = i < 6 ? VerticesLength : (i - remainder) / 3;

            switch (remainder)
            {
                case 1:
                case 4:
                    vIndex -= 1;
                    break;
                case 2:
                    vIndex -= 2;
                    break;
                case 3:
                    vIndex += 1;
                    break;
            }

            vIndex %= VerticesLength;
                
            Triangles[i] = vIndex;
        }
    }
}