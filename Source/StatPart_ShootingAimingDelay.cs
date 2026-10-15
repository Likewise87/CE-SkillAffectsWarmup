using RimWorld;
using Verse;

namespace TSA.CE.SkillAffectsWarmup
{
    public class StatPart_ShootingAimingDelay : StatPart
    {
        public override void TransformValue(StatRequest req, ref float val)
        {
            if (!req.HasThing || req.Thing is not Pawn pawn) return;
            if (pawn.skills == null) return;

            int skill = pawn.skills.GetSkill(SkillDefOf.Shooting).Level;
            float factor = SkillAffectsWarmup_Settings.Current.GetFactorForSkill(skill);

            val *= factor; // we scale the AimingDelayFactor
        }

        public override string ExplanationPart(StatRequest req)
        {
            if (!req.HasThing || req.Thing is not Pawn pawn) return null;
            if (pawn.skills == null) return null;

            // Base warm-up from the currently equipped weapon (if any)
            float baseWarmup = 0f;
            string weaponLabel = "—";
            var eq = pawn.equipment;
            var primary = eq?.Primary;
            if (primary != null && primary.TryGetComp<CompEquippable>()?.PrimaryVerb != null)
            {
                var verb = primary.TryGetComp<CompEquippable>().PrimaryVerb;
                baseWarmup = verb.verbProps?.warmupTime ?? 0f;
                weaponLabel = primary.LabelShortCap;
            }

            int skill = pawn.skills.GetSkill(SkillDefOf.Shooting).Level;
            float skillFactor = SkillAffectsWarmup_Settings.Current.GetFactorForSkill(skill);

            // Ask the game for the (post-processed) AimingDelayFactor so "final warm-up" matches what the game uses
            float finalAimingTime = pawn.GetStatValue(StatDefOf.AimingDelayFactor, true);
            float finalWarmup = baseWarmup * finalAimingTime;

            // Show all three: base warm-up, our skill factor, and the final warm-up
            return
                $"CE SkillAffectsWarmup (Shooting skill {skill}): x{skillFactor * 100:0}%\n" +
                $"Base warm-up ({weaponLabel}): {baseWarmup:0.00}s\n" +
                $"→ Final warm-up time: {finalWarmup:0.00}s";
        }
    }
}
