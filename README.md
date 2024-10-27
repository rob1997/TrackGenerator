# Voronoi
[![Made with Unity](https://img.shields.io/badge/Made%20with-Unity-57b9d3.svg?style=flat&logo=unity)](https://unity3d.com)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A Vornoi Diagram Generator on Unity using C# Job System (DOTS).

Unity Version: `2022.3.44f1`

## How to use
#### 1. Install Dependencies
Install dependency package `Burst` and `Collections` Packages from the Package Manager.
#### 2. Import `Voronoi.unitypackage` in your project
Download and import `Voronoi.unitypackage` into your project found [here](https://github.com/rob1997/Voronoi/releases/).
#### 3. Usage
- Create a serialized `VoronoiPlane` object in your MonoBehaviour script and Generate it.
```csharp
    [field: SerializeField] public VoronoiPlane VoronoiPlane { get; private set; }

    private void Update()
    {
        // Generate Voronoi Diagram on Space Key Press
        if (Input.GetKeyDown(KeyCode.Space))
        {
            VoronoiPlane.Generate(transform);
        }
    }
```
- The `VoronoiPlane.Generate()` method takes a `Transform` object as an argument that determines the position and rotation of the diagram, you can also alternatively use `VoronoiDiagram.Generate(Vector3 origin, Vector3 forward, Vector3 up)` where `origin` will be used to position the diagram while `forward` and `up` are used to rotate the diagram.
- You can also use the `VoronoiPlane` object to get the generated Voronoi Diagram Cells.
```csharp
    // Get the Voronoi Diagram data
    Cell[] cells = VoronoiPlane.Cells;
```
- Each cell contains the center point of each cell as `Cell.Center` and an array of segments and vertices for each cell arranged in a **clockwise** order as `Cell.Segments` and `Cell.Vertices`.
```csharp
    Cell[] cells = VoronoiPlane.Cells;
    
    for (int i = 0; i < cells.Length; i++)
    {
        Cell cell = cells[i];
        
        // Center point of a Voronoi cell
        Vector3 center = cell.Center;
        
        // Segments of a single Voronoi cell arranged in a clockwise manner
        Segment[] segments = cell.Segments;
        
        // Vertices of a single Voronoi cell arranged in a clockwise manner
        float3[] vertices = cell.Vertices;
    }
``` 
- Each segment contains the start and end points of the segment as `Segment.Start` and `Segment.End` ordered clockwise.
- You can also visualize the Voronoi Diagram by either adding a `Drawer` instance in your scene and enabling `VoronoiPlane.drawGizmos` in the inspector or by calling `VoronoiPlane.Draw()` in `OnDrawGizmos`.
