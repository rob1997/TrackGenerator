using UnityEngine;

namespace Track
{
    public class RandomTrackGenerator : TrackGenerator
    {
        [field: SerializeField] public int Size { get; private set; } = 10;

        [field: SerializeField] public float Scale { get; private set; } = 25;
        
        [field: SerializeField, Range(0f, 1f)] public float Complexity { get; private set; } = .5f;
        
        protected override Path GetPath()
        {
            return new RandomPath(Size, Scale, Complexity);
        }
    }
}