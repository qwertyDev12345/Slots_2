using UnityEditor;
using UnityEngine;

namespace Slots.Data
{
    namespace Slots.Data.Styles
    {
        [CustomEditor(typeof(SlotMachineSettings))]
        public class SlotMachineSettingsEditor : Editor
        {
            private SerializedProperty _fieldSizeProperty;
            private SerializedProperty _maxBetProperty;
            private SerializedProperty _combinationsProperty;

            private Texture2D _activeTexture = null;
            private Texture2D _disabledTexture = null;

            private void OnEnable()
            {
                _activeTexture = MakeTex(2, 2, Color.green);
                _disabledTexture = MakeTex(2, 2, Color.grey);

                _fieldSizeProperty = serializedObject.FindProperty("_fieldSize");
                _maxBetProperty = serializedObject.FindProperty("_maxBet");
                _combinationsProperty = serializedObject.FindProperty("_combinations");
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                GUILayout.Space(10);

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.BeginVertical();

                DrawBaseSettings();

                GUILayout.Space(10);

                DrawCombinations();

                EditorGUILayout.EndVertical();

                GUILayout.Space(15);

                EditorGUILayout.EndHorizontal();

                GUILayout.Space(10);

                serializedObject.ApplyModifiedProperties();
            }

            private void DrawBaseSettings()
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);

                GUILayout.Space(5);

                DrawHeader("Base Settings");

                GUILayout.Space(5);

                EditorGUILayout.BeginHorizontal();

                GUILayout.Space(10);

                EditorGUILayout.LabelField("Field size", GUILayout.Width(100f));

                Vector2Int size = _fieldSizeProperty.vector2IntValue;

                size.x = EditorGUILayout.IntField(size.x, GUILayout.ExpandWidth(true));

                GUIStyle centeredLabel = new GUIStyle(GUI.skin.label);
                centeredLabel.alignment = TextAnchor.UpperCenter;

                EditorGUILayout.LabelField("x", centeredLabel, GUILayout.Width(10f));

                size.y = EditorGUILayout.IntField(size.y, GUILayout.ExpandWidth(true));

                _fieldSizeProperty.vector2IntValue = size;

                GUILayout.Space(10);

                EditorGUILayout.EndHorizontal();

                GUILayout.Space(5);

                EditorGUILayout.BeginHorizontal();

                GUILayout.Space(10);

                EditorGUILayout.LabelField("Max bet", GUILayout.Width(100f));

                int maxBet = _maxBetProperty.intValue;

                maxBet = EditorGUILayout.IntField(maxBet, GUILayout.ExpandWidth(true));

                _maxBetProperty.intValue = maxBet;

                GUILayout.Space(10);

                EditorGUILayout.EndHorizontal();

                GUILayout.Space(10f);

                EditorGUILayout.EndVertical();
            }

