using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SummonUpgrade.GlobalItems
{
    public class ReforgeGlobalItem : GlobalItem
    {
        public bool hasBestPrefix = false;

        public override bool InstancePerEntity => true;

        public override GlobalItem Clone(Item item, Item itemClone)
        {
            ReforgeGlobalItem myClone = (ReforgeGlobalItem)base.Clone(item, itemClone);
            myClone.hasBestPrefix = hasBestPrefix;
            return myClone;
        }

        public override bool CanReforge(Item item)
        {
            // Block reforging if the cooldown timer is active
            if (ReforgeSystem.ReforgeCooldownTimer > 0)
            {
                return false;
            }
            return base.CanReforge(item);
        }

        public override bool AllowPrefix(Item item, int pre)
        {
            // Block Magic prefixes for non-whip Summon weapons
            bool isWhip = item.shoot > 0 && ProjectileID.Sets.IsAWhip[item.shoot];
            if (item.damage > 0 && item.CountsAsClass(DamageClass.Summon) && !isWhip)
            {
                if ((pre >= PrefixID.Mystic && pre <= PrefixID.Manic) || pre == PrefixID.Mythical)
                {
                    return false;
                }
            }
            return base.AllowPrefix(item, pre);
        }

        public override void PostReforge(Item item)
        {
            if (IsBestPrefix(item))
            {
                hasBestPrefix = true;
                
                // Add to recent best reforges list
                ReforgeSystem.RecentBestReforges.Add(item.AffixName());
                ReforgeSystem.ClearTimer = 300;
                ReforgeSystem.ReforgeCooldownTimer = 60; // 1 second cooldown
                
                // Use the exact sound path the user requested so resource packs can override it
                SoundEngine.PlaySound(new SoundStyle("Terraria/Sounds/Custom/best_reforge") { Volume = 0.8f });
            }
            else
            {
                hasBestPrefix = false;
                // No need to clear RecentBestReforges here, the timer handles it
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (hasBestPrefix || IsBestPrefix(item)) 
            {
                // The item name will be displayed in rainbow color
                foreach (TooltipLine line in tooltips)
                {
                    if (line.Mod == "Terraria" && line.Name == "ItemName")
                    {
                        line.OverrideColor = Main.DiscoColor;
                        break;
                    }
                }
            }
        }

        private static Dictionary<int, HashSet<int>> bestPrefixesCache = new Dictionary<int, HashSet<int>>();

        public static bool IsBestPrefix(Item item)
        {
            int prefix = item.prefix;
            if (prefix == 0) return false;

            // 1. Accessories (Hardcoded as they don't roll dynamically the same way)
            if (prefix == PrefixID.Warding || prefix == PrefixID.Menacing || prefix == PrefixID.Lucky) return true;

            // 2. Tools
            if (prefix == PrefixID.Light || prefix == PrefixID.Massive) return true;

            // 3. Ruthless is a unique case: its monetary 'value' is lower than Mythical, 
            // but mechanically it is considered a top-tier choice for pure summon damage.
            if (item.CountsAsClass(DamageClass.Summon) && prefix == PrefixID.Ruthless) return true;

            // 5. Dynamic Monte Carlo Check for the absolute highest value prefix this specific weapon can NATURALLY roll
            if (item.damage > 0)
            {
                if (!bestPrefixesCache.TryGetValue(item.type, out HashSet<int> bestPrefixes))
                {
                    bestPrefixes = new HashSet<int>();
                    int highestValue = 0;
                    Dictionary<int, int> prefixValues = new Dictionary<int, int>();
                    
                    // Simulate the Goblin Tinkerer 1000 times to mathematically discover all naturally possible 
                    // prefixes for this specific weapon and find the one that yields the highest item value.
                    // (Takes < 1 millisecond and is cached forever for this item type).
                    for (int i = 0; i < 1000; i++)
                    {
                        Item clone = new Item();
                        clone.SetDefaults(item.type);
                        clone.Prefix(-1); // Roll random valid prefix
                        
                        if (clone.prefix > 0)
                        {
                            prefixValues[clone.prefix] = clone.value;
                            if (clone.value > highestValue)
                            {
                                highestValue = clone.value;
                            }
                        }
                    }
                    
                    // Any prefix that reaches the highest possible value is considered a "best" prefix (e.g. Legendary, Eager, Unreal)
                    foreach (var kvp in prefixValues)
                    {
                        if (kvp.Value == highestValue)
                        {
                            bestPrefixes.Add(kvp.Key);
                        }
                    }
                    
                    bestPrefixesCache[item.type] = bestPrefixes;
                }
                
                if (bestPrefixes.Contains(prefix)) return true;
            }

            return false;
        }
    }
}
