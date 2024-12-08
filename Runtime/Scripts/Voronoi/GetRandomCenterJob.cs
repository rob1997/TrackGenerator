using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Voronoi
{
    [BurstCompile]
    public struct GetRandomCenterJob : IJobParallelFor
    {
        public NativeArray<float3> Centers;

        public int Width;

        public int Height;

        public float CellSize;

        public int Seed;

        public void Execute(int index)
        {
            //Get row and column
            int column = Width > Height ? (index - (index % Height)) / Height : index % Width;

            int row = Width > Height ? index % Height : (index - (index % Width)) / Width;

            Random random = new Random((uint)(Seed * (index + 1)));

            float x = column * CellSize;

            x += random.NextFloat(0f, CellSize);

            float y = row * CellSize;

            y += random.NextFloat(0f, CellSize);

            Centers[index] = new float3(x, y, 0);
        }
    }
}
