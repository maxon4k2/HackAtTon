using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialogueResponseEvents))]

public class DialogueResponseEventsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DialogueResponseEvents responseEvents = (DialogueResponseEvents)target;

        if (GUILayout.Button(text:"Refresh"))
        {
            responseEvents.OnValidate();
        }
    }
}
