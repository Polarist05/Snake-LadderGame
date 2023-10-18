using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(GameAction))]
public class GameActionDrawer: PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var root = new VisualElement();
        root.Add(new IntegerField(maxLength: 100, label: nameof(GameAction.position)) { bindingPath = nameof(GameAction.position) });
        root.Add(new EnumField(label: nameof(GameAction.type)) { bindingPath = nameof(GameAction.type) });
        root.Add(new PropertyField(property.FindPropertyRelative(nameof(GameAction.distance)),label:nameof(GameAction.distance))); 
        return root;
    }
}
