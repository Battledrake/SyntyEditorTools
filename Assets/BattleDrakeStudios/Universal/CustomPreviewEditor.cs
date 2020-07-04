using UnityEngine;
using UnityEditor;
using System;

namespace BattleDrakeStudios.Utilities {

    public class CustomPreviewEditor : Editor {
        private PreviewRenderUtility previewUtil;

        private Vector2 rotationDrag;
        private Vector2 zRotation;
        private Vector2 positionDrag;
        private float scrollDelta;

        private GameObject targetAsset;
        private GameObject targetObject;

        private Texture2D previewTexture;
        private FilterMode currentFilterMode = FilterMode.Bilinear;

        private Texture2D bgTexture;
        public Texture2D BGTexture { get => bgTexture; set { bgTexture = value; } }

        public GameObject TargetAsset { get => targetAsset; set { targetAsset = value; UpdatePreviewTarget(); } }
        public GameObject TargetObject => targetObject;
        public Texture2D PreviewTexture => previewTexture;

        public event Action<GameObject> OnPreviewObjectInstantiated;

        private void SetupPreviewRenderUtility() {
            if (previewUtil == null) {
                previewUtil = new PreviewRenderUtility(true, true);

                previewUtil.camera.transform.position = new Vector3(0, 0, -3);
                previewUtil.camera.transform.rotation = Quaternion.identity;
                previewUtil.cameraFieldOfView = 30.0f;
                //                 previewUtil.camera.clearFlags = CameraClearFlags.Color;
            }
        }

        private void UpdatePreviewTarget() {
            SetupPreviewRenderUtility();

            targetObject = previewUtil.InstantiatePrefabInScene(targetAsset);
            OnPreviewObjectInstantiated?.Invoke(targetObject);
            targetObject.transform.position = Vector3.zero;
            targetObject.transform.Rotate(new Vector3(0, 180, 0));
        }

        public override bool HasPreviewGUI() {
            return true;
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background) {
            if (targetObject == null) {
                UpdatePreviewTarget();
            }

            if (Event.current.button == 0) {
                rotationDrag = DragDelta(rotationDrag, r);
            } else if (Event.current.button == 1) {
                zRotation = DragDelta(zRotation, r);
            } else if (Event.current.button == 2) {
                positionDrag = DragDelta(positionDrag, r);
            }

            if (Event.current.type == EventType.ScrollWheel) {
                scrollDelta = Event.current.delta.y;
                scrollDelta /= Event.current.shift ? 1 : 3; //delta default value is 3/-3, we divide to make this value 1/-1 unless shift is held
                GUI.changed = true;
            }

            if (Event.current.type == EventType.Repaint) {
                if (previewUtil == null) {
                    SetupPreviewRenderUtility();
                    return;
                }

                previewUtil.BeginPreview(r, background);
                if (targetObject != null) {
                    targetObject.transform.position += new Vector3(-positionDrag.x, positionDrag.y, 0);

                    //I wanted a very specific style of object rotation, 2 different Rotate calls achieved it.
                    targetObject.transform.Rotate(new Vector3(0, rotationDrag.x * 180, 0), Space.Self);
                    targetObject.transform.Rotate(new Vector3(rotationDrag.y * 180, 0, zRotation.x * 180), Space.World);

                    positionDrag = Vector2.zero;
                    rotationDrag = Vector2.zero;
                    zRotation = Vector2.zero;
                }

                previewUtil.camera.transform.position = Vector2.zero;

                previewUtil.camera.transform.position = previewUtil.camera.transform.forward * -6f;

                previewUtil.cameraFieldOfView = previewUtil.cameraFieldOfView + scrollDelta;
                scrollDelta = 0;

                previewUtil.camera.targetTexture.filterMode = currentFilterMode;

                if (bgTexture != null)
                    GUI.DrawTexture(new Rect(r.x, r.y, r.width * 2, r.height * 2), bgTexture, ScaleMode.StretchToFill, true);

                previewUtil.camera.Render();

                previewTexture = previewUtil.EndStaticPreview();

                GUI.DrawTexture(r, previewTexture, ScaleMode.StretchToFill, true);
            }
        }

        public void SetBackgroundColor(Color bgColor, bool isTransparent = false) {
            if (previewUtil != null) {
                if (isTransparent) {
                    currentFilterMode = FilterMode.Point;
                } else {
                    currentFilterMode = FilterMode.Bilinear;
                }
                previewUtil.camera.backgroundColor = bgColor;
            }

        }

        private Vector2 DragDelta(Vector2 dragValue, Rect position) {
            int controlID = GUIUtility.GetControlID("Slider".GetHashCode(), FocusType.Passive);
            Event current = Event.current;
            switch (current.GetTypeForControl(controlID)) {
                case EventType.MouseDown:
                    if (position.Contains(current.mousePosition) && position.width > 50f) {
                        GUIUtility.hotControl = controlID;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(1);
                    }
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID) {
                        GUIUtility.hotControl = 0;
                    }
                    EditorGUIUtility.SetWantsMouseJumping(0);
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID) {
                        if (Mathf.Abs(current.delta.x) > Mathf.Abs(current.delta.y)) {
                            dragValue.x -= current.delta.x * (float)((!current.shift) ? 1 : 3) / Mathf.Min(position.width, position.height);
                        } else if (Mathf.Abs(current.delta.y) > Mathf.Abs(current.delta.x)) {
                            dragValue.y -= current.delta.y * (float)((!current.shift) ? 1 : 3) / Mathf.Min(position.width, position.height);
                        }
                        dragValue.y = Mathf.Clamp(dragValue.y, -90f, 90f);
                        current.Use();
                        GUI.changed = true;
                    }
                    break;
            }
            return dragValue;
        }

        private void OnDisable() {
            if (previewUtil != null)
                previewUtil.Cleanup();
        }
    }
}
