using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

namespace QuestDialogueSystem
{
    [CustomEditor(typeof(ConversationScript))]
    public class ConversationScriptEditor : Editor
    {
        ReorderableList pieceList;

        void OnEnable()
        {
            pieceList = new ReorderableList(serializedObject, serializedObject.FindProperty("pieces"), true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "Conversation Pieces");
                },

                elementHeightCallback = index =>
                {
                    var element = pieceList.serializedProperty.GetArrayElementAtIndex(index);
                    var options = element.FindPropertyRelative("options");

                    int optionCount = (options == null) ? 1 : Mathf.Max(1, options.arraySize);

                    float height = EditorGUIUtility.singleLineHeight * (optionCount + 20 + 5);
                    return height;
                },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var element = pieceList.serializedProperty.GetArrayElementAtIndex(index);
                    float x = rect.x;
                    float y = rect.y + 2;
                    float width  = rect.width;

                    float spacing = EditorGUIUtility.singleLineHeight + 2;

                    var idProp = element.FindPropertyRelative("id");
                    EditorGUI.PropertyField(
                        new Rect(x, y, width, spacing),
                        idProp, new GUIContent("ID"));
                        

                    EditorGUI.PropertyField(
                        new Rect(x, y + spacing, width, spacing * 2),
                        element.FindPropertyRelative("text"), new GUIContent("Text"));

                    EditorGUI.PropertyField(
                        new Rect(x, y + spacing * 3, width, spacing),
                        element.FindPropertyRelative("portrait"), new GUIContent("Portrait"));
                        
                    EditorGUI.PropertyField(
                        new Rect(x, y + spacing * 4, width, spacing),
                        element.FindPropertyRelative("portraitDescription"), new GUIContent("Portrait Description"));

                    EditorGUI.PropertyField(
                        new Rect(x, y + spacing * 5, width, spacing),
                        element.FindPropertyRelative("audio"), new GUIContent("Audio"));

                    EditorGUI.PropertyField(
                        new Rect(x, y + spacing * 6, width, spacing * Mathf.Max(1, element.FindPropertyRelative("options").arraySize)),
                        element.FindPropertyRelative("options"), new GUIContent("Options"), true);
                }
            };
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            pieceList.serializedProperty = serializedObject.FindProperty("pieces");
            pieceList.DoLayoutList();

            if(GUI.changed) serializedObject.ApplyModifiedProperties();
        }
    }
}