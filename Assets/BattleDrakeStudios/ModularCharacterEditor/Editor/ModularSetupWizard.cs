using UnityEngine;
using UnityEditor;

namespace BattleDrakeStudios.ModularCharacters {
    public class ModularSetupWizard : EditorWindow {
        private enum SetupState {
            SelectGameObject,
            SelectExisting,
            SelectGenderOption,
            SelectMaterialOption,
            SetupDuplicateMaterial,
            SetupExistingMaterial,
            Ready
        }

        private ModularCharacterManager characterManager;

        private SetupState currentState = SetupState.SelectGameObject;

        private bool isExistingCharacter;
        private Gender characterGender;
        private Material characterMat;
        private string materialName;
        private bool isNewMaterial;

        [MenuItem("BattleDrakeStudios/ModularCharacter/SetupWizard")]
        public static void ShowWizard() {
            ModularSetupWizard wizardWindow = GetWindow<ModularSetupWizard>();
            wizardWindow.titleContent = new GUIContent("Setup Wizard");
            wizardWindow.maxSize = new Vector2(400.0f, 200.0f);
            wizardWindow.minSize = wizardWindow.maxSize;
            wizardWindow.autoRepaintOnSceneChange = true;
            wizardWindow.Show();
        }

        private void OnSelectionChange() {
            if (Selection.activeGameObject != null) {
                characterManager = Selection.activeGameObject.GetComponent<ModularCharacterManager>();
                if (characterManager != null) {
                    isExistingCharacter = false;
                    isNewMaterial = false;
                    currentState = SetupState.SelectExisting;
                } else
                    currentState = SetupState.SelectGameObject;
            }
            Repaint();
        }

        private void OnInspectorUpdate() {
            if (Selection.activeGameObject != null) {
                if (characterManager == null) {
                    characterManager = Selection.activeGameObject.GetComponent<ModularCharacterManager>();
                    if (characterManager != null)
                        currentState = SetupState.SelectExisting;
                    else
                        currentState = SetupState.SelectGameObject;
                    Repaint();
                }

                if (currentState == SetupState.SelectGameObject) {
                    characterManager = Selection.activeGameObject.GetComponent<ModularCharacterManager>();
                    if (characterManager != null)
                        currentState = SetupState.SelectExisting;
                    Repaint();
                }
            }
        }

        private void OnGUI() {
            if (Selection.activeGameObject == null)
                return;

            if (characterManager != null)
                if (characterManager.IsInitialized) {
                    GUILayout.Label("Character is initialized!");
                    return;
                }


            switch (currentState) {
                case SetupState.SelectGameObject:
                    if (characterManager == null) {
                        GUILayout.BeginVertical();

                        GUILayout.Label("Please select a gameobject with the ModularManager script attached.");
                        GUILayout.Label("Add one to selected gameobject?");
                        if (GUILayout.Button("Add ModularManager Component")) {
                            Selection.activeGameObject.AddComponent<ModularCharacterManager>();
                            currentState = SetupState.SelectExisting;
                        }

                        GUILayout.EndVertical();
                    }
                    break;

                case SetupState.SelectExisting:
                    GUILayout.BeginVertical();

                    GUILayout.Label("Is this a new or existing character?");

                    GUILayout.BeginHorizontal();
                    SetIsExisting();
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();
                    break;

                case SetupState.SelectGenderOption:
                    GUILayout.BeginVertical();

                    GUILayout.Label("Is it a male or female? (You can change this in the editor later)");

                    GUILayout.BeginHorizontal();
                    SetCharacterGender();
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();
                    break;

                case SetupState.SelectMaterialOption:
                    GUILayout.BeginVertical();

                    GUILayout.Label("Use existing material or create duplicate?");

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Create Duplicate"))
                        currentState = SetupState.SetupDuplicateMaterial;
                    else if (GUILayout.Button("Use Existing")) {
                        currentState = SetupState.SetupExistingMaterial;
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();
                    break;

                case SetupState.SetupDuplicateMaterial:
                    GUILayout.BeginVertical();

                    GUILayout.Label("Please insert the material to dupliate");
                    characterMat = EditorGUILayout.ObjectField(characterMat, typeof(Material), false) as Material;

                    GUILayout.Label("Please enter a name for the duplicate material.");
                    GUILayout.Label("(saves to: BattleDrakeStudios/ModularCharacterEditor/Materials");
                    materialName = GUILayout.TextField(materialName);
                    if (!string.IsNullOrEmpty(materialName) && characterMat != null) {
                        if (GUILayout.Button("Continue")) {
                            characterMat = new Material(characterMat);
                            characterMat.name = materialName;
                            isNewMaterial = true;
                            currentState = SetupState.Ready;
                        }
                    }


                    GUILayout.EndVertical();
                    break;

                case SetupState.SetupExistingMaterial:
                    GUILayout.BeginVertical();

                    GUILayout.Label("Please insert the desired material");
                    characterMat = EditorGUILayout.ObjectField(characterMat, typeof(Material), false) as Material;

                    if (characterMat != null) {
                        if (GUILayout.Button("Continue")) {
                            currentState = SetupState.Ready;
                        }
                    }

                    if (GUILayout.Button("Return"))
                        currentState = SetupState.SelectMaterialOption;
                    break;

                case SetupState.Ready:
                    GUILayout.BeginVertical();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Character: ");
                    if (isExistingCharacter)
                        GUILayout.Label("Existing");
                    else
                        GUILayout.Label("New");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Gender:      ");
                    if (characterGender == Gender.Male)
                        GUILayout.Label("Male");
                    else
                        GUILayout.Label("Female");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Material:      ");
                    GUILayout.Label(characterMat.name);
                    GUILayout.EndHorizontal();

                    if (GUILayout.Button("Commit")) {
                        CommitChanges(false);
                    }
                    if (GUILayout.Button("Commit and Show Editor")) {
                        CommitChanges(true);
                    }
                    if (GUILayout.Button("Reset")) {
                        currentState = SetupState.SelectExisting;
                        isExistingCharacter = false;
                        isNewMaterial = false;
                    }

                    GUILayout.EndVertical();
                    break;
            }
        }

        private void SetIsExisting() {
            if (GUILayout.Button("New")) {
                isExistingCharacter = false;
                currentState = SetupState.SelectGenderOption;
            } else if (GUILayout.Button("Existing")) {
                isExistingCharacter = true;
                currentState = SetupState.SelectGenderOption;
            }
        }

        private void SetCharacterGender() {
            if (GUILayout.Button("Male")) {
                characterGender = Gender.Male;
                currentState = SetupState.SelectMaterialOption;
            } else if (GUILayout.Button("Female")) {
                characterGender = Gender.Female;
                currentState = SetupState.SelectMaterialOption;
            }
        }

        private void CommitChanges(bool openEditor) {
            if (isNewMaterial) {
                if(!AssetDatabase.IsValidFolder("Assets/BattleDrakeStudios/ModularCharacterEditor/Materials")) {
                    AssetDatabase.CreateFolder("Assets/BattleDrakeStudios/ModularCharacterEditor", "Materials");
                }
                AssetDatabase.CreateAsset(characterMat, "Assets/BattleDrakeStudios/ModularCharacterEditor/Materials/" + materialName + ".mat");
            }


            if (isExistingCharacter) {
                characterManager.SetupExistingCharacter(characterGender, characterMat);
            } else {
                characterManager.SetupNewCharacter(characterGender, characterMat);
            }

            if (openEditor) {
                ModularCharacterEditor.ShowWindow();
            }

            isExistingCharacter = false;
            isNewMaterial = false;
        }
    }
}
