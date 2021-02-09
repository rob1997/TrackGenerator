using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathCreator))]
public class PathEditor : Editor {

    PathCreator _creator;
    Path _path;

    void OnEnable()
    {
        _creator = (PathCreator)target;
        if (_creator.path == null)
        {
            _creator.CreatePath();
        }
        _path = _creator.path;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Create New Path"))
        {
            Undo.RecordObject(_creator, "Create New Path");
            _creator.CreatePath();
            _path = _creator.path;
            
            SceneView.RepaintAll();
        }
        
        if (GUILayout.Button("Toggle Closed Path"))
        {
            Undo.RecordObject(_creator, "Toggle Closed Path");
            _path.ToggleClosed();
            
            SceneView.RepaintAll();
        }
        
        if (GUILayout.Button("Auto Map"))
        {
            Undo.RecordObject(_creator, "AutoMap");
            _path.AutoMap();
            SceneView.RepaintAll();
        }
    }

    void OnSceneGUI()
    {
        Input();
        
        Draw();
    }

    void Input()
    {
        Event guiEvent = Event.current;
        
        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
        {
            float distanceCamToEndAnchor = (Camera.current.transform.position - _path[_path.TotalPoints - 1]).magnitude;
            
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            Vector3 worldMousePosition = mouseRay.GetPoint(distanceCamToEndAnchor);
            
            float yDir = mouseRay.direction.y;
            
            if (Math.Abs(yDir) > .01f)
            {
                float distanceToXzPlane = Mathf.Abs(mouseRay.origin.y / yDir);
                worldMousePosition = mouseRay.GetPoint(distanceToXzPlane);
            }
            
            Undo.RecordObject(_creator, "Add segment");
            _path.AddSegment(worldMousePosition);
        }
    }
    
    void Draw()
    {
        Handles.color = Color.black;

        for (int i = 0; i < _path.TotalSegments; i++)
        {
            Vector3[] points = _path.GetPointsInSegment(i);
            
            Handles.DrawLine(points[0], points[1]);
            Handles.DrawLine(points[2], points[3]);
            
            Handles.DrawBezier(points[0], points[3], 
                points[1], points[2], Color.green, null, 1f);
        }
        
        Handles.color = Color.red;

        for (int i = 0; i < _path.TotalPoints; i++)
        {
            Vector3 handlePosition = Handles.FreeMoveHandle(_path[i], 
                Quaternion.identity, .1f, Vector2.zero, Handles.SphereHandleCap);
            
            if (_path[i] != handlePosition)
            {
                Undo.RecordObject(_creator, "Move Handle");
                _path.MovePoint(i, handlePosition);
            }
        }
    }
}