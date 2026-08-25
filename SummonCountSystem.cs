using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace SummonUpgrade
{
    public class SummonCountSystem : ModSystem
    {
        // Maps a BuffType to a list of ProjectileTypes that the buff represents
        public static Dictionary<int, List<int>> BuffToProjectiles = new Dictionary<int, List<int>>();

        public override void PostSetupContent()
        {
            // Iterate over all items to find summon/sentry weapons and map their buffs to their projectiles
            for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
                Item item = new Item();
                item.SetDefaults(i);
                
                // If it's a summon weapon, has a buff, and shoots a projectile
                if (item.damage > 0 && item.CountsAsClass(DamageClass.Summon) && item.buffType > 0 && item.shoot > 0)
                {
                    if (!BuffToProjectiles.ContainsKey(item.buffType))
                    {
                        BuffToProjectiles[item.buffType] = new List<int>();
                    }
                    
                    if (!BuffToProjectiles[item.buffType].Contains(item.shoot))
                    {
                        BuffToProjectiles[item.buffType].Add(item.shoot);
                    }
                }
            }
        }
    }

    public class SummonProjectileTracker : GlobalProjectile
    {
        public override void OnSpawn(Projectile projectile, Terraria.DataStructures.IEntitySource source)
        {
            // Dynamically track any projectile spawned by an item that gives a buff
            if (source is Terraria.DataStructures.EntitySource_ItemUse itemSource)
            {
                Item item = itemSource.Item;
                if (item != null && item.buffType > 0)
                {
                    if (!SummonCountSystem.BuffToProjectiles.ContainsKey(item.buffType))
                        SummonCountSystem.BuffToProjectiles[item.buffType] = new List<int>();
                    
                    if (!SummonCountSystem.BuffToProjectiles[item.buffType].Contains(projectile.type))
                        SummonCountSystem.BuffToProjectiles[item.buffType].Add(projectile.type);
                }
            }
            // Or if spawned by another projectile (e.g. dummy spawner to real minion), inherit the buff
            else if (source is Terraria.DataStructures.EntitySource_Parent parentSource && parentSource.Entity is Projectile parentProj)
            {
                foreach (var kvp in SummonCountSystem.BuffToProjectiles)
                {
                    if (kvp.Value.Contains(parentProj.type))
                    {
                        if (!kvp.Value.Contains(projectile.type))
                            kvp.Value.Add(projectile.type);
                    }
                }
            }
            // Or if spawned directly by a buff update
            else if (source is Terraria.DataStructures.EntitySource_Buff buffSource)
            {
                if (!SummonCountSystem.BuffToProjectiles.ContainsKey(buffSource.BuffId))
                    SummonCountSystem.BuffToProjectiles[buffSource.BuffId] = new List<int>();
                    
                if (!SummonCountSystem.BuffToProjectiles[buffSource.BuffId].Contains(projectile.type))
                    SummonCountSystem.BuffToProjectiles[buffSource.BuffId].Add(projectile.type);
            }
        }
    }
}
