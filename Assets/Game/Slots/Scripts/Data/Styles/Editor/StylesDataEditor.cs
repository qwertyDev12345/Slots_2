using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Slots.Data.Styles
{
    [CustomEditor(typeof(StylesData))]
    public class StylesDataEditor : Editor
    {
        private SerializedProperty _groupsProperty;
        private SerializedProperty _setsProperty;

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

            _groupsProperty = serializedObject.FindProperty("_groups");
            _setsProperty = serializedObject.FindProperty("_sets");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            string[] groups = Enum.GetNames(typeof(StyleSetGroup));

            List<string> setsGroups = new List<string>();

            while (_groupsProperty.arraySize != _setsProperty.arraySize)
            {
                if (_groupsProperty.arraySize > _setsProperty.arraySize)
                    _setsProperty.InsertArrayElementAtIndex(_setsProperty.arraySize);
                else
                    _setsProperty.DeleteArrayElementAtIndex(_groupsProperty.arraySize);
            }

            for (int i = 0; i < _groupsProperty.arraySize; i++)
                setsGroups.Add(_groupsProperty.GetArrayElementAtIndex(i).stringValue);

            for (int i = 0; i < groups.Length; i++)
            {
                if (setsGroups.Count == i)
                {
                    int dstIndex = i;

                    setsGroups.Insert(dstIndex, groups[i]);

                    _groupsProperty.InsertArrayElementAtIndex(dstIndex);
                    _groupsProperty.GetArrayElementAtIndex(dstIndex).stringValue = groups[i];

                    _setsProperty.InsertArrayElementAtIndex(dstIndex);

                    continue;
                }

                if (setsGroups[i] != groups[i])
                {
                    if (setsGroups.Contains(groups[i]))
                    {
                        int srcIndex = setsGroups.IndexOf(groups[i]);
                        int dstIndex = i;

                        setsGroups.RemoveAt(srcIndex);
                        setsGroups.Insert(dstIndex, groups[i]);

                        _groupsProperty.MoveArrayElement(srcIndex, dstIndex);
                        _setsProperty.MoveArrayElement(srcIndex, dstIndex);
                    }
                    else
                    {
                        int dstIndex = i;

                        setsGroups.Insert(dstIndex, groups[i]);

                        _groupsProperty.InsertArrayElementAtIndex(dstIndex);
                        _groupsProperty.GetArrayElementAtIndex(dstIndex).stringValue = groups[i];

                        _setsProperty.InsertArrayElementAtIndex(dstIndex);
                    }
                }
            }

            while (_groupsProperty.arraySize > groups.Length)
            {
                _groupsProperty.DeleteArrayElementAtIndex(groups.Length);
                _setsProperty.DeleteArrayElementAtIndex(groups.Length);
            }

            GUILayout.Space(10);

            for (int i = 0; i < _setsProperty.arraySize; i++)
            {
                GUILayout.Space(5);

                EditorGUILayout.BeginHorizontal();

                SerializedProperty groupProperty = _groupsProperty.GetArrayElementAtIndex(i);
                SerializedProperty setProperty = _setsProperty.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginVertical(GUI.skin.box);

                GUILayout.Space(5);

                DrawHeader(groupProperty);

                GUILayout.Space(5);

                DrawStyleSetFor(setProperty);

                GUILayout.Space(5f);

                switch ((StyleSetType)setProperty.FindPropertyRelative("_type").enumValueIndex)
                {
                    case StyleSetType.Image:

                        DrawSpriteField(setProperty.FindPropertyRelative("_sprite_0"), "Main sprite");

                        break;

                    case StyleSetType.Text:

                        DrawFontProperty(setProperty.FindPropertyRelative("_font"));

                        GUILayout.Space(5);

                        DrawSizeProperty(setProperty.FindPropertyRelative("_value"));
                        
                        GUILayout.Space(5);

                        DrawColorProperty(setProperty.FindPropertyRelative("_color"));

                        GUILayout.Space(5);

                        break;

                    case StyleSetType.Button:

                        EditorGUILayout.BeginHorizontal();

                        GUILayout.Space(10);

                        GUILayout.FlexibleSpace();

                        DrawSpriteField(setProperty.FindPropertyRelative("_sprite_0"), "Enabled");

                        GUILayout.FlexibleSpace();

                        DrawSpriteField(setProperty.FindPropertyRelative("_sprite_1"), "Pressed");

                        GUILayout.FlexibleSpace();

                        DrawSpriteField(setProperty.FindPropertyRelative("_sprite_2"), "Disabled");

                        GUILayout.FlexibleSpace();

                        GUILayout.Space(10);

                        EditorGUILayout.EndHorizontal();

                        break;

                    case StyleSetType.Toggle:

                        EditorGUILayout.BeginHorizontal();

                        GUILayout.Space(10);

                        GUILayout.FlexibleSpace();

                        DrawSpriteField(setProperty.FindPropertyRelative("_sprite_0"), "Enabled");

                        GUILayout.FlexibleSpace();

                        DrawSpriteField(setProperty.FindPropertyRelative("_sprite_1"), "Pressed");

                        GUILayout.FlexibleSpace();

                        DrawSpriteField(setProperty.FindPropertyRelative("_sprite_2"), "Disabled");

                        GUILayout.FlexibleSpace();

                        DrawSpriteField(setProperty.FindPropertyRelative("_sprite_3"), "Active");

                        GUILayout.FlexibleSpace();

                        GUILayout.Space(10);

                        EditorGUILayout.EndHorizontal();

                        break;

                    case StyleSetType.LineRenderer:

                        EditorGUILayout.BeginHorizontal();

                        GUILayout.Space(10);

                        GUILayout.FlexibleSpace();

                        DrawSpriteField(setProperty.FindPropertyRelative("_sprite_0"), "Sprite");

                        GUILayout.FlexibleSpace();

                        GUILayout.Space(10);

                        EditorGUILayout.EndHorizontal();

                        GUILayout.Space(5);

                        DrawColorsProperty(setProperty.FindPropertyRelative("_colors"));

                        GUILayout.Space(5);

                        break;
                }

                GUILayout.Space(5f);

                EditorGUILayout.EndVertical();

                GUILayout.Space(15);

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(10);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHeader(SerializedProperty groupProperty)
        {
            string header = groupProperty.stringValue;

            header = header.Replace("_", " ");

            GUIStyle centeredLabel = new GUIStyle(GUI.skin.label);
            centeredLabel.alignment = TextAnchor.MiddleCenter;
            centeredLabel.fontSize = 16;
            GUILayout.Label(header, centeredLabel);
        }

        private void DrawStyleSetFor(SerializedProperty setProperty)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(10);

            SerializedProperty styleForProperty = setProperty.FindPropertyRelative("_type");

            EditorGUILayout.LabelField("Style set for", GUILayout.Width(100f));
            styleForProperty.enumValueIndex = (int)(StyleSetType)EditorGUILayout.EnumPopup((StyleSetType)styleForProperty.enumValueIndex, GUILayout.ExpandWidth(true));

            GUILayout.Space(10);

            EditorGUILayout.EndHorizontal();
        }

        private void DrawColorsProperty(SerializedProperty colorsProperty)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Space(5);

            GUIStyle centeredLabel = new GUIStyle(GUI.skin.label);
            centeredLabel.alignment = TextAnchor.MiddleCenter;
            centeredLabel.fontSize = 14;

            GUILayout.Label("Colors", centeredLabel);

            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.BeginVertical();

            for (int i = 0; i < colorsProperty.arraySize; i++)
            {
                SerializedProperty colorProperty = colorsProperty.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginChangeCheck();

                Color color = EditorGUILayout.ColorField(GUIContent.none, colorProperty.colorValue, GUILayout.ExpandWidth(true));

                if (EditorGUI.EndChangeCheck())
                {
                    colorProperty.colorValue = color;
                }

                GUILayout.Space(10);

                if (GUILayout.Button("Remove", GUILayout.Width(75)))
                {
                    colorsProperty.DeleteArrayElementAtIndex(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(5);

            if (GUILayout.Button("Add Color"))
            {
                colorsProperty.InsertArrayElementAtIndex(colorsProperty.arraySize);
                colorsProperty.GetArrayElementAtIndex(colorsProperty.arraySize - 1).colorValue = Color.white;
            }

            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);

            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            EditorGUILayout.EndHorizontal();
        }

        private void DrawSizeProperty(SerializedProperty sizeProperty)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(10);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Size", GUILayout.Width(100f));

            int size = EditorGUILayout.IntField(GUIContent.none, (int)sizeProperty.floatValue, GUILayout.ExpandWidth(true));

            if (EditorGUI.EndChangeCheck())
            {
                sizeProperty.floatValue = size;
            }

            GUILayout.Space(10);

            EditorGUILayout.EndHorizontal();
        }

        private void DrawColorProperty(SerializedProperty colorProperty)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(10);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Color", GUILayout.Width(100f));
            
            Color color = EditorGUILayout.ColorField(GUIContent.none, colorProperty.colorValue, GUILayout.ExpandWidth(true));

            if (EditorGUI.EndChangeCheck())
            {
                colorProperty.colorValue = color;
            }

            GUILayout.Space(10);

            EditorGUILayout.EndHorizontal();
        }

        private void DrawFontProperty(SerializedProperty fontProperty)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.LabelField("Font", GUILayout.Width(100f));

            fontProperty.objectReferenceValue = EditorGUILayout.ObjectField(fontProperty.objectReferenceValue, typeof(Font), false, GUILayout.ExpandWidth(true));

            if (fontProperty.objectReferenceValue != null)
            {
                GUIStyle fontPreviewStyle = new GUIStyle(GUI.skin.label);
                fontPreviewStyle.alignment = TextAnchor.UpperRight;
                fontPreviewStyle.font = (Font)fontProperty.objectReferenceValue;
                fontPreviewStyle.fontSize = 18;

                GUILayout.Label("Sample Text", fontPreviewStyle, GUILayout.Width(125f), GUILayout.Height(25f));
            }
            else
            {
                GUIStyle fontPreviewStyle = new GUIStyle(GUI.skin.label);
                fontPreviewStyle.alignment = TextAnchor.UpperRight;
                fontPreviewStyle.fontSize = 18;

                GUILayout.Label("No Font", fontPreviewStyle, GUILayout.Width(125f), GUILayout.Height(25f));
            }

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
