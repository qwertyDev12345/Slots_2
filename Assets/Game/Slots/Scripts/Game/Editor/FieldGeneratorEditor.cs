using Slots.Data;
using Slots.Data.Slots;
using Slots.Data.Styles;
using UnityEditor;
using UnityEngine;

namespace Slots.Game
{
    [CustomEditor(typeof(FieldGenerator))]
    public class FieldGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            
            GUILayout.Space(5);

            DrawSlotMachineSettings();
            
            GUILayout.Space(5);
            
            DrawStylesSettings();
            
            GUILayout.Space(5);
            
            DrawSlotsSettings();
            
            GUILayout.Space(5);
            
            DrawOtherSettings();
            
            GUILayout.Space(5);

            DrawPreloadButton();
            
            GUILayout.Space(5);

            EditorGUILayout.EndVertical();

            GUILayout.Space(15);

            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSlotMachineSettings()
        {
            SerializedProperty settingsProperty = serializedObject.FindProperty("_settings");

            Color color = GUI.color;

            if (settingsProperty.objectReferenceValue == null)
                GUI.color = Color.red;

            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.BeginVertical();

            GUILayout.Space(5);

            GUIStyle centeredLabel = new GUIStyle(GUI.skin.label);
            centeredLabel.alignment = TextAnchor.MiddleCenter;
            centeredLabel.fontSize = 16;

            GUILayout.Label("SlotMachine Settings", centeredLabel);

            GUILayout.Space(5);

            EditorGUI.BeginChangeCheck();

            settingsProperty.objectReferenceValue = EditorGUILayout.ObjectField(settingsProperty.objectReferenceValue, typeof(SlotMachineSettings), false);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();

                (target as FieldGenerator).PreloadSlotMachineDesign();
            }

            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.EndVertical();
            
            GUI.color = color;
        }

        private void DrawStylesSettings()
        {
            SerializedProperty settingsProperty = serializedObject.FindProperty("_styles");

            Color color = GUI.color;

            if (settingsProperty.objectReferenceValue == null)
                GUI.color = Color.red;

            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.BeginVertical();

            GUILayout.Space(5);

            GUIStyle centeredLabel = new GUIStyle(GUI.skin.label);
            centeredLabel.alignment = TextAnchor.MiddleCenter;
            centeredLabel.fontSize = 16;

            GUILayout.Label("Styles Settings", centeredLabel);

            GUILayout.Space(5);
            
            EditorGUI.BeginChangeCheck();

            settingsProperty.objectReferenceValue = EditorGUILayout.ObjectField(settingsProperty.objectReferenceValue, typeof(StylesData), false);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();

                (target as FieldGenerator).PreloadSlotMachineDesign();
            }

            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.EndVertical();
            
            GUI.color = color;
        }

        private void DrawSlotsSettings()
        {
            SerializedProperty settingsProperty = serializedObject.FindProperty("_slots");

            Color color = GUI.color;

            if (settingsProperty.objectReferenceValue == null)
                GUI.color = Color.red;

            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.BeginVertical();

            GUILayout.Space(5);

            GUIStyle centeredLabel = new GUIStyle(GUI.skin.label);
            centeredLabel.alignment = TextAnchor.MiddleCenter;
            centeredLabel.fontSize = 16;

            GUILayout.Label("Slots Settings", centeredLabel);

            GUILayout.Space(5);

            EditorGUI.BeginChangeCheck();

            settingsProperty.objectReferenceValue = EditorGUILayout.ObjectField(settingsProperty.objectReferenceValue, typeof(SlotsData), false);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();

                (target as FieldGenerator).PreloadSlotMachineDesign();
            }

            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.EndVertical();
            
            GUI.color = color;
        }

        private void DrawOtherSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.BeginVertical();

            GUILayout.Space(5);

            GUIStyle centeredLabel = new GUIStyle(GUI.skin.label);
            centeredLabel.alignment = TextAnchor.MiddleCenter;
            centeredLabel.fontSize = 16;

            GUILayout.Label("Other Settings", centeredLabel);

            GUILayout.Space(10);

            SerializedProperty slotMachineProperty = serializedObject.FindProperty("_slotMachine");
            EditorGUILayout.PropertyField(slotMachineProperty);
            
            GUILayout.Space(5);

            SerializedProperty slotsProperty = serializedObject.FindProperty("_slotsLine");
            EditorGUILayout.PropertyField(slotsProperty);
            
            GUILayout.Space(5);

            SerializedProperty linesProperty = serializedObject.FindProperty("_lines");
            EditorGUILayout.PropertyField(linesProperty);
            
            GUILayout.Space(5);

            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.EndVertical();
        }

        private void DrawPreloadButton()
        {
            if (GUILayout.Button("Preload SlotMachine"))
                (target as FieldGenerator).PreloadSlotMachineDesign();
        }
    }
}