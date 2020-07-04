namespace BattleDrakeStudios.ModularCharacters {
    //Links a bodypart with a partid to fit into a list when serialization is needed.
    [System.Serializable]
    public class BodyPartLinker {
        public ModularBodyPart bodyType;
        public int partID;

        public BodyPartLinker(ModularBodyPart bodyType) {
            this.bodyType = bodyType;
            this.partID = 0;
        }

        public BodyPartLinker(ModularBodyPart bodyType, int partID) {
            this.bodyType = bodyType;
            this.partID = partID;
        }
    }
}
