using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Splines;

namespace Track
{
    [BurstCompile]
    public struct CalculateUVsAndVerticesJob : IJobParallelFor
    {
        public NativeArray<float3> Vertices;
        
        public NativeArray<float2> UVs;
        
        public int Length;

        public NativeSpline Spline;
        
        public float Width;
        
        public void Execute(int index)
        {
            bool even = index % 2 == 0;
            
            float t = index / (float) Length;
            
            if (Spline.Evaluate(t, out float3 position, out float3 tangent, out float3 up))
            {
                float3 right = math.normalize(math.cross(tangent, up)) * Width;

                // Set Vertices
                Vertices[index] = position + (even ? right : - right);
            
                // Instead of 0 - 1 oscillation, we want 0 - 1 - 0 oscillation to avoid sudden change for the UVs at index 0
                t = 1 - math.abs(2 * t - 1);
                    
                // Set UVs
                UVs[index] = even ? new float2(0, t) : new float2(1, t);
            }

            else
            {
                throw new Exception($"Failed to evaluate spline at {t}");
            }
        }
    }
}