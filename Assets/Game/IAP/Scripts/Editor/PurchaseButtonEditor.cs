using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UI;

namespace IAP
{
    [CustomEditor(typeof(PurchaseButton))]
    internal sealed class PurchaseButtonEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space(5);

            SerializedProperty productName = serializedObject.FindProperty("_productName");
            SerializedProperty priceText = serializedObject.FindProperty("_priceText");

            List<string> products = new List<string>();

            products.AddRange(IAPSettings.Data.GetAllProductInfos().Select(p => p.Name));

            int currentIndex = products.IndexOf(productName.stringValue);

            if (currentIndex < 0 && !string.IsNullOrWhiteSpace(productName.stringValue))
            {
                products.Insert(0, productName.stringValue);
                currentIndex = 0;
            }
            else if (currentIndex < 0 && string.IsNullOrWhiteSpace(productName.stringValue))
            {
                products.Insert(0, "None");
                currentIndex = 0;
            }

            if (products.Count == 0)
            {
                currentIndex = 0;
                products = new List<string> { "None" };
            }

            currentIndex = EditorGUILayout.Popup("Product name:", currentIndex, products.ToArray());

            EditorGUILayout.Space(3);

            priceText.objectReferenceValue = (Text)EditorGUILayout.ObjectField("Price text:", (Text)priceText.objectReferenceValue, typeof(Text), true);

            productName.stringValue = products[currentIndex];

            EditorGUILayout.Space(10);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_onSucces"));

            EditorGUILayout.Space(5);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_onFailed"));

            EditorGUILayout.Space(5);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
