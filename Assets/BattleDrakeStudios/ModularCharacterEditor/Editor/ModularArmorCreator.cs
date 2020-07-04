using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BattleDrakeStudios.ModularCharacters;
using BattleDrakeStudios.Utilities;
using System;
using static BattleDrakeStudios.ModularCharacters.ModularCharacterStatics;
using BattleDrakeStudios.SimpleIconCreator;
using System.IO;

namespace BattleDrakeStudios.ModularCharacters {
    public class ModularArmorCreator : EditorWindow {
        private Vector2 windowPadding;
        private Vector2 armorScrollView;
        private Rect previewRect;
        private Rect bodyOptionsRect;
        private Rect armorOptionsRect;
        private Rect colorOptionsRect;
        private Rect armorNameRect;
        private Rect saveButtonRect;

        private GameObject previewPrefab;
        private CustomPreviewEditor previewEditor;
        private ModularCharacterManager characterManager;
        private Material previewMaterial;

        private IconCreatorWindow iconWindow;

        private ModularArmor existingArmor;

        private string armorName = "New Armor Item";
        private string assetPath = "Assets/BattleDrakeStudios/ModularCharacterEditor/ScriptableObjects/ModularArmor";

        private int armorTypeIndex;
        private ModularArmorType[] armorTypes;

        private List<BodyPartLinker> armorParts;
        private int[] activePartID;

        public ColorPropertyLinker[] armorColors = { new ColorPropertyLinker(COLOR_PRIMARY), new ColorPropertyLinker(COLOR_SECONDARY), new ColorPropertyLinker(COLOR_LEATHER_PRIMARY),
    new ColorPropertyLinker(COLOR_LEATHER_SECONDARY), new ColorPropertyLinker(COLOR_METAL_PRIMARY), new ColorPropertyLinker(COLOR_METAL_SECONDARY), new ColorPropertyLinker(COLOR_METAL_DARK)};

        private Gender currentGender;

        [MenuItem("BattleDrakeStudios/ModularCharacter/ArmorCreator")]
        public static void ShowWindow() {
            EditorWindow newWindow = GetWindow<ModularArmorCreator>("Modular Armor Creator");
            newWindow.minSize = new Vector2(600, 425);
            newWindow.maxSize = new Vector2(600, 425);
            newWindow.Show();
        }

        private void OnEnable() {
            EditorApplication.playModeStateChanged += ReloadPreviewObject;
            Undo.undoRedoPerformed += UndoPerformed;
            Initialize();
        }
        private void OnDisable() {
            EditorApplication.playModeStateChanged -= ReloadPreviewObject;
            Undo.undoRedoPerformed -= UndoPerformed;
        }

        private void ReloadPreviewObject(PlayModeStateChange stateChange) {
            if (stateChange == PlayModeStateChange.EnteredEditMode) {
                if (previewEditor.TargetObject != null) {
                    LoadPreviewObject(previewEditor.TargetObject);
                    Repaint();
                }
            }
        }

        private void UndoPerformed() {
            InitializeColors();
            Repaint();
        }

        private void Initialize() {
            SetupRectAreas();

            previewPrefab = Resources.Load<GameObject>("Pf_ArmorCreatorBase");

            currentGender = Gender.Male;

            armorTypes = (ModularArmorType[])System.Enum.GetValues(typeof(ModularArmorType));

            armorTypeIndex = 0;
            SetupParts();
        }

        private void SetupRectAreas() {
            windowPadding = new Vector2(4, 4);
            previewRect = new Rect(0, 0, 256, 256);
            bodyOptionsRect = new Rect(0, 256, 256, 125);
            armorOptionsRect = new Rect(256, 0, 340, 200);
            colorOptionsRect = new Rect(256, 200, 340, 150);
            armorNameRect = new Rect(256, 350, 340, 25);
            saveButtonRect = new Rect(0, 375, 597, 50);
        }

        private void SetupParts() {
            armorParts = GetArmorParts(armorTypes[armorTypeIndex]);

            activePartID = new int[armorParts.Count];
            for (int i = 0; i < activePartID.Length; i++) {
                if (armorParts[i].bodyType.IsBaseBodyPart()) {
                    activePartID[i] = 0;
                    armorParts[i].partID = 0;
                } else {
                    activePartID[i] = -1;
                    armorParts[i].partID = -1;
                }
            }
        }

