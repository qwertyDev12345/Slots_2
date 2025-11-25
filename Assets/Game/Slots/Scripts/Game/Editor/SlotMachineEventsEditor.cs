using UnityEditor;
using UnityEngine;

namespace Slots.Game
{
    [CustomEditor(typeof(SlotMachineEvents))]
    public class SlotMachineEventsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            
            GUILayout.Space(5);

            DrawMainEvents();
            
            GUILayout.Space(5);
            
            DrawSlotsEvents();
            
            GUILayout.Space(5);
            
            DrawAudioEvents();
            
            GUILayout.Space(5);

            EditorGUILayout.EndVertical();

            GUILayout.Space(15);

            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawMainEvents()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.BeginVertical();

            GUILayout.Space(5);

            GUIStyle centeredLabel = new GUIStyle(GUI.skin.label);
            centeredLabel.alignment = TextAnchor.MiddleCenter;
            centeredLabel.fontSize = 16;

            GUILayout.Label("Main Events", centeredLabel);

            GUILayout.Space(5);
            
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_onClickExit"));
            
            GUILayout.Space(5);
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_onEndMoney"));
            
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.EndVertical();
        }

        private void DrawSlotsEvents()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.BeginVertical();

            GUILayout.Space(5);

            GUIStyle centeredLabel = new GUIStyle(GUI.skin.label);
            centeredLabel.alignment = TextAnchor.MiddleCenter;
            centeredLabel.fontSize = 16;

            GUILayout.Label("Slots Events", centeredLabel);

            GUILayout.Space(5);
            
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_onWinningLine"));
            
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.EndVertical();
        }

        private void DrawAudioEvents()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.BeginVertical();

            GUILayout.Space(5);

            GUIStyle centeredLabel = new GUIStyle(GUI.skin.label);
            centeredLabel.alignment = TextAnchor.MiddleCenter;
            centeredLabel.fontSize = 16;

            GUILayout.Label("Audio Events", centeredLabel);

            GUILayout.Space(5);
            
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_playClick"));
            
            GUILayout.Space(5);
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_playSpin"));
            
            GUILayout.Space(5);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_playWin"));

            GUILayout.Space(5);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_playLose"));

            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.EndVertical();
        }
    }
}