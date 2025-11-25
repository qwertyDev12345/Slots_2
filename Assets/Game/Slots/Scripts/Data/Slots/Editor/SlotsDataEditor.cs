using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Slots.Data.Slots
{
    [CustomEditor(typeof(SlotsData))]
    public class SlotsDataEditor : Editor
    {
        private SerializedProperty _slotsProperty;

        private Texture2D _transparentTexture = null;
        private SerializedProperty _propertyForSpriteSelecting = null;

        private void OnEnable()
        {
            Color[] colors = new Color[] { Color.clear, Color.clear, Color.clear, Color.clear };

            _transparentTexture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            _transparentTexture.filterMode = FilterMode.Point;
            _transparentTexture.wrapMode = TextureWrapMode.Repeat;
            _transparentTexture.alphaIsTransparency = true;
            _transparentTexture.SetPixels(colors);
            _transparentTexture.Apply();

            _slotsProperty = serializedObject.FindProperty("_slots");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.Space(10);

            int slotsCounter = 0;

            for (int i = 0; i < _slotsProperty.arraySize; i++)
            {
                GUILayout.Space(5);

                EditorGUILayout.BeginHorizontal();

                SerializedProperty slotProperty = _slotsProperty.GetArrayElementAtIndex(i);

                SlotType slotType = (SlotType)slotProperty.FindPropertyRelative("_type").enumValueIndex;

                EditorGUILayout.BeginVertical(GUI.skin.box);

                GUILayout.Space(5);

                if (slotType == SlotType.Standart)
                {
                    slotsCounter++;

                    DrawHeader("Slot " + slotsCounter);
                }
                else
                    DrawHeader(slotType.ToString());

                GUILayout.Space(5);

                DrawSlotType(slotProperty);

                GUILayout.Space(5f);

                DrawSpritesSelecting(slotProperty);

                GUILayout.Space(5f);

                DrawSpawnPercent(slotProperty);

                GUILayout.Space(5f);

                switch (slotType)
                {
                    case SlotType.Standart:
                    case SlotType.Scatter:
                        DrawLinePrizes(slotProperty);
                        break;

                    default:
                        break;
                }

                GUILayout.Space(5f);

                RemoveButton(i);

                GUILayout.Space(5f);

                EditorGUILayout.EndVertical();

                GUILayout.Space(15);

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(5f);

            AddButton();

            GUILayout.Space(10);

            serializedObject.ApplyModifiedProperties();
        }

        private void AddButton()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Add new slot"))
            {
                _slotsProperty.InsertArrayElementAtIndex(_slotsProperty.arraySize);
            }

            GUILayout.Space(15);

            EditorGUILayout.EndHorizontal();
        }

        private void RemoveButton(int index)
        {
            if (GUILayout.Button("Remove slot"))
            {
                _slotsProperty.DeleteArrayElementAtIndex(index);
            }
        }

        private void DrawHeader(string header)
        {
            GUIStyle centeredLabel = new GUIStyle(GUI.skin.label);
            centeredLabel.alignment = TextAnchor.MiddleCenter;
            centeredLabel.fontSize = 16;

            GUILayout.Label(header, centeredLabel);
        }

        private void DrawSlotType(SerializedProperty slotProperty)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(10);

            SerializedProperty styleForProperty = slotProperty.FindPropertyRelative("_type");

            EditorGUILayout.LabelField("Style type", GUILayout.Width(100f));
            styleForProperty.enumValueIndex = (int)(SlotType)EditorGUILayout.EnumPopup((SlotType)styleForProperty.enumValueIndex, GUILayout.ExpandWidth(true));

            GUILayout.Space(10);

            EditorGUILayout.EndHorizontal();
        }

        private void DrawSpawnPercent(SerializedProperty slotProperty)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(10);

            SerializedProperty spawnPercentProperty = slotProperty.FindPropertyRelative("_spawnPercent");

            EditorGUILayout.LabelField("Spawn percent", GUILayout.Width(100f));
            spawnPercentProperty.floatValue = EditorGUILayout.FloatField(spawnPercentProperty.floatValue, GUILayout.ExpandWidth(true));

            GUILayout.Space(10);

            EditorGUILayout.EndHorizontal();
        }

        private void DrawSpritesSelecting(SerializedProperty slotProperty)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(10);

            GUILayout.FlexibleSpace();

            DrawSpriteField(slotProperty.FindPropertyRelative("_slotSprite"), "Main sprite");

            GUILayout.FlexibleSpace();

            DrawSpriteField(slotProperty.FindPropertyRelative("_glowSprite"), "Glow");

            GUILayout.FlexibleSpace();

            GUILayout.Space(10);

            EditorGUILayout.EndHorizontal();
        }

        private void DrawLinePrizes(SerializedProperty slotProperty)
        {
            SerializedProperty inLineCountProperty = slotProperty.FindPropertyRelative("_inLineCount");
            SerializedProperty inLinePrizeProperty = slotProperty.FindPropertyRelative("_inLinePrize");

            while (inLineCountProperty.arraySize != inLinePrizeProperty.arraySize)
            {
                if (inLineCountProperty.arraySize > inLinePrizeProperty.arraySize)
                    inLinePrizeProperty.InsertArrayElementAtIndex(inLinePrizeProperty.arraySize);
                else
                    inLinePrizeProperty.DeleteArrayElementAtIndex(inLineCountProperty.arraySize);
            }

            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.BeginVertical(GUI.skin.box);

            GUIStyle centeredLabel = new GUIStyle(GUI.skin.label);
            centeredLabel.alignment = TextAnchor.MiddleCenter;
            centeredLabel.fontSize = 14;

            GUILayout.Label("Prizes", centeredLabel);

            GUILayout.Space(5);

            int count = inLineCountProperty.arraySize;

            for (int i = 0; i < count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                SerializedProperty countProperty = inLineCountProperty.GetArrayElementAtIndex(i);
                countProperty.intValue = EditorGUILayout.IntField(countProperty.intValue, GUILayout.ExpandWidth(true));

                SerializedProperty prizeProperty = inLinePrizeProperty.GetArrayElementAtIndex(i);
                prizeProperty.floatValue = EditorGUILayout.FloatField(prizeProperty.floatValue, GUILayout.ExpandWidth(true));

                if (GUILayout.Button("Remove", GUILayout.Width(100f)))
                {
                    inLineCountProperty.DeleteArrayElementAtIndex(i);
                    inLinePrizeProperty.DeleteArrayElementAtIndex(i);

                    count--;
                }

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(5);

            if (GUILayout.Button("Add"))
            {
                inLineCountProperty.InsertArrayElementAtIndex(count);
                inLinePrizeProperty.InsertArrayElementAtIndex(count);
                count++;
            }

            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            EditorGUILayout.EndHorizontal();
        }


        private void DrawSpriteField(SerializedProperty spriteProperty, string label)
        {
            Rect rect = GUILayoutUtility.GetRect(110, 130);

            GUIContent labelContent = new GUIContent(label);

            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.alignment = TextAnchor.MiddleCenter;

            Rect labelRect = new Rect(rect.x, rect.y, rect.width, 20);

            EditorGUI.LabelField(labelRect, labelContent, labelStyle);

            Rect previewRect = new Rect(rect.x + ((rect.width - 100) / 2), rect.y + 25, 100, rect.height - 30);

            EditorGUI.DrawTextureTransparent(previewRect, _transparentTexture, ScaleMode.StretchToFill);

            Texture2D texture = AssetPreview.GetAssetPreview(spriteProperty.objectReferenceValue);

            if (texture != null)
            {
                EditorGUI.DrawTextureTransparent(previewRect, texture, ScaleMode.ScaleToFit);
            }
            else
            {
                GUIContent previewTextContent = new GUIContent("Preview");

                GUIStyle previewTextStyle = new GUIStyle(GUI.skin.label);
                previewTextStyle.fontSize = 20;
                previewTextStyle.fontStyle = FontStyle.Bold;
                previewTextStyle.alignment = TextAnchor.MiddleCenter;

                Rect previewTextRect = new Rect(rect.x + ((rect.width - 100) / 2), rect.y + 90, 100, 30);

                EditorGUI.LabelField(previewTextRect, previewTextContent, previewTextStyle);
            }

            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:

                    if (!previewRect.Contains(Event.current.mousePosition))
                        break;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        foreach (Object draggedObject in DragAndDrop.objectReferences)
                        {
                            if (draggedObject.GetType() == typeof(Sprite))
                            {
                                spriteProperty.objectReferenceValue = draggedObject;
                                break;
                            }
                            else
                            {
                                string assetPath = AssetDatabase.GetAssetPath(draggedObject);

                                if (!string.IsNullOrEmpty(assetPath))
                                {
                                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);

                                    if (sprite != null)
                                    {
                                        spriteProperty.objectReferenceValue = sprite;

                                        break;
                                    }
                                }
                            }
                        }
                    }

                    Event.current.Use();

                    break;

                case EventType.MouseDown:

                    if (!previewRect.Contains(Event.current.mousePosition))
                        break;

                    _propertyForSpriteSelecting = spriteProperty;

                    EditorGUIUtility.ShowObjectPicker<Sprite>(null, false, null, EditorGUIUtility.GetControlID(FocusType.Passive));

                    Event.current.Use();

                    break;

                case EventType.ExecuteCommand:

                    if (Event.current.commandName != "ObjectSelectorUpdated")
                        break;

                    if (_propertyForSpriteSelecting == null)
                        break;

                    if (_propertyForSpriteSelecting.propertyPath != spriteProperty.propertyPath)
                        break;

                    spriteProperty.objectReferenceValue = EditorGUIUtility.GetObjectPickerObject();

                    break;
            }
        }
    }
}
