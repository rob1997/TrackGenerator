using Unity.Mathematics;

namespace Voronoi
{
    /// <summary>
    /// Rect implementation that works with Unity Jobs.
    /// </summary>
    public struct Rect
    {
        public float3 Min;
        public float3 Max;
        
        public readonly float Diagonal;

        private readonly float3 _forward;
        
        private readonly float3 _up;
        
        private readonly float _width;
        
        private readonly float3 _height;
        
        public float MinX => Min.x;
        public float MinY => Min.y;

        public float MaxX => Max.x;
        public float MaxY => Max.y;

        public float3 TopLeft => Min + _forward * _height;
        
        public float3 BottomRight => Min + Utils.Cross(_forward, _up) * _width;
        
        public Rect(float2 min, float2 max, float3 forward, float3 up)
        {
            Min = new float3(min.xy, 0);
            Max = new float3(max.xy, 0);
            
            Diagonal = math.distance(Min, Max);

            float2 delta = max - min;

            _width = delta.x;
            
            _height = delta.y;
            
            _forward = forward.Normalize();
            
            _up = up.Normalize();
        }

        public void ProjectAndTranslate(float3 forward, float3 up, float3 origin)
        {
            Min = Utils.ProjectAndTranslate(Min, forward, up, origin);

            Max = Utils.ProjectAndTranslate(Max, forward, up, origin);
        }
    }
}
