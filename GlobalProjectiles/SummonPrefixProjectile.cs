using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ID;

namespace SummonUpgrade.GlobalProjectiles
{
    public class SummonPrefixProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public int TagDamage;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo itemSourceAmmo && itemSourceAmmo.Item != null)
            {
                if (itemSourceAmmo.Item.TryGetGlobalItem(out GlobalItems.SummonPrefixItem globalItem))
                    TagDamage = globalItem.TagDamage;
            }
            else if (source is EntitySource_ItemUse itemSource && itemSource.Item != null)
            {
                if (itemSource.Item.TryGetGlobalItem(out GlobalItems.SummonPrefixItem globalItem))
                    TagDamage = globalItem.TagDamage;
            }
            else if (source is EntitySource_Parent parentSource && parentSource.Entity is Projectile parentProj)
            {
                if (parentProj.TryGetGlobalProjectile(out SummonPrefixProjectile globalParent))
                    TagDamage = globalParent.TagDamage;
            }
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (TagDamage > 0 && (projectile.minion || projectile.sentry || ProjectileID.Sets.MinionShot[projectile.type] || ProjectileID.Sets.SentryShot[projectile.type]))
            {
                bool hasWhipTag = false;
                for (int i = 0; i < NPC.maxBuffs; i++)
                {
                    int buffType = target.buffType[i];
                    if (buffType > 0)
                    {
                        // Vanilla whip buffs
                        if (buffType == 130 || buffType == 285 || buffType == 286 || buffType == 287 || 
                            buffType == 288 || buffType == 289 || buffType == 290 || buffType == 312 || buffType == 322)
                        {
                            hasWhipTag = true;
                            break;
                        }
                    }
                }

                if (hasWhipTag)
                {
                    float tagMultiplier = 1f;
                    if (projectile.type == ProjectileID.DD2LightningAuraT1 || projectile.type == ProjectileID.DD2LightningAuraT2 || projectile.type == ProjectileID.DD2LightningAuraT3)
                    {
                        tagMultiplier = 0.5f;
                    }

                    modifiers.FlatBonusDamage += TagDamage * tagMultiplier;
                }
            }
        }
    }
}
