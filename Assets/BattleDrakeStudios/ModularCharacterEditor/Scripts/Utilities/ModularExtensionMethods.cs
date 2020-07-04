
namespace BattleDrakeStudios.ModularCharacters {
    public static class ModularExtensionMethods {
        public static bool IsHead(this ModularBodyPart part) {
            if (part == ModularBodyPart.Head) {
                return true;
            }
            return false;
        }

        public static bool IsHeadPart(this ModularBodyPart part) {
            if (part == ModularBodyPart.Hair || part == ModularBodyPart.Eyebrow || part == ModularBodyPart.Ear || part == ModularBodyPart.FacialHair) {
                return true;
            }
            return false;
        }

        public static bool IsBaseBodyPart(this ModularBodyPart part) {
            if (part == ModularBodyPart.Head || part == ModularBodyPart.Torso || part == ModularBodyPart.ArmUpperRight || part == ModularBodyPart.ArmUpperLeft ||
                    part == ModularBodyPart.ArmLowerRight || part == ModularBodyPart.ArmLowerLeft || part == ModularBodyPart.HandRight || part == ModularBodyPart.HandLeft ||
                    part == ModularBodyPart.Hips || part == ModularBodyPart.LegRight || part == ModularBodyPart.LegLeft) {
                return true;
            }
            return false;
        }
    }
}
