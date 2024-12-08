using Track;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(TrackGenerator), true)]
    public class TrackGeneratorEditor : UnityEditor.Editor
    {
        private TrackGenerator _trackGenerator;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _trackGenerator = (TrackGenerator) target;
            
            if (GUILayout.Button(new GUIContent("Generate", "Generate track")))
            {
                _trackGenerator.Generate();
            }
            
            if (GUILayout.Button(new GUIContent("Generate Mesh", "Generate the track mesh")))
            {
                _trackGenerator.GenerateMesh();
            }
        }
    }
}