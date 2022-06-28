using UnityEditor;
using UnityEngine;

namespace ActionCode.Persistence.Editor
{
    [CustomEditor(typeof(PersistenceSettings))]
    public sealed class PersistenceSettingsEditor : UnityEditor.Editor
    {
        private PersistenceSettings persistenceSettings;

        private void OnEnable()
        {
            persistenceSettings = (PersistenceSettings)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            if (GUILayout.Button("Open Save Folder")) FileSystem.OpenSaveFolder();

            var showXmlWarning = persistenceSettings.serializer == SerializerType.Xml;
            if (showXmlWarning)
            {
                EditorGUILayout.HelpBox(
                    "Your Serializer class must have an empty constructor in order to work with XML Serializer.",
                    MessageType.Warning
                );
            }

            var showCryptographerKeyButton = persistenceSettings.cryptographer != CryptographerType.None;
            if (showCryptographerKeyButton)
            {
                if (GUILayout.Button("Get New Cryptographer Key"))
                {
                    var isAES = persistenceSettings.cryptographer == CryptographerType.AES;
                    var url = isAES ?
                        "https://randomkeygen.com/#txt_ci_key_0" :
                        "https://randomkeygen.com/";
                    Application.OpenURL(url);
                }
            }
        }
    }
}