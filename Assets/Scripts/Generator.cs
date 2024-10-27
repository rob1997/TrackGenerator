using UnityEngine;
using Voronoi;

public class Generator : MonoBehaviour
{
    [field: SerializeField] public VoronoiPlane VoronoiPlane { get; private set; }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float startTime = Time.realtimeSinceStartup;
                
            VoronoiPlane.Generate(transform);
            
            // Execution time in milliseconds
            Debug.Log($"{(Time.realtimeSinceStartup - startTime) * 1000f}ms");
        }
    }
}