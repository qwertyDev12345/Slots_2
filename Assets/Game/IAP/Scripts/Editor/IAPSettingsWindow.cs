using UnityEditor;
using UnityEngine;

namespace IAP
{
    internal sealed class IAPSettingsWindow : EditorWindow
    {
        private static SerializedObject _serializedObject = null;

        private static EditorWindow _window;

        private static Vector2 _scrollPos;

        private bool _isInCompilation = false;

        [MenuItem("IAP/Settings", false, 0)]
        public static void Init()
        {
            _window = GetWindow<IAPSettingsWindow>("IAP Settings", typeof(SceneView));

            _window.name = "IAP Settings";

            if (IAPSettings.Data == null)
            {
                _window.Close();
            }
        }

        private void OnGUI()
        {
            if (!EditorApplication.isCompiling)
            {
                if (!_isInCompilation)
                {
                    DrawInterface();
                    
                    return;
                }

                if (Event.current.type == EventType.Repaint)
                {
                    _isInCompilation = false;

                    Init();

                    return;
                }

                return;
            }

            _isInCompilation = true;

            DrawWaiting();
        }

        private void DrawWaiting()
        {
            EditorGUILayout.BeginVertical("BOX");

            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("Waiting for compiling", new GUIStyle(EditorStyles.boldLabel) 
                { alignment = TextAnchor.MiddleCenter, fontSize = 15 });

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginVertical();

            float progress = (float)(EditorApplication.timeSinceStartup % 1.0);
            
            EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(), progress, "");

            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(10);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            EditorGUILayout.EndVertical();

            // Оновлення вікна, щоб прогрес-бар анімувався
            if (progress < 0.99f)
            {
                _window.Repaint();
            }
        }

        private void DrawInterface()
        {
            if (IAPSettings.Data == null)
            {
                _window.Close();
                
                return;
            }

            _serializedObject = new SerializedObject(IAPSettings.Data);
            
            _serializedObject.Update();

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            DrawHeader();
            
            DrawDemoIAPSettings();

            DrawIAPPanel();

            EditorGUILayout.EndScrollView();

            _serializedObject.ApplyModifiedProperties();
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginVertical("BOX");
            
            EditorGUILayout.Space(5);
            
            EditorGUILayout.LabelField("IAP Settings", new GUIStyle(EditorStyles.boldLabel) 
                { alignment = TextAnchor.MiddleCenter, fontSize = 20 }, GUILayout.Height(30));
            
            EditorGUILayout.Space(5);

            EditorGUILayout.EndVertical();
        }

        private void DrawDemoIAPSettings()
        {
            SerializedProperty usDemoIAPProperty = _serializedObject.FindProperty("_usDemoIAP");

            EditorGUILayout.BeginVertical("BOX");
            
            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("Demo IAP Settings", new GUIStyle(EditorStyles.boldLabel) 
                { alignment = TextAnchor.MiddleCenter, fontSize = 15 });
            
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginVertical();
            
            Color originalColor = GUI.backgroundColor;

            GUI.backgroundColor = usDemoIAPProperty.boolValue ? Color.green : Color.red;
            
            if (GUILayout.Button(usDemoIAPProperty.boolValue ? "Demo IAP: ON" : "Demo IAP: OFF", 
                    new GUIStyle(EditorStyles.miniButton) { fontSize = 15 }))
            {
                usDemoIAPProperty.boolValue = !usDemoIAPProperty.boolValue;
            }
            
            GUI.backgroundColor = originalColor;
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(10);

            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(10);

            EditorGUILayout.EndVertical();
        }
        
        private void DrawIAPPanel()
        {
            EditorGUILayout.BeginVertical("BOX");

            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("Products list", new GUIStyle(EditorStyles.boldLabel) 
                { alignment = TextAnchor.MiddleCenter, fontSize = 15 });
            
            EditorGUILayout.Space(10);

            SerializedProperty productsProperty = _serializedObject.FindProperty("_products");
            
            for (int i = 0; i < productsProperty.arraySize; i++)
            {
                SerializedProperty productProperty = productsProperty.GetArrayElementAtIndex(i);

                SerializedProperty nameProperty = productProperty.FindPropertyRelative("_name");
                SerializedProperty defaultPriceProperty = productProperty.FindPropertyRelative("_defaultPrice");
                SerializedProperty typeProperty = productProperty.FindPropertyRelative("_type");
                SerializedProperty androidIdProperty = productProperty.FindPropertyRelative("_androidId");
                SerializedProperty iosIdProperty = productProperty.FindPropertyRelative("_iosId");
                
                EditorGUILayout.BeginVertical("BOX");

                EditorGUILayout.Space(2);

                EditorGUILayout.BeginHorizontal();
                
                EditorGUILayout.Space(2);

                EditorGUILayout.BeginVertical();

                EditorGUILayout.Space(2);

                EditorGUILayout.LabelField("Product #" + (i + 1), new GUIStyle(EditorStyles.boldLabel) 
                    { alignment = TextAnchor.MiddleCenter, fontSize = 12 });

                EditorGUILayout.Space(2);

                nameProperty.stringValue = EditorGUILayout.TextField("Name", nameProperty.stringValue);
                defaultPriceProperty.stringValue = EditorGUILayout.TextField("Default price", defaultPriceProperty.stringValue);
                typeProperty.enumValueIndex = (int)(ProductType)EditorGUILayout.EnumPopup("Type", (ProductType)typeProperty.enumValueIndex);
                androidIdProperty.stringValue = EditorGUILayout.TextField("Android ID", androidIdProperty.stringValue);
                iosIdProperty.stringValue = EditorGUILayout.TextField("IOS ID", iosIdProperty.stringValue);

                EditorGUILayout.Space(5);

                if (GUILayout.Button("Remove"))
                {
                    productsProperty.DeleteArrayElementAtIndex(i);
                }

                EditorGUILayout.EndVertical();
                
                EditorGUILayout.Space(2);

                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.Space(5);

                EditorGUILayout.EndVertical();
                
                GUILayout.Space(5);
            }

            EditorGUILayout.BeginHorizontal();
                
            EditorGUILayout.Space(2);

            EditorGUILayout.BeginVertical();

            if (GUILayout.Button("Add"))
            {
                int newElementIndex = productsProperty.arraySize;
                
                productsProperty.InsertArrayElementAtIndex(newElementIndex);
                
                SerializedProperty productProperty = productsProperty.GetArrayElementAtIndex(newElementIndex);

                SerializedProperty nameProperty = productProperty.FindPropertyRelative("_name");
                SerializedProperty defaultPriceProperty = productProperty.FindPropertyRelative("_defaultPrice");
                SerializedProperty typeProperty = productProperty.FindPropertyRelative("_type");
                SerializedProperty androidIdProperty = productProperty.FindPropertyRelative("_androidId");
                SerializedProperty iosIdProperty = productProperty.FindPropertyRelative("_iosId");

                nameProperty.stringValue = "Product";
                defaultPriceProperty.stringValue = "0.99$";
                typeProperty.enumValueIndex = (int)ProductType.Consumable;
                androidIdProperty.stringValue = "android_product_id";
                iosIdProperty.stringValue = "com.ios.product.id";
            }

            EditorGUILayout.EndVertical();
                
            EditorGUILayout.Space(2);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            EditorGUILayout.EndVertical();
        }
    }
}