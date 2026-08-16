using Terraria;
using Terraria.ModLoader;

namespace SummonUpgrade.GlobalItems
{
    public class NoManaSummonsGlobalItem : GlobalItem
    {
        public override void SetDefaults(Item item)
        {
            // Remove mana cost for all summoner weapons and sentries
            if (item.damage > 0 && (item.CountsAsClass(DamageClass.Summon) || item.sentry))
            {
                item.mana = 0;
            }
        }
    }
}