        private void SetupPartsFromExisting() {
            if (existingArmor != null) {
                armorTypeIndex = (int)existingArmor.armorType;
                armorParts = GetArmorParts(armorTypes[armorTypeIndex]);
                activePartID = new int[armorParts.Count];
                for (int i = 0; i < activePartID.Length; i++) {
                    activePartID[i] = existingArmor.armorParts[i].partID;
                    armorParts[i].partID = activePartID[i];
                    if (armorParts[i].partID > -1)
                        characterManager.ActivatePart(armorParts[i].bodyType, armorParts[i].partID);
                }
            }
        }

        private void LoadPreviewObject(GameObject previewObject) {
            previewEditor.OnPreviewObjectInstantiated -= LoadPreviewObject;

            if (previewEditor.TargetObject != null) {
                characterManager = previewEditor.TargetObject.GetComponent<ModularCharacterManager>();
                if (characterManager != null) {
                    currentGender = characterManager.CharacterGender;
                    previewMaterial = new Material(characterManager.CharacterMaterial);
                    if (existingArmor != null) {
                        armorName = existingArmor.name;
                        SetMaterialColorsToExisting();
                        SetupPartsFromExisting();
                    }
                    characterManager.SetAllPartsMaterial(previewMaterial);
                    InitializeColors();
                }
            }
        }

        private void InitializeColors() {
            foreach (var armorColor in armorColors) {
                armorColor.color = previewMaterial.GetColor(armorColor.property);
            }
        }

        private void SetMaterialColorsToExisting() {
            foreach (var armorColor in existingArmor.armorColors) {
                previewMaterial.SetColor(armorColor.property, armorColor.color);
            }
        }

        private void OnGUI() {
            GUILayout.BeginArea(new Rect(previewRect.x + windowPadding.x, previewRect.y + windowPadding.y,
                previewRect.width - windowPadding.x, previewRect.height - windowPadding.y));

            if (previewPrefab != null) {
                if (previewEditor == null) {
                    previewEditor = Editor.CreateEditor(previewPrefab, typeof(CustomPreviewEditor)) as CustomPreviewEditor;
                    previewEditor.OnPreviewObjectInstantiated += LoadPreviewObject;
                    previewEditor.TargetAsset = previewPrefab;
                } else {
                    if (characterManager == null) {
                        LoadPreviewObject(previewEditor.TargetObject);
                    }
                }
                if (previewEditor.HasPreviewGUI()) {
                    previewEditor.OnInteractivePreviewGUI(new Rect(0, 0, previewRect.width - windowPadding.x, previewRect.height - windowPadding.y), null);
                }
            }
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(bodyOptionsRect.x + windowPadding.x, bodyOptionsRect.y + windowPadding.y,
                bodyOptionsRect.width - windowPadding.x, bodyOptionsRect.height - windowPadding.y));
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Male")) {
                characterManager.SwapGender(Gender.Male);
                currentGender = Gender.Male;
            }
            if (GUILayout.Button("Female")) {
                characterManager.SwapGender(Gender.Female);
                currentGender = Gender.Female;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("ShowBody")) {
                characterManager.ToggleBaseBodyDisplay(true);
            }
            if (GUILayout.Button("HideBody")) {
                characterManager.ToggleBaseBodyDisplay(false);
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Reset")) {
                ResetAll();
            }

            GUILayout.Label("Existing Modular Armor Asset");

