using UnityEngine;

namespace BattleDrakeStudios.ModularCharacters {
    [System.Serializable]
    public class ColorPropertyLinker {
        public string property;
        public Color color;

        public ColorPropertyLinker(string property) {
            this.property = property;
            this.color = new Color();
        }

        public ColorPropertyLinker(string property, Color color) {
            this.property = property;
            this.color = color;
        }
    }
}
