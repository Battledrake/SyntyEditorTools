using UnityEngine;
using UnityEditor;
using System;

namespace BattleDrakeStudios.Utilities {

    public class CustomPreviewEditor : Editor {
        private PreviewRenderUtility _previewUtil;

        private Vector2 _rotationDrag;
        private Vector2 _zRotation;
        private Vector2 _posDrag;
        private float _scrollDelta;

        private GameObject _targetAsset;
        private GameObject _targetObject;

        private Texture2D _previewTexture;
        private FilterMode _currentFilterMode = FilterMode.Bilinear;

        private float _lightOneIntensity;
        private float _lightTwoIntensity;
        private Vector3 _lightOneRotation;
        private Vector3 _lightTwoRotation;

        private Texture2D _bgTexture;
        private Texture2D _fgTexture;

        public GameObject TargetAsset { get => _targetAsset; set { _targetAsset = value; UpdatePreviewTarget(); } }
        public GameObject TargetObject => _targetObject;
        public Texture2D PreviewTexture => _previewTexture;

        public float LightOneIntensity { get => _lightOneIntensity; set { _previewUtil.lights[0].intensity = value; } }
        public float LightTwoIntensity { get => _lightTwoIntensity; set { _previewUtil.lights[1].intensity = value; } }
        public Color LightOneColor { get => _previewUtil.lights[0].color; set { _previewUtil.lights[0].color = value; } }
        public Color LightTwoColor { get => _previewUtil.lights[1].color; set { _previewUtil.lights[1].color = value; } }
        public Vector3 LightOneRotation => _lightOneRotation;
        public Vector3 LightTwoRotation => _lightTwoRotation;

        public Color BGColor => _previewUtil.camera.backgroundColor;
        public Texture2D BGTexture { get => _bgTexture; set { _bgTexture = value; } }
        public Texture2D FGTExture { get => _fgTexture; set { _fgTexture = value; } }

        public event Action<GameObject> OnPreviewObjectInstantiated;

        private void SetupPreviewRenderUtility() {
            if (_previewUtil == null) {
                _previewUtil = new PreviewRenderUtility(true, true);

                _previewUtil.camera.transform.position = new Vector3(0, 0, -3);
                _previewUtil.camera.transform.rotation = Quaternion.identity;
                _previewUtil.cameraFieldOfView = 30.0f;
            }
        }

        private void UpdatePreviewTarget() {
            SetupPreviewRenderUtility();

            _targetObject = _previewUtil.InstantiatePrefabInScene(_targetAsset);
            OnPreviewObjectInstantiated?.Invoke(_targetObject);
            _targetObject.transform.position = Vector3.zero;
            _targetObject.transform.Rotate(new Vector3(0, 180, 0));
        }

        public override bool HasPreviewGUI() {
            return true;
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background) {
            if (_targetObject == null) {
                UpdatePreviewTarget();
            }

            if (Event.current.button == 0) {
                _rotationDrag = DragDelta(_rotationDrag, r);
            } else if (Event.current.button == 1) {
                _zRotation = DragDelta(_zRotation, r);
            } else if (Event.current.button == 2) {
                _posDrag = PositionDelta(_posDrag, r);
            }

            if (Event.current.type == EventType.ScrollWheel) {
                _scrollDelta = Event.current.delta.y;
                _scrollDelta /= Event.current.shift ? 9 : 1;
                GUI.changed = true;
            }

            if (Event.current.type == EventType.Repaint) {
                if (_previewUtil == null) {
                    SetupPreviewRenderUtility();
                    return;
                }

                _previewUtil.BeginPreview(r, background);
                if (_targetObject != null) {
                    _targetObject.transform.position += new Vector3(-_posDrag.x, _posDrag.y, 0);

                    //I wanted a very specific style of object rotation, 2 different Rotate calls achieved it.
                    _targetObject.transform.Rotate(new Vector3(0, _rotationDrag.x * 180, 0), Space.Self);
                    _targetObject.transform.Rotate(new Vector3(_rotationDrag.y * 180, 0, _zRotation.x * 180), Space.World);

                    _posDrag = Vector2.zero;
                    _rotationDrag = Vector2.zero;
                    _zRotation = Vector2.zero;
                }

                _previewUtil.camera.transform.position = Vector2.zero;

                _previewUtil.camera.transform.position = _previewUtil.camera.transform.forward * -6f;

                _previewUtil.cameraFieldOfView = _previewUtil.cameraFieldOfView + _scrollDelta;
                _scrollDelta = 0;

                _previewUtil.camera.targetTexture.filterMode = _currentFilterMode;

                if (_bgTexture != null)
                    GUI.DrawTexture(new Rect(0, 0, r.width * 2, r.height * 2), _bgTexture, ScaleMode.StretchToFill, true);

                _previewUtil.camera.Render();

                if (_fgTexture != null)
                    GUI.DrawTexture(new Rect(0, 0, r.width * 2, r.height * 2), _fgTexture, ScaleMode.StretchToFill, true);

                _previewTexture = _previewUtil.EndStaticPreview();

                GUI.DrawTexture(r, _previewTexture, ScaleMode.StretchToFill, true);
            }
        }

        public void ResetTargetObject() {
            _targetObject.transform.position = Vector3.zero;
            _targetObject.transform.rotation = Quaternion.identity;
        }

        public void SetBackgroundColor(Color bgColor, bool isTransparent = false) {
            if (_previewUtil != null) {
                if (isTransparent) {
                    _currentFilterMode = FilterMode.Point;
                } else {
                    _currentFilterMode = FilterMode.Bilinear;
                }
                _previewUtil.camera.backgroundColor = bgColor;
            }

        }

        private Vector2 DragDelta(Vector2 dragValue, Rect position) {
            int controlID = GUIUtility.GetControlID("Slider".GetHashCode(), FocusType.Passive);
            Event current = Event.current;
            switch (current.GetTypeForControl(controlID)) {
                case EventType.MouseDown:
                    if (position.Contains(current.mousePosition)/* && position.width > 50f*/) {
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

        private Vector2 PositionDelta(Vector2 dragValue, Rect position) {
            int controlID = GUIUtility.GetControlID("Slider".GetHashCode(), FocusType.Passive);
            Event current = Event.current;
            switch (current.GetTypeForControl(controlID)) {
                case EventType.MouseDown:
                    if (position.Contains(current.mousePosition)) {
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
                        dragValue -= new Vector2(current.delta.x, current.delta.y) * (float)((!current.shift) ? 1 : 3) / Mathf.Min(position.width, position.height);
                        current.Use();
                        GUI.changed = true;
                    }
                    break;
            }
            return dragValue;
        }

        private void OnDisable() {
            if (_previewUtil != null)
                _previewUtil.Cleanup();
        }
    }
}
