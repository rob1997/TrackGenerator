using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Path
{
    [SerializeField, HideInInspector]
    private List<Vector3> points;
    
    [SerializeField, HideInInspector]
    private bool closed;

    public int TotalSegments => points.Count / 3;
    public int TotalPoints => points.Count;
    
    public Vector3 this[int i] => points[i];
    
    public Path(Vector3 centre)
    {
        points = new List<Vector3>
        {
            centre + Vector3.left,
            centre + (Vector3.left + Vector3.forward) * .5f,
            centre + (Vector3.right + Vector3.back) * .5f,
            centre + Vector3.right,
        };
    }

    public void AddSegment(Vector3 anchorPosition)
    {
        //add new control point for the shared anchor of end anchor of last segment
        points.Add(points[points.Count - 1] + (points[points.Count - 1] - points[points.Count - 2]));
        
        //add new control point for the end anchor of new segment
        points.Add((points[points.Count - 1] + anchorPosition) * .5f);
        
        //add new end anchor for new segment
        points.Add(anchorPosition);
    }

    public Vector3[] GetPointsInSegment(int i)
    {
        return  new Vector3[]
        {
            points[i * 3],
            points[i * 3 + 1],
            points[i * 3 + 2],
            points[LoopIndex(i * 3 + 3)],
        };
    }

    public void MovePoint(int i, Vector3 position)
    {
        //change in position
        Vector3 deltaMove = position - points[i];
        points[i] = position;

        //anchor point
        if (i % 3 == 0)
        {

            if (i + 1 < TotalPoints || closed)
            {
                points[LoopIndex(i + 1)] += deltaMove;
            }

            if (i - 1 >= 0 || closed)
            {
                points[LoopIndex(i - 1)] += deltaMove;
            }
        }

        //control point
        else
        {
            bool nextPointIsAnchor = (i + 1) % 3 == 0;

            int correspondingControlIndex = nextPointIsAnchor ? i + 2 : i - 2;
            int anchorIndex = nextPointIsAnchor ? i + 1 : i - 1;

            if (correspondingControlIndex < points.Count && correspondingControlIndex >= 0 || closed)
            {
                correspondingControlIndex = LoopIndex(correspondingControlIndex);
                anchorIndex = LoopIndex(anchorIndex);
                
                float distanceToCorrespondingControlIndex = (points[anchorIndex] - points[correspondingControlIndex]).magnitude;
                
                Vector3 directionFromAnchorToPosition = (points[anchorIndex] - position).normalized;
                
                points[correspondingControlIndex] = points[anchorIndex] + directionFromAnchorToPosition 
                                                    * distanceToCorrespondingControlIndex;
            }
        }
    }

    public void AutoMap()
    {
        Vector3[] anchors = new Vector3[TotalSegments];

        for (int i = 0; i < TotalSegments; i++)
        {
            anchors[i] = points[i * 3];
        }

        Vector3 lowestAnchor = anchors.OrderBy(a => a.z).ToArray()[0];

        anchors = anchors.OrderBy(a =>
        {
            Vector3 delta = a - lowestAnchor;

            return Mathf.Atan2(delta.z, delta.x) * Mathf.Rad2Deg;
        }).ToArray();

        for (int i = 0; i < TotalSegments; i++)
        {
            points[i * 3] = anchors[i];
        }
    }

    public void ToggleClosed()
    {
        closed = !closed;

        if (closed)
        {
            //add new control point for the shared anchor of end anchor of last segment
            points.Add(points[points.Count - 1] + (points[points.Count - 1] - points[points.Count - 2]));
            
            points.Add(points[0] + (points[0] - points[1]));
        }

        else
        {
            points.RemoveRange(TotalPoints - 2, 2);
        }
    }

    private int LoopIndex(int i)
    {
        return (i + points.Count) % points.Count;
    }
}