            private void DrawCombinations()
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);

                GUILayout.Space(5);

                DrawHeader("Combinations");

                EditorGUILayout.BeginHorizontal();

                GUILayout.Space(10);

                EditorGUILayout.BeginVertical();

                for (int i = 0; i < _combinationsProperty.arraySize; i++)
                {
                    GUILayout.Space(5);

                    SerializedProperty combinationProperty = _combinationsProperty.GetArrayElementAtIndex(i);

                    NormalizeCombination(combinationProperty, _fieldSizeProperty.vector2IntValue);
                    DrawCombination(combinationProperty, i);
                }

                GUILayout.Space(5);

                AddButton();

                EditorGUILayout.EndVertical();

                GUILayout.Space(10);

                EditorGUILayout.EndHorizontal();

                GUILayout.Space(10f);

                EditorGUILayout.EndVertical();
            }

            private void AddButton()
            {
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Add new combination"))
                {
                    _combinationsProperty.InsertArrayElementAtIndex(_combinationsProperty.arraySize);
                }

                EditorGUILayout.EndHorizontal();
            }

            private void NormalizeCombination(SerializedProperty combinationProperty, Vector2Int fieldSize)
            {
                SerializedProperty columnsProperty = combinationProperty.FindPropertyRelative("_columns");

                while (columnsProperty.arraySize != fieldSize.x)
                {
                    if (fieldSize.x > columnsProperty.arraySize)
                        columnsProperty.InsertArrayElementAtIndex(columnsProperty.arraySize);
                    else
                        columnsProperty.DeleteArrayElementAtIndex(fieldSize.x);
                }

                for (int x = 0; x < columnsProperty.arraySize; x++)
                {
                    SerializedProperty cellsProperty = columnsProperty.GetArrayElementAtIndex(x).FindPropertyRelative("_cells");

                    while (cellsProperty.arraySize != fieldSize.y)
                    {
                        if (fieldSize.y > cellsProperty.arraySize)
                        {
                            cellsProperty.InsertArrayElementAtIndex(cellsProperty.arraySize);
                            cellsProperty.GetArrayElementAtIndex(cellsProperty.arraySize - 1).boolValue = false;
                        }
                        else
                            cellsProperty.DeleteArrayElementAtIndex(fieldSize.y);
                    }
                }
            }

            private void DrawCombination(SerializedProperty combinationProperty, int index)
            {
                SerializedProperty columnsProperty = combinationProperty.FindPropertyRelative("_columns");

                EditorGUILayout.BeginVertical(GUI.skin.box);

                GUILayout.Space(5);

                EditorGUILayout.BeginHorizontal();

                GUILayout.Space(5);

                EditorGUILayout.BeginVertical();
                    
                EditorGUILayout.BeginHorizontal();

                GUILayout.FlexibleSpace();

                EditorGUILayout.BeginVertical(GUI.skin.box);

                GUILayout.Space(5);

                EditorGUILayout.BeginHorizontal();

                GUILayout.Space(5);

                for (int x = 0; x < columnsProperty.arraySize; x++)
                {
                    SerializedProperty cellsProperty = columnsProperty.GetArrayElementAtIndex(x).FindPropertyRelative("_cells");

                    EditorGUILayout.BeginVertical();

                    for (int y = 0; y < cellsProperty.arraySize; y++)
                    {
                        EditorGUILayout.BeginVertical(GUI.skin.box);

                        GUILayout.Space(2);

                        EditorGUILayout.BeginHorizontal();

                        GUILayout.Space(2);

                        Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Width(40), GUILayout.Height(40));

                        bool value = cellsProperty.GetArrayElementAtIndex(y).boolValue;

                        Color backgroundColor = value ? Color.green : Color.gray;

                        Texture2D texture = value ? _activeTexture : _disabledTexture;

                        if (GUI.Button(rect, GUIContent.none, new GUIStyle(GUI.skin.box) { normal = { background = texture } }))
                        {
                            for (int dy = 0; dy < cellsProperty.arraySize; dy++)
                                cellsProperty.GetArrayElementAtIndex(dy).boolValue = false;

                            value = !value;
                            cellsProperty.GetArrayElementAtIndex(y).boolValue = value;
                        }

                        GUILayout.Space(2);

                        EditorGUILayout.EndHorizontal();

                        GUILayout.Space(2);

                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.EndVertical();
                }

                GUILayout.Space(5);

                EditorGUILayout.EndHorizontal();

                GUILayout.Space(5);

                EditorGUILayout.EndVertical();

                GUILayout.FlexibleSpace();

                EditorGUILayout.EndHorizontal();

                GUILayout.Space(5f);

                RemoveButton(index);

                GUILayout.Space(5f);

                EditorGUILayout.EndVertical();

                GUILayout.Space(5);

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }

            private Texture2D MakeTex(int width, int height, Color color)
            {
                Color[] pix = new Color[width * height];
                for (int i = 0; i < pix.Length; ++i)
                {
                    pix[i] = color;
                }
                Texture2D result = new Texture2D(width, height);
                result.SetPixels(pix);
                result.Apply();
                return result;
            }

            private void RemoveButton(int index)
            {
                if (GUILayout.Button("Remove combination"))
                {
                    _combinationsProperty.DeleteArrayElementAtIndex(index);
                }
            }

            private void DrawHeader(string header)
            {
                GUIStyle centeredLabel = new GUIStyle(GUI.skin.label);
                centeredLabel.alignment = TextAnchor.MiddleCenter;
                centeredLabel.fontSize = 16;

                GUILayout.Label(header, centeredLabel);
            }
        }
    }
}