using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SFX))]
public class SFXPropDrawer : PropertyDrawer
{
    GUIStyle labelStyle = new GUIStyle();

    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
    {
        var labelRect = new Rect(pos.x, pos.y, (pos.width * .2f), pos.height);
        var stringRect = new Rect(labelRect.position.x + labelRect.width, pos.y, (pos.width * .4f), pos.height);
        var audioRect = new Rect(stringRect.position.x + stringRect.width, pos.y, (pos.width * .4f), pos.height);

        labelStyle.clipping = TextClipping.Clip;

        if (label.text.Contains("Element"))
        {
            stringRect.x = pos.x;
            stringRect.width = (pos.width * .5f);
            audioRect.x = stringRect.x + stringRect.width;
            audioRect.width = (pos.width * .5f);
        }
        else
        {
            EditorGUI.LabelField(labelRect, label, labelStyle);
        }
        EditorGUI.PropertyField(stringRect, prop.FindPropertyRelative("tag"), GUIContent.none);
        EditorGUI.PropertyField(audioRect, prop.FindPropertyRelative("audio"), GUIContent.none);
    }
}
