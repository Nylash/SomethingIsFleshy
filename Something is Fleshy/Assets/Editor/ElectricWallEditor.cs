using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ElectricWall))]
public class ElectricWallEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ElectricWall myScript = (ElectricWall)target;
        if (GUILayout.Button("Spawn Emitter"))
            myScript.SpawnEmitter();
    }
}
