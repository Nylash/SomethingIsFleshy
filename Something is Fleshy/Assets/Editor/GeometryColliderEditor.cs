using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GeometryCollider))]
public class GeometryColliderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GeometryCollider myScript = (GeometryCollider)target;
        if (GUILayout.Button("Bake collider"))
            myScript.Bake();
    }
}
