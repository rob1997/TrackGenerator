using UnityEngine;

namespace Track
{
    public class RectTrackGenerator : TrackGenerator
    {
        [field: SerializeField] public Vector2Int Size { get; private set; } = new Vector2Int(3, 3);

        [field: SerializeField] public float Scale { get; private set; } = 5f;
        
        [field: SerializeField, Range(0f, 1f)] public float Complexity { get; private set; } = .5f;
        
        protected override Path GetPath()
        {
            return new RectPath(Size.x, Size.y, Scale, Complexity);
        }
    }
}
