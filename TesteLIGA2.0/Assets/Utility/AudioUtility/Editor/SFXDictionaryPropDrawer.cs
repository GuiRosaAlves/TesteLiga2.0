using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

[CustomPropertyDrawer(typeof(SFXDictionary))]
public class SFXDictionaryPropDrawer : PropertyDrawer
{
    protected SFXDictionary script;
    protected static bool foldoutState = false;
    protected float maxWindowSize = 100f;
    const float minWindowSize = 16f;

    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
    {
        script = (SFXDictionary)fieldInfo.GetValue(prop.serializedObject.targetObject);

        maxWindowSize = minWindowSize * (script.Count + 1);

        // Calculate rects
        var foldoutRect = new Rect(pos.x, pos.y, (pos.width * .4f), minWindowSize);
        var newBtnRect = new Rect(foldoutRect.x + foldoutRect.width, pos.y, (pos.width * .3f), minWindowSize);
        var clearBtnRect = new Rect(newBtnRect.x + newBtnRect.width, pos.y, (pos.width * .3f), minWindowSize);

        EditorGUI.BeginProperty(pos, label, prop);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        if ((foldoutState = EditorGUI.Foldout(foldoutRect, foldoutState, label)))
        {
            maxWindowSize += minWindowSize;
            var removeBtnRect = new Rect(pos.x, pos.y, (pos.width * .1f), minWindowSize);
            var sfxRect = new Rect(removeBtnRect.x + removeBtnRect.width, pos.y, (pos.width * .9f), minWindowSize);
            for (int i = 0; i < script.Count; i++)
            {
                removeBtnRect.y = pos.y + minWindowSize * (i+1);
                sfxRect.y = pos.y + minWindowSize * (i + 1);
                if(GUI.Button(removeBtnRect, "-"))
                {
                    if (script.Get(i).audio != null)
                    {
                        if (EditorUtility.DisplayDialog("WARNING!", "Are your sure you want to remove this item?", "Yes", "No"))
                        {
                            script.Remove(i);
                        }
                    }
                    else
                    {
                        script.Remove(i);
                    }
                }
                else
                {
                    SerializedProperty value = prop.FindPropertyRelative("library").GetArrayElementAtIndex(i);
                    EditorGUI.PropertyField(sfxRect, value, GUIContent.none);
                }
            }
        }
        if(GUI.Button(newBtnRect, "Add"))
        {
            script.Add();
            foldoutState = true;
        }
        if (GUI.Button(clearBtnRect, "Clear"))
        {
            if (script.Count != 0)
            {
                if (EditorUtility.DisplayDialog("WARNING!", "Are your sure you want to remove this item?", "Yes", "No"))
                {
                    script.Clear();
                    foldoutState = false;
                }
            }
            else
            {
                script.Clear();
            }
        }
        // Set indent back to what it was
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (foldoutState)
            return maxWindowSize;
        else
            return minWindowSize;
    }
}
