using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using BattleDrakeStudios.Utilities;

namespace BattleDrakeStudios.SimpleIconCreator {

    public class IconCreatorWindow : EditorWindow {

        private CustomPreviewEditor previewEditor;
        private GameObject targetObject;

        private Rect previewRectangle;

        private bool isTransparent;

        private Texture2D bgTexture;

        private Color iconBackgroundColor = Color.black;

        private string iconName = "New Icon";

        private int previewResIndex = 3;
        private int[] previewResolutions = { 32, 64, 128, 256, 512, 1024 };

        public GameObject TargetObject { get { return targetObject; } set { targetObject = value; } }
        public CustomPreviewEditor PreviewEditor => previewEditor;

        public event Action<CustomPreviewEditor> OnPreviewCreated;

        [MenuItem("BattleDrakeStudios/SimpleIconCreator")]
        public static void ShowWindow() {
            IconCreatorWindow editorWindow = EditorWindow.GetWindow<IconCreatorWindow>("Simple Icon Creator");

            editorWindow.Show();
        }



        private void OnGUI() {
            if (targetObject != null) {
                if (previewEditor == null) {
                    previewEditor = Editor.CreateEditor(targetObject, typeof(CustomPreviewEditor)) as CustomPreviewEditor;
                    previewEditor.TargetAsset = targetObject;
                    OnPreviewCreated?.Invoke(previewEditor);
                }
                if (previewEditor.HasPreviewGUI()) {
                    previewEditor.OnInteractivePreviewGUI(new Rect(0, 0, previewResolutions[previewResIndex], previewResolutions[previewResIndex]), null);
                }
            }

            GUILayout.BeginArea(new Rect(previewResolutions[previewResIndex], 0, 200, 200));

            GUILayout.BeginVertical();

            GUILayout.Label("Gameobject to create icon from");

            EditorGUI.BeginChangeCheck();
            targetObject = EditorGUILayout.ObjectField(targetObject, typeof(GameObject), false) as GameObject;
            if (EditorGUI.EndChangeCheck()) {
                DestroyImmediate(previewEditor);
                if (targetObject != null)
                    iconName = targetObject.name;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Icon Resolution");

            EditorGUI.BeginChangeCheck();
            previewResIndex = EditorGUILayout.Popup(previewResIndex, Array.ConvertAll<int, string>(previewResolutions, x => x.ToString()));
            if (EditorGUI.EndChangeCheck()) {
                GUI.changed = true;
            }
            GUILayout.EndHorizontal();

            if (targetObject != null) {
                EditorGUI.BeginChangeCheck();
                isTransparent = GUILayout.Toggle(isTransparent, "isTransparent", GUI.skin.button);
                if (EditorGUI.EndChangeCheck()) {
                    if (isTransparent) {
                        previewEditor.SetBackgroundColor(Color.magenta, true);
                    } else {
                        previewEditor.SetBackgroundColor(iconBackgroundColor);
                    }
                }

                EditorGUI.BeginChangeCheck();
                bgTexture = EditorGUILayout.ObjectField(bgTexture, typeof(Texture2D), false) as Texture2D;
                if (EditorGUI.EndChangeCheck()) {
                    if (bgTexture != null)
                        previewEditor.BGTexture = bgTexture;
                    else {
                        previewEditor.BGTexture = null;
                    }
                }


                if (!isTransparent) {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Background Color");
                    EditorGUI.BeginChangeCheck();
                    iconBackgroundColor = EditorGUILayout.ColorField(iconBackgroundColor);
                    if (EditorGUI.EndChangeCheck()) {
                        previewEditor.SetBackgroundColor(iconBackgroundColor);
                    }
                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label("Icon name");
                iconName = GUILayout.TextField(iconName);
                GUILayout.EndHorizontal();

                if (GUILayout.Button("Create Icon")) {
                    if (previewEditor.PreviewTexture != null) {
                        CreatePngFromTexture();
                    }
                }
            }

            GUILayout.EndVertical();

            GUILayout.EndArea();
        }

        private void CreatePngFromTexture() {
            Texture2D textureToConvert = previewEditor.PreviewTexture;

            if (isTransparent) {
                textureToConvert = CreateTextureWithTransparency(textureToConvert);
            }

            byte[] byteTexture = textureToConvert.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/BattleDrakeStudios/SimpleIconCreator/Icons/" + iconName + ".png", byteTexture);

            AssetDatabase.Refresh();
        }

        private Texture2D CreateTextureWithTransparency(Texture2D texture) {
            Texture2D newTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);

            Color[] pixels = texture.GetPixels(0, 0, texture.width, texture.height);

            for (int i = 0; i < pixels.Length; i++) {
                if (pixels[i] == Color.magenta) {
                    pixels[i] = Color.clear;
                }
            }
            newTexture.SetPixels(0, 0, texture.width, texture.height, pixels);
            newTexture.Apply();

            return newTexture;
        }
    }
}

