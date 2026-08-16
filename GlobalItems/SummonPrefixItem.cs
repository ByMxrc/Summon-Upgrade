using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;

namespace SummonUpgrade.GlobalItems
{
    public class SummonPrefixItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public int TagDamage;
        public int ArmorPenetration;

        public override GlobalItem Clone(Item item, Item itemClone)
        {
            SummonPrefixItem myClone = (SummonPrefixItem)base.Clone(item, itemClone);
            myClone.TagDamage = TagDamage;
            myClone.ArmorPenetration = ArmorPenetration;
            return myClone;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (TagDamage > 0)
            {
                string text = Language.GetTextValue("Mods.SummonUpgrade.PrefixTagDamage", TagDamage);
                if (string.IsNullOrEmpty(text) || text == "Mods.SummonUpgrade.PrefixTagDamage")
                    text = $"+{TagDamage} summon tag damage";

                TooltipLine line = new TooltipLine(Mod, "PrefixSummonTagDamage", text)
                {
                    IsModifier = true,
                    IsModifierBad = false
                };
                
                tooltips.Add(line);
            }

            if (ArmorPenetration > 0)
            {
                string text = Language.GetTextValue("Mods.SummonUpgrade.PrefixArmorPenetration", ArmorPenetration);
                if (string.IsNullOrEmpty(text) || text == "Mods.SummonUpgrade.PrefixArmorPenetration")
                    text = $"+{ArmorPenetration} penetración de armadura";

                TooltipLine line = new TooltipLine(Mod, "PrefixSummonArmorPenetration", text)
                {
                    IsModifier = true,
                    IsModifierBad = false
                };
                
                tooltips.Add(line);
            }
        }
    }
}
