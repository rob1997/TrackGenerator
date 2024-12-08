using UnityEngine;

namespace Track
{
    public class RandomTrackGenerator : TrackGenerator
    {
        [SerializeField] private int size = 10;

        [SerializeField] private float scale = 5;
        
        [SerializeField, Range(0f, 1f)] private float complexity = .5f;
        
        protected override Path GetPath()
        {
            return new RandomPath(size, scale, complexity);
        }
    }
}