            EditorGUI.BeginChangeCheck();
            existingArmor = EditorGUILayout.ObjectField(existingArmor, typeof(ModularArmor), false) as ModularArmor;
            if (EditorGUI.EndChangeCheck()) {
                if (existingArmor != null) {
                    armorName = existingArmor.name;
                    ResetParts();
                    SetupPartsFromExisting();
                    SetMaterialColorsToExisting();
                    characterManager.SetAllPartsMaterial(previewMaterial);
                    InitializeColors();
                } else {
                    ResetAll();
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(armorOptionsRect.x + windowPadding.x, armorOptionsRect.y + windowPadding.y,
                armorOptionsRect.width - windowPadding.x, armorOptionsRect.height - windowPadding.y));

            EditorGUI.BeginChangeCheck();
            armorTypeIndex = EditorGUILayout.Popup(armorTypeIndex, Array.ConvertAll<ModularArmorType, string>(armorTypes, x => x.ToString()), GUILayout.Width(340));
            if (EditorGUI.EndChangeCheck()) {
                ResetParts();
                armorParts = GetArmorParts(armorTypes[armorTypeIndex]);
                SetupParts();
            }

            GUILayout.BeginVertical();

            armorScrollView = GUILayout.BeginScrollView(armorScrollView);

            if (armorParts.Count > 0) {
                for (int i = 0; i < armorParts.Count; i++) {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(armorParts[i].bodyType.ToString());
                    EditorGUI.BeginChangeCheck();
                    int maxPartID = -1;
                    if (characterManager != null) {
                        maxPartID = characterManager.GetCharacterBody()[armorParts[i].bodyType].Length - 1;
                    }
                    activePartID[i] = EditorGUILayout.IntSlider(activePartID[i], -1, maxPartID);
                    if (EditorGUI.EndChangeCheck()) {
                        if (activePartID[i] > -1) {
                            characterManager.ActivatePart(armorParts[i].bodyType, activePartID[i]);
                        } else {
                            characterManager.DeactivatePart(armorParts[i].bodyType);
                        }
                        armorParts[i].partID = activePartID[i];
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();

            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(colorOptionsRect.x + windowPadding.x, colorOptionsRect.y + windowPadding.y,
                colorOptionsRect.width - windowPadding.x, colorOptionsRect.height - windowPadding.y));
            foreach (var armorColor in armorColors) {
                SetupColorFields(ref armorColor.color, armorColor.property, armorColor.property);
            }
            GUILayout.EndArea();

            GUILayout.EndVertical();

            GUILayout.BeginArea(new Rect(armorNameRect.x + windowPadding.x, armorNameRect.y + windowPadding.y,
                armorNameRect.width - windowPadding.x, armorNameRect.height - windowPadding.y));

            GUILayout.BeginHorizontal();
            GUILayout.Label("Armor Name: ", GUILayout.Width(75));
            armorName = GUILayout.TextField(armorName, GUILayout.Width(260));
            GUILayout.EndHorizontal();

            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(saveButtonRect.x + windowPadding.x, saveButtonRect.y + windowPadding.y,
                saveButtonRect.width - windowPadding.x, saveButtonRect.height - windowPadding.y));

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save as New")) {
                SaveDataToAsset(true);
            }
            if (GUILayout.Button("Overwrite Existing")) {
                SaveDataToAsset(false);
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Create Icon")) {
                iconWindow = EditorWindow.GetWindow<IconCreatorWindow>();
                iconWindow.Show();
                if (iconWindow.PreviewEditor != null) {
                    DestroyImmediate(iconWindow.PreviewEditor);
                }
                iconWindow.TargetObject = previewPrefab;

                iconWindow.OnPreviewCreated += SetupIconPreview;
            }

            GUILayout.EndArea();
        }

        private void SetupIconPreview(CustomPreviewEditor preview) {
            iconWindow.OnPreviewCreated -= SetupIconPreview;

            ModularCharacterManager iconCharacter = preview.TargetObject.GetComponent<ModularCharacterManager>();
            iconCharacter.SwapGender(currentGender);
            iconCharacter.ToggleBaseBodyDisplay(false);
            foreach (var part in armorParts) {
                if (part.partID > -1) {
                    iconCharacter.ActivatePart(part.bodyType, part.partID);
                    for (int i = 0; i < armorColors.Length; i++) {
                        iconCharacter.SetPartColor(part.bodyType, part.partID, armorColors[i].property, armorColors[i].color);
                    }
                } else
                    iconCharacter.DeactivatePart(part.bodyType);
            }
        }

