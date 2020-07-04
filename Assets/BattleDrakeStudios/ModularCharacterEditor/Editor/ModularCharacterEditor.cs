using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BattleDrakeStudios.ModularCharacters {
    public class ModularCharacterEditor : EditorWindow {
        private ModularCharacterManager characterManager;
        private Dictionary<ModularBodyPart, GameObject[]> characterBody;
        private Material characterMaterial;

        private Color primaryColor;
        private Color secondaryColor;
        private Color leatherPrimaryColor;
        private Color leatherSecondaryColor;
        private Color metalPrimaryColor;
        private Color metalSecondaryColor;
        private Color metalDarkColor;
        private Color hairColor;
        private Color skinColor;
        private Color stubbleColor;
        private Color scarColor;
        private Color bodyArtColor;
        private Color eyeColor;

        private string primaryProperty = "_Color_Primary";
        private string secondaryProperty = "_Color_Secondary";
        private string leatherPrimaryProperty = "_Color_Leather_Primary";
        private string leatherSecondaryProperty = "_Color_Leather_Secondary";
        private string metalPrimaryProperty = "_Color_Metal_Primary";
        private string metalSecondaryProperty = "_Color_Metal_Secondary";
        private string metalDarkProperty = "_Color_Metal_Dark";
        private string hairProperty = "_Color_Hair";
        private string skinProperty = "_Color_Skin";
        private string stubbleProperty = "_Color_Stubble";
        private string scarProperty = "_Color_Scar";
        private string bodyArtProperty = "_Color_BodyArt";
        private string eyesProperty = "_Color_Eyes";

        private int helmetCurrentIndex;
        private int helmetMaxIndex;

        private int headAttachmentCurrentIndex;
        private int headAttachmentMaxIndex;

        private int headCurrentIndex;
        private int headMaxIndex;

        private int hatCurrentIndex;
        private int hatMaxIndex;

        private int maskCurrentIndex;
        private int maskMaxIndex;

        private int headCoveringCurrentIndex;
        private int headCoveringMaxIndex;

        private int hairCurrentIndex;
        private int hairMaxIndex;

        private int eyebrowCurrentIndex;
        private int eyebrowMaxIndex;

        private int earCurrentIndex;
        private int earMaxIndex;

        private int facialHairCurrentIndex;
        private int facialHairMaxIndex;

        private int backAttachmentCurrentIndex;
        private int backAttachmentMaxIndex;

        private int torsoCurrentIndex;
        private int torsoMaxIndex;

        private int shoulderAttachmentRightCurrentIndex;
        private int shoulderAttachmentRightMaxIndex;

        private int shoulderAttachmentLeftCurrentIndex;
        private int shoulderAttachmentLeftMaxIndex;

        private int armUpperRightCurrentIndex;
        private int armUpperRightMaxIndex;

        private int armUpperLeftCurrentIndex;
        private int armUpperLeftMaxIndex;

        private int elbowAttachmentRightCurrentIndex;
        private int elbowAttachmentRightMaxIndex;

        private int elbowAttachmentLeftCurrentIndex;
        private int elbowAttachmentLeftMaxIndex;

        private int armLowerRightCurrentIndex;
        private int armLowerRightMaxIndex;

        private int armLowerLeftCurrentIndex;
        private int armLowerLeftMaxIndex;

        private int handRightCurrentIndex;
        private int handRightMaxIndex;

        private int handLeftCurrentIndex;
        private int handLeftMaxIndex;

        private int hipsAttachmentCurrentIndex;
        private int hipsAttachmentMaxIndex;

        private int hipsCurrentIndex;
        private int hipsMaxIndex;

        private int kneeAttachmentRightCurrentIndex;
        private int kneeAttachmentRightMaxIndex;

        private int kneeAttachmentLeftCurrentIndex;
        private int kneeAttachmentLeftMaxIndex;

        private int legRightCurrentIndex;
        private int legRightMaxIndex;

        private int legLeftCurrentIndex;
        private int legLeftMaxIndex;

        private Vector2 scrollPos;
        private bool selectionInitialized;

        [MenuItem("BattleDrakeStudios/ModularCharacter/CharacterEditor")]
        public static void ShowWindow() {
            ModularCharacterEditor editorWindow = EditorWindow.GetWindow<ModularCharacterEditor>();
            editorWindow.titleContent = new GUIContent("Modular Character Editor");
            editorWindow.Show();
        }

        private void OnEnable() {
            Undo.undoRedoPerformed += UndoPerformed;
        }
        private void OnDisable() {
            Undo.undoRedoPerformed -= UndoPerformed;
        }

        private void UndoPerformed() {
            if (Selection.activeGameObject) {
                characterManager = Selection.activeGameObject.GetComponent<ModularCharacterManager>();
                if (characterManager != null) {
                    if (characterManager.IsInitialized) {
                        InitializeColors();
                    }
                }
            }
        }

        private void OnSelectionChange() {
            selectionInitialized = false;

            if (Selection.activeGameObject != null) {
                characterManager = Selection.activeGameObject.GetComponent<ModularCharacterManager>();
                if (characterManager != null) {
                    if (characterManager.IsInitialized) {
                        InitializeBodyParts();
                        InitializeColors();
                    }

                }
            }
            Repaint();
        }

        private void OnInspectorUpdate() {
            if (Selection.activeGameObject != null) {
                if (characterManager == null) {
                    characterManager = Selection.activeGameObject.GetComponent<ModularCharacterManager>();

                }
                if (characterManager != null)
                    if (characterManager.gameObject == Selection.activeGameObject) {
                        if (characterManager.IsInitialized) {
                            if (!selectionInitialized) {
                                InitializeBodyParts();
                                InitializeColors();
                            }
                        }
                    } else {
                        selectionInitialized = false;
                        characterManager = Selection.activeGameObject.GetComponent<ModularCharacterManager>();
                    }

                Repaint();
            }
        }

        private void InitializeBodyParts() {
            characterBody = characterManager.GetCharacterBody();

            SetupBodyPart(ModularBodyPart.Helmet, ref helmetCurrentIndex, ref helmetMaxIndex);
            SetupBodyPart(ModularBodyPart.HeadAttachment, ref headAttachmentCurrentIndex, ref headAttachmentMaxIndex);
            SetupBodyPart(ModularBodyPart.Head, ref headCurrentIndex, ref headMaxIndex);
            SetupBodyPart(ModularBodyPart.Hat, ref hatCurrentIndex, ref hatMaxIndex);
            SetupBodyPart(ModularBodyPart.Mask, ref maskCurrentIndex, ref maskMaxIndex);
            SetupBodyPart(ModularBodyPart.HeadCovering, ref headCoveringCurrentIndex, ref headCoveringMaxIndex);
            SetupBodyPart(ModularBodyPart.Hair, ref hairCurrentIndex, ref hairMaxIndex);
            SetupBodyPart(ModularBodyPart.Eyebrow, ref eyebrowCurrentIndex, ref eyebrowMaxIndex);
            SetupBodyPart(ModularBodyPart.Ear, ref earCurrentIndex, ref earMaxIndex);
            SetupBodyPart(ModularBodyPart.FacialHair, ref facialHairCurrentIndex, ref facialHairMaxIndex);
            SetupBodyPart(ModularBodyPart.BackAttachment, ref backAttachmentCurrentIndex, ref backAttachmentMaxIndex);
            SetupBodyPart(ModularBodyPart.Torso, ref torsoCurrentIndex, ref torsoMaxIndex);
            SetupBodyPart(ModularBodyPart.ShoulderAttachmentRight, ref shoulderAttachmentRightCurrentIndex, ref shoulderAttachmentRightMaxIndex);
            SetupBodyPart(ModularBodyPart.ShoulderAttachmentLeft, ref shoulderAttachmentLeftCurrentIndex, ref shoulderAttachmentLeftMaxIndex);
            SetupBodyPart(ModularBodyPart.ArmUpperRight, ref armUpperRightCurrentIndex, ref armUpperRightMaxIndex);
            SetupBodyPart(ModularBodyPart.ArmUpperLeft, ref armUpperLeftCurrentIndex, ref armUpperLeftMaxIndex);
            SetupBodyPart(ModularBodyPart.ElbowAttachmentRight, ref elbowAttachmentRightCurrentIndex, ref elbowAttachmentRightMaxIndex);
            SetupBodyPart(ModularBodyPart.ElbowAttachmentLeft, ref elbowAttachmentLeftCurrentIndex, ref elbowAttachmentLeftMaxIndex);
            SetupBodyPart(ModularBodyPart.ArmLowerRight, ref armLowerRightCurrentIndex, ref armLowerRightMaxIndex);
            SetupBodyPart(ModularBodyPart.ArmLowerLeft, ref armLowerLeftCurrentIndex, ref armLowerLeftMaxIndex);
            SetupBodyPart(ModularBodyPart.HandRight, ref handRightCurrentIndex, ref handRightMaxIndex);
            SetupBodyPart(ModularBodyPart.HandLeft, ref handLeftCurrentIndex, ref handLeftMaxIndex);
            SetupBodyPart(ModularBodyPart.HipsAttachment, ref hipsAttachmentCurrentIndex, ref hipsAttachmentMaxIndex);
            SetupBodyPart(ModularBodyPart.Hips, ref hipsCurrentIndex, ref hipsMaxIndex);
            SetupBodyPart(ModularBodyPart.KneeAttachmentRight, ref kneeAttachmentRightCurrentIndex, ref kneeAttachmentRightMaxIndex);
            SetupBodyPart(ModularBodyPart.KneeAttachmentLeft, ref kneeAttachmentLeftCurrentIndex, ref kneeAttachmentLeftMaxIndex);
            SetupBodyPart(ModularBodyPart.LegRight, ref legRightCurrentIndex, ref legRightMaxIndex);
            SetupBodyPart(ModularBodyPart.LegLeft, ref legLeftCurrentIndex, ref legLeftMaxIndex);

            selectionInitialized = true;
        }

        private void InitializeColors() {
            characterMaterial = characterManager.CharacterMaterial;

            primaryColor = characterMaterial.GetColor(primaryProperty);
            secondaryColor = characterMaterial.GetColor(secondaryProperty);
            leatherPrimaryColor = characterMaterial.GetColor(leatherPrimaryProperty);
            leatherSecondaryColor = characterMaterial.GetColor(leatherSecondaryProperty);
            metalPrimaryColor = characterMaterial.GetColor(metalPrimaryProperty);
            metalSecondaryColor = characterMaterial.GetColor(metalSecondaryProperty);
            metalDarkColor = characterMaterial.GetColor(metalDarkProperty);
            hairColor = characterMaterial.GetColor(hairProperty);
            skinColor = characterMaterial.GetColor(skinProperty);
            stubbleColor = characterMaterial.GetColor(stubbleProperty);
            scarColor = characterMaterial.GetColor(scarProperty);
            bodyArtColor = characterMaterial.GetColor(bodyArtProperty);
            eyeColor = characterMaterial.GetColor(eyesProperty);
        }

        private void SetupBodyPart(ModularBodyPart bodyPart, ref int index, ref int maxIndex) {
            if (characterBody.TryGetValue(bodyPart, out GameObject[] partsArray)) {
                maxIndex = partsArray.Length - 1;
                bool hasActivePart = false;
                for (int i = 0; i < partsArray.Length; i++) {
                    if (partsArray[i].activeSelf) {
                        index = i;
                        hasActivePart = true;
                    }
                }
                if (!hasActivePart)
                    index = -1;
            } else {
                index = -1;
            }
        }

        private void OnGUI() {
            if (Selection.activeGameObject != null) {
                if (characterManager != null) {
                    if (!characterManager.IsInitialized) {
                        GUILayout.Label("You must initialize character before using the editor");
                        if (GUILayout.Button("Open Setup Wizard")) {
                            ModularSetupWizard.ShowWizard();
                        }
                        return;
                    }

                    GUILayout.BeginHorizontal();

                    GUILayout.BeginVertical();
                    scrollPos = GUILayout.BeginScrollView(scrollPos);

                    SetupPartSlider(ModularBodyPart.Helmet, ref helmetCurrentIndex, helmetMaxIndex);
                    SetupPartSlider(ModularBodyPart.HeadAttachment, ref headAttachmentCurrentIndex, headAttachmentMaxIndex);
                    SetupPartSlider(ModularBodyPart.Head, ref headCurrentIndex, headMaxIndex);
                    SetupPartSlider(ModularBodyPart.Hat, ref hatCurrentIndex, hatMaxIndex);
                    SetupPartSlider(ModularBodyPart.Mask, ref maskCurrentIndex, maskMaxIndex);
                    SetupPartSlider(ModularBodyPart.HeadCovering, ref headCoveringCurrentIndex, headCoveringMaxIndex);
                    SetupPartSlider(ModularBodyPart.Hair, ref hairCurrentIndex, hairMaxIndex);
                    SetupPartSlider(ModularBodyPart.Eyebrow, ref eyebrowCurrentIndex, eyebrowMaxIndex);
                    SetupPartSlider(ModularBodyPart.Ear, ref earCurrentIndex, earMaxIndex);
                    SetupPartSlider(ModularBodyPart.FacialHair, ref facialHairCurrentIndex, facialHairMaxIndex);
                    SetupPartSlider(ModularBodyPart.BackAttachment, ref backAttachmentCurrentIndex, backAttachmentMaxIndex);
                    SetupPartSlider(ModularBodyPart.Torso, ref torsoCurrentIndex, torsoMaxIndex);
                    SetupPartSlider(ModularBodyPart.ShoulderAttachmentRight, ref shoulderAttachmentRightCurrentIndex, shoulderAttachmentRightMaxIndex);
                    SetupPartSlider(ModularBodyPart.ShoulderAttachmentLeft, ref shoulderAttachmentLeftCurrentIndex, shoulderAttachmentLeftMaxIndex);
                    SetupPartSlider(ModularBodyPart.ArmUpperRight, ref armUpperRightCurrentIndex, armUpperRightMaxIndex);
                    SetupPartSlider(ModularBodyPart.ArmUpperLeft, ref armUpperLeftCurrentIndex, armUpperLeftMaxIndex);
                    SetupPartSlider(ModularBodyPart.ElbowAttachmentRight, ref elbowAttachmentRightCurrentIndex, elbowAttachmentRightMaxIndex);
                    SetupPartSlider(ModularBodyPart.ElbowAttachmentLeft, ref elbowAttachmentLeftCurrentIndex, elbowAttachmentLeftMaxIndex);
                    SetupPartSlider(ModularBodyPart.ArmLowerRight, ref armLowerRightCurrentIndex, armLowerRightMaxIndex);
                    SetupPartSlider(ModularBodyPart.ArmLowerLeft, ref armLowerLeftCurrentIndex, armLowerLeftMaxIndex);
                    SetupPartSlider(ModularBodyPart.HandRight, ref handRightCurrentIndex, handRightMaxIndex);
                    SetupPartSlider(ModularBodyPart.HandLeft, ref handLeftCurrentIndex, handLeftMaxIndex);
                    SetupPartSlider(ModularBodyPart.HipsAttachment, ref hipsAttachmentCurrentIndex, hipsAttachmentMaxIndex);
                    SetupPartSlider(ModularBodyPart.Hips, ref hipsCurrentIndex, hipsMaxIndex);
                    SetupPartSlider(ModularBodyPart.KneeAttachmentRight, ref kneeAttachmentRightCurrentIndex, kneeAttachmentRightMaxIndex);
                    SetupPartSlider(ModularBodyPart.KneeAttachmentLeft, ref kneeAttachmentLeftCurrentIndex, kneeAttachmentLeftMaxIndex);
                    SetupPartSlider(ModularBodyPart.LegRight, ref legRightCurrentIndex, legRightMaxIndex);
                    SetupPartSlider(ModularBodyPart.LegLeft, ref legLeftCurrentIndex, legLeftMaxIndex);

                    GUILayout.EndScrollView();

                    GUILayout.EndVertical();

                    GUILayout.BeginVertical();

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Male")) {
                        characterManager.SwapGender(Gender.Male);
                        InitializeBodyParts();
                    }
                    if (GUILayout.Button("Female")) {
                        characterManager.SwapGender(Gender.Female);
                        InitializeBodyParts();
                    }
                    GUILayout.EndHorizontal();

                    SetupColorFields(ref primaryColor, "Primary Color", primaryProperty);
                    SetupColorFields(ref secondaryColor, "Secondary Color", secondaryProperty);
                    SetupColorFields(ref leatherPrimaryColor, "Leather Primary Color", leatherPrimaryProperty);
                    SetupColorFields(ref leatherSecondaryColor, "Leather Secondary Color", leatherSecondaryProperty);
                    SetupColorFields(ref metalPrimaryColor, "Metal Primary Color", metalPrimaryProperty);
                    SetupColorFields(ref metalSecondaryColor, "Metal Secondary Color", metalSecondaryProperty);
                    SetupColorFields(ref metalDarkColor, "Metal Dark Color", metalDarkProperty);
                    SetupColorFields(ref hairColor, "Hair Color", hairProperty);
                    SetupColorFields(ref skinColor, "Skin Color", skinProperty);
                    SetupColorFields(ref stubbleColor, "Stubble Color", stubbleProperty);
                    SetupColorFields(ref scarColor, "Scar Color", scarProperty);
                    SetupColorFields(ref bodyArtColor, "BodyArt Color", bodyArtProperty);
                    SetupColorFields(ref eyeColor, "Eye Color", eyesProperty);

                    GUILayout.EndVertical();

                    GUILayout.EndHorizontal();
                } else {
                    GUILayout.BeginVertical();

                    GUILayout.Label("Target does not have a ModularManager component attached.");
                    if (GUILayout.Button("Open Setup Wizard")) {
                        ModularSetupWizard.ShowWizard();
                    }

                    GUILayout.EndVertical();
                }
            }
        }

        private void SetupColorFields(ref Color partColor, string label, string shaderProperty) {
            EditorGUI.BeginChangeCheck();
            partColor = EditorGUILayout.ColorField(label, partColor);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(characterMaterial, "Undo Color change");
                characterMaterial.SetColor(shaderProperty, partColor);
            }
        }

        private void SetupPartSlider(ModularBodyPart bodyPart, ref int partIndex, int maxIndex) {
            EditorGUI.BeginChangeCheck();
            partIndex = EditorGUILayout.IntSlider(bodyPart.ToString(), partIndex, -1, maxIndex);
            if (EditorGUI.EndChangeCheck()) {
                if (partIndex == -1)
                    characterManager.DeactivatePart(bodyPart);
                else
                    characterManager.ActivatePart(bodyPart, partIndex);
            }

        }
    }
}
