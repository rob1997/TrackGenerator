using UnityEngine;

namespace Track
{
    public class RectTrackGenerator : TrackGenerator
    {
        [SerializeField] private Vector2Int size = new Vector2Int(3, 3);

        [SerializeField] private float scale = 5f;
        
        [SerializeField, Range(0f, 1f)] private float complexity = .5f;
        
        protected override Path GetPath()
        {
            return new RectPath(size.x, size.y, scale, complexity);
        }
    }
}