        private void SaveDataToAsset(bool isNew) {
            ModularArmor newArmor = ScriptableObject.CreateInstance<ModularArmor>();

            newArmor.armorType = armorTypes[armorTypeIndex];

            foreach (var armorColor in newArmor.armorColors) {
                armorColor.color = previewMaterial.GetColor(armorColor.property);
            }

            newArmor.armorParts = armorParts.ToArray();
            for (int i = 0; i < newArmor.armorParts.Length; i++) {
                newArmor.armorParts[i].partID = activePartID[i];
            }

            if (isNew) {
                string newAssetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath + "/" + armorName + ".asset");
                AssetDatabase.CreateAsset(newArmor, newAssetPath);
            } else {
                if (existingArmor == null) {
                    Debug.LogWarning("Nothing to overwrite");
                    return;
                }
                if (armorName != existingArmor.name) {
                    string existingPath = AssetDatabase.GetAssetPath(existingArmor);
                    AssetDatabase.RenameAsset(existingPath, armorName);
                }
                EditorUtility.CopySerializedIfDifferent(newArmor, existingArmor);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void SetupColorFields(ref Color partColor, string label, string shaderProperty) {
            EditorGUI.BeginChangeCheck();
            partColor = EditorGUILayout.ColorField(label, partColor, GUILayout.Width(340));
            if (EditorGUI.EndChangeCheck()) {
                if (previewMaterial != null) {
                    Undo.RecordObject(previewMaterial, "Undo Color change");
                    previewMaterial.SetColor(shaderProperty, partColor);
                }

            }
        }

        private void ResetAll() {
            DestroyImmediate(previewEditor);
            existingArmor = null;
            Initialize();
        }

        private void ResetParts() {
            foreach (var part in armorParts) {
                if (part.bodyType.IsBaseBodyPart()) {
                    if (characterManager != null)
                        characterManager.ActivatePart(part.bodyType, 0);
                } else {
                    if (characterManager != null)
                        characterManager.DeactivatePart(part.bodyType);
                }
            }
        }

        private List<BodyPartLinker> GetArmorParts(ModularArmorType armorType) {
            List<BodyPartLinker> armorParts = new List<BodyPartLinker>();
            switch (armorType) {
                case ModularArmorType.Helmet:
                    armorParts.Add(new BodyPartLinker(ModularBodyPart.HeadAttachment));
                    armorParts.Add(new BodyPartLinker(ModularBodyPart.Helmet));
                    armorParts.Add(new BodyPartLinker(ModularBodyPart.HeadCovering));
                    armorParts.Add(new BodyPartLinker(ModularBodyPart.Hat));
                    armorParts.Add(new BodyPartLinker(ModularBodyPart.Mask));
                    break;
                case ModularArmorType.Shoulders:
                    armorParts.Add(new BodyPartLinker(ModularBodyPart.ShoulderAttachmentLeft));
                    armorParts.Add(new BodyPartLinker(ModularBodyPart.ShoulderAttachmentRight));
                    break;
                case ModularArmorType.Cloak:
                    armorParts.Add(new BodyPartLinker(ModularBodyPart.BackAttachment));
                    break;
                case ModularArmorType.Chest:
                    armorParts.Add(new BodyPartLinker(ModularBodyPart.Torso));
                    armorParts.Add(new BodyPartLinker(ModularBodyPart.ArmUpperLeft));
                    armorParts.Add(new BodyPartLinker(ModularBodyPart.ArmUpperRight));
                    break;
                case ModularArmorType.Gloves:
                    armorParts.Add(new BodyPartLinker(ModularBodyPart.ElbowAttachmentLeft));
                    armorParts.Add(new BodyPartLinker(ModularBodyPart.ElbowAttachmentRight));
                    armorParts.Add(new BodyPartLinker(ModularBodyPart.ArmLowerLeft));
                    armorParts.Add(new BodyPartLinker(ModularBodyPart.ArmLowerRight));
                    armorParts.Add(new BodyPartLinker(ModularBodyPart.HandLeft));
                    armorParts.Add(new BodyPartLinker(ModularBodyPart.HandRight));
                    break;
                case ModularArmorType.Legs:
                    armorParts.Add(new BodyPartLinker(ModularBodyPart.Hips));
                    armorParts.Add(new BodyPartLinker(ModularBodyPart.HipsAttachment));
                    break;
                case ModularArmorType.Boots:
                    armorParts.Add(new BodyPartLinker(ModularBodyPart.KneeAttachmentLeft));
                    armorParts.Add(new BodyPartLinker(ModularBodyPart.KneeAttachmentRight));
                    armorParts.Add(new BodyPartLinker(ModularBodyPart.LegLeft));
                    armorParts.Add(new BodyPartLinker(ModularBodyPart.LegRight));
                    break;
            }
            return armorParts;
        }
    }
}
