using Terraria;
using Terraria.ModLoader;

namespace SummonUpgrade.Prefixes
{
    public abstract class SummonPrefix : ModPrefix
    {
        public override PrefixCategory Category => PrefixCategory.AnyWeapon;

        public virtual float DamageMult => 1f;
        public virtual float KnockbackMult => 1f;
        public virtual int ArmorPenetration => 0;
        public virtual int TagDamage => 0;

        public override bool CanRoll(Item item)
        {
            if (item.DamageType != DamageClass.Summon)
                return false;
            
            // Cannot roll on Whips. Whips use SummonMeleeSpeed in 1.4.4.
            // Also checking ProjectileID.Sets.IsAWhip would be ideal, but we don't have the projectile here easily.
            // Items with SummonMeleeSpeed are almost exclusively whips.
            if (item.DamageType == DamageClass.SummonMeleeSpeed)
                return false;

            if (item.knockBack == 0f && KnockbackMult != 1f)
                return false;

            return true;
        }

        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            damageMult = DamageMult;
            knockbackMult = KnockbackMult;
        }

        public override void Apply(Item item)
        {
            item.ArmorPenetration += ArmorPenetration;

            if (item.TryGetGlobalItem(out GlobalItems.SummonPrefixItem globalItem))
            {
                globalItem.TagDamage = TagDamage;
                globalItem.ArmorPenetration = ArmorPenetration; // Store to show in tooltip maybe
            }
        }

        public override void ModifyValue(ref float valueMult)
        {
            // Terraria naturally increases value based on DamageMult and KnockbackMult, 
            // but it ignores custom stats like ArmorPenetration and TagDamage.
            // We must manually scale the value so the game (and our Monte Carlo simulation) 
            // knows that Eager (+25 AP) is actually extremely valuable!
            
            float customMultiplier = 1f;
            
            if (ArmorPenetration > 0)
                customMultiplier += (ArmorPenetration * 0.02f); // Each 1 AP = +2% value
                
            if (TagDamage > 0)
                customMultiplier += (TagDamage * 0.03f); // Each 1 Tag Dmg = +3% value
                
            valueMult *= customMultiplier;
        }
    }

    // --- Positive Modifiers ---

    public class FabledPrefix : SummonPrefix
    {
        public override float DamageMult => 1.15f;
        public override int ArmorPenetration => 10;
        public override int TagDamage => 3;
        public override float KnockbackMult => 1.15f;
    }

    public class LoyalPrefix : SummonPrefix
    {
        public override float DamageMult => 1.10f;
        public override int ArmorPenetration => 5;
        public override int TagDamage => 3;
        public override float KnockbackMult => 1.05f;
    }

    public class WorthyPrefix : SummonPrefix
    {
        public override float DamageMult => 1.15f;
        public override int ArmorPenetration => 8;
    }

    public class FocusedPrefix : SummonPrefix
    {
        public override float DamageMult => 1.10f;
        public override int TagDamage => 3;
    }

    public class EagerPrefix : SummonPrefix
    {
        public override int ArmorPenetration => 25;
    }

    public class BallisticPrefix : SummonPrefix
    {
        public override int TagDamage => 5;
    }

    public class ScragglingPrefix : SummonPrefix
    {
        public override float KnockbackMult => 1.25f;
    }

    // --- Negative Modifiers ---

    public class PatientPrefix : SummonPrefix
    {
        public override float DamageMult => 0.95f;
        public override int TagDamage => 3;
    }

    public class RabidPrefix : SummonPrefix
    {
        public override float DamageMult => 1.10f;
        public override float KnockbackMult => 0.90f;
    }

    public class IllTemperedPrefix : SummonPrefix
    {
        public override float DamageMult => 0.95f;
        public override int ArmorPenetration => 10;
    }

    public class PettyPrefix : SummonPrefix
    {
        public override float DamageMult => 0.70f;
    }

    public class FeeblePrefix : SummonPrefix
    {
        public override float KnockbackMult => 0.75f;
    }

    public class SkittishPrefix : SummonPrefix
    {
        public override float DamageMult => 0.85f;
        public override float KnockbackMult => 0.90f;
    }
}
