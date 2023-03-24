using BuildingBlokks.Noise;
using UnityEditor;
using UnityEngine;

namespace BuildingBlokks.Editor
{
    [CustomEditor(typeof(NoiseGenerator))]
    public class HeightMapEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var generator = (NoiseGenerator)target;
            
            if (DrawDefaultInspector())
            {
                if (generator.autoUpdate)
                {
                    generator.DrawMap();
                }
            }

            if (GUILayout.Button("Generate"))
            {
                generator.DrawMap();
            }
        }
    }
}
