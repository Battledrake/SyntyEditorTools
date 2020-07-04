using UnityEngine;
using BattleDrakeStudios.ModularCharacters;
using System.Linq;

public class EquipmentManager : MonoBehaviour {

    [SerializeField] private Item[] equipmentSlots;

    private ModularCharacterManager characterManager;

    private void Awake() {
        characterManager = GetComponent<ModularCharacterManager>();
    }

    private void Start() {
        foreach (var item in equipmentSlots) {
            EquipItem(item);
        }
    }

    private void EquipItem(Item itemToEquip) {

        foreach (var part in itemToEquip.modularArmor.armorParts) {
            if (part.partID > -1) {
                characterManager.ActivatePart(part.bodyType, part.partID);
                ColorPropertyLinker[] armorColors = itemToEquip.modularArmor.armorColors;
                for (int i = 0; i < armorColors.Length; i++) {
                    characterManager.SetPartColor(part.bodyType, part.partID, armorColors[i].property, armorColors[i].color);
                }
            } else {
                characterManager.DeactivatePart(part.bodyType);
            }
        }
    }
}
