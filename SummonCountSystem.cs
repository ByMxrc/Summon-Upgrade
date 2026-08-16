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
}
