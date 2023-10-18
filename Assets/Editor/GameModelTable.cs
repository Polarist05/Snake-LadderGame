using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(MapModelTable))]
public class MapModelTableInspector : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        return new PropertyField(serializedObject.FindProperty(nameof(MapModelTable.maps)));
    }
}
