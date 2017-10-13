using System.Collections.Generic;
using System.Linq;
using Warsmiths.Common.Domain.CommonCharacterProfile.Primary;
using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.CommonCharacterProfile.Groups
{
    public class MasteryGroup
    {
        public static readonly float[] ModBossPoint = {1, 1.2f, 1.3f, 1, 4f, 1, 55f, 1.7f, 2f};

        public int Summary;
        public MindPrimary Mind;
        public BodyPrimary Body;
        public SkillPrimary Skill;

        public MasteryGroup()
        {
            Mind = new MindPrimary();
            Body = new BodyPrimary();
            Skill = new SkillPrimary();

            Summary = 0;
        }

        public void Calculate(Character c)
        {
            Mind.Calculate(c);
            Body.Calculate(c);
            Skill.Calculate(c);

            var totalAnomality = Skill.TotalAnomaly.Value;
            var diff = totalAnomality - 25;
            var anomalityMod = 1 + diff/1.5f/100f;
            var anomalityReverseMod = 1 - diff / 1.5f / 100f;

            var aa = c.Equipment.Armor?.AttackAngle * c.CommonProfile.GetModuleMultiplier(ModulesTypesOfImpacts.MeleeDefenceAttackAngle) ?? 0f;
            // ReSharper disable once MergeConditionalExpression
            var angleKoef = aa > 0 ? 0.01f * (120 - aa) / 2.4f : 0;
            var armorKoef = 1 + angleKoef;

            var speeds = new List<float>();
            if (c.CommonProfile.Offence.MeleeAttack.AttackSpeed.Value > 0) speeds.Add(c.CommonProfile.Offence.MeleeAttack.AttackSpeed.Value);
            if (c.CommonProfile.Offence.RangedAttack.AttackSpeed.Value > 0) speeds.Add(c.CommonProfile.Offence.RangedAttack.AttackSpeed.Value);
            if (c.CommonProfile.Offence.MagicAttack.AttackSpeed.Value > 0) speeds.Add(c.CommonProfile.Offence.MagicAttack.AttackSpeed.Value);

            var weaponKoef = speeds.Count > 0 ? speeds.Min() : 1;

            var middleSpeed = (weaponKoef + armorKoef) / 2f;

            Skill.TotalSpeed.Value = middleSpeed;
            diff = middleSpeed - 1;
            var modSpeed = 1 + diff /0.02f / 100f;
            var modReverseSpeed = 1 - diff / 0.02f / 100f;

            var totalWeight = Skill.TotalWeight.Value;
            diff = totalWeight - 600;
            var modWeight = 1 + diff /30f / 100f;
            var modReverseWeight = 1 - diff / 30f / 100f;

            Body.Vision.Value = (100 + 10*(c.CommonProfile.Characteristics.Power.Value - 1) + c.CommonProfile.GetSeconadryModulesModifer(ModulesTypesOfImpacts.BodyVision)) * anomalityMod;

            Body.Movement.Value = (100 + 10 * (c.CommonProfile.Characteristics.Speed.Value - 1) + c.CommonProfile.GetSeconadryModulesModifer(ModulesTypesOfImpacts.BodyMovement)) * modReverseWeight;

            Mind.Immunity.Value = (100 + 10 * (c.CommonProfile.Characteristics.Intellect.Value - 1) + c.CommonProfile.GetSeconadryModulesModifer(ModulesTypesOfImpacts.MindImmunity)) * modReverseSpeed;

            Body.Charge.Value = (100 + 10 * (c.CommonProfile.Characteristics.Endurance.Value - 1) + c.CommonProfile.GetSeconadryModulesModifer(ModulesTypesOfImpacts.BodyCharge)) * modWeight;

            Mind.Initiative.Value = (100 + 10 * (c.CommonProfile.Characteristics.Dexterity.Value - 1) + c.CommonProfile.GetSeconadryModulesModifer(ModulesTypesOfImpacts.MindInitiative)) * modSpeed;

            Mind.Stamina.Value = (100 + 10 * (c.CommonProfile.Characteristics.Wisdom.Value - 1) + c.CommonProfile.GetSeconadryModulesModifer(ModulesTypesOfImpacts.MindStamina)) * anomalityReverseMod;

            Summary = 0;
        }
    }
}
