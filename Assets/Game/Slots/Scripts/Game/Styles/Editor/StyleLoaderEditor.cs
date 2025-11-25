using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Slots.Data.Styles;
using System;
using System.Runtime.Remoting.Messaging;

namespace Slots.Game.Styles
{
    [CustomEditor(typeof(StyleLoader))]
    public class StyleLoaderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(5);

            EditorGUILayout.BeginVertical();

            GUIStyle centeredLabel = new GUIStyle(GUI.skin.label);
            centeredLabel.alignment = TextAnchor.MiddleCenter;
            centeredLabel.fontSize = 16;

            GUILayout.Label("Style set", centeredLabel);

            GUILayout.Space(5);

            SerializedProperty groupProperty = serializedObject.FindProperty("_styleGroupKey");

            StyleSetGroup group;

            if (!Enum.TryParse<StyleSetGroup>(groupProperty.stringValue, out group))
                group = StyleSetGroup.FreeSpins_Header_Font;

            EditorGUI.BeginChangeCheck();

            group = (StyleSetGroup)EditorGUILayout.EnumPopup(group);

            if (EditorGUI.EndChangeCheck())
            {
                groupProperty.stringValue = group.ToString();

                serializedObject.ApplyModifiedProperties();

                (target as StyleLoader).LoadSkin();
            }

            EditorGUILayout.EndVertical();

            GUILayout.Space(5);

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5f);

            EditorGUILayout.EndVertical();

            GUILayout.Space(15);

            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}