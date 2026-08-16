using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SummonUpgrade
{
    public class ReforgeSystem : ModSystem
    {
        public static int BestReforgeTimer = 0;
        public static string BestReforgeName = "";

        public class TrackedReforgeText
        {
            public int Index;
            public string Name;
        }

        public static List<string> RecentBestReforges = new List<string>();
        public static int ClearTimer = 0;
        public static int ReforgeCooldownTimer = 0;
        
        public static List<TrackedReforgeText> TrackedTexts = new List<TrackedReforgeText>();

        public override void PostUpdateEverything()
        {
            if (ReforgeCooldownTimer > 0)
            {
                ReforgeCooldownTimer--;
            }

            if (ClearTimer > 0)
            {
                ClearTimer--;
                if (ClearTimer <= 0)
                {
                    RecentBestReforges.Clear();
                }
            }

            // First, clean up all tracked texts to ensure no falsely tracked indices block new ones
            for (int i = TrackedTexts.Count - 1; i >= 0; i--)
            {
                var tracker = TrackedTexts[i];
                if (!Main.popupText[tracker.Index].active || Main.popupText[tracker.Index].name != tracker.Name)
                {
                    // Text died or index was reused
                    TrackedTexts.RemoveAt(i);
                }
                else
                {
                    // Keep vanilla text transparent so our custom drawing takes over
                    Main.popupText[tracker.Index].color = Microsoft.Xna.Framework.Color.Transparent;
                }
            }

            // Find any newly spawned PopupText that matches a recent best reforge
            if (RecentBestReforges.Count > 0)
            {
                for (int i = 0; i < Main.popupText.Length; i++)
                {
                    if (Main.popupText[i].active)
                    {
                        if (RecentBestReforges.Contains(Main.popupText[i].name))
                        {
                            // Make sure we aren't already tracking this exact instance
                            bool alreadyTracked = false;
                            foreach (var t in TrackedTexts)
                            {
                                if (t.Index == i) alreadyTracked = true;
                            }
                            
                            if (!alreadyTracked)
                            {
                                TrackedTexts.Add(new TrackedReforgeText { Index = i, Name = Main.popupText[i].name });
                            }
                        }
                    }
                }
            }
        }

        public override void ModifyInterfaceLayers(System.Collections.Generic.List<Terraria.UI.GameInterfaceLayer> layers)
        {
            // By inserting at index 0, we guarantee it draws at the very bottom of the UI stack,
            // directly over the game world, so menus, inventory, and other UI elements draw ON TOP of it.
            layers.Insert(0, new Terraria.UI.LegacyGameInterfaceLayer(
                "SummonUpgrade: Best Reforge Text",
                delegate
                {
                    DrawCustomReforgeTexts();
                    return true;
                },
                Terraria.UI.InterfaceScaleType.Game));
        }

        private void DrawCustomReforgeTexts()
        {
            if (TrackedTexts.Count == 0) return;

            var font = Terraria.GameContent.FontAssets.MouseText.Value;

            foreach (var tracker in TrackedTexts)
            {
                var popup = Main.popupText[tracker.Index];
                if (popup.active && popup.name == tracker.Name)
                {
                    // Base color alpha from the game's text pulsing
                    float discoAlpha = Main.mouseTextColor / 255f;
                    
                    // Generate a custom rainbow based on time
                    float hue = (Main.GlobalTimeWrappedHourly * 0.5f) % 1f;
                    
                    // Shift the border hue slightly forward so it contrasts with the core text
                    float borderHue = (hue + 0.15f) % 1f;
                    
                    Color baseTextColor = Main.hslToRgb(hue, 1f, 0.5f);
                    Color baseBorderColor = Main.hslToRgb(borderHue, 1f, 0.3f); // slightly darker border for readability
                    
                    Color textColor = new Color((int)(baseTextColor.R * discoAlpha), (int)(baseTextColor.G * discoAlpha), (int)(baseTextColor.B * discoAlpha), Main.mouseTextColor);
                    Color borderColor = new Color((int)(baseBorderColor.R * discoAlpha), (int)(baseBorderColor.G * discoAlpha), (int)(baseBorderColor.B * discoAlpha), Main.mouseTextColor);

                    string text = tracker.Name;
                    Vector2 textStringSize = font.MeasureString(text);
                    Vector2 origin = textStringSize / 2f;
                    
                    // Subtract Main.screenPosition to correctly draw in world space with InterfaceScaleType.Game!
                    Vector2 position = popup.position - Main.screenPosition + origin;
                    float scale = popup.scale;

                    // 8 directional offsets for a THICK border
                    Vector2[] offsets = new Vector2[]
                    {
                        new Vector2(-2, 0), new Vector2(2, 0),
                        new Vector2(0, -2), new Vector2(0, 2),
                        new Vector2(-2, -2), new Vector2(2, -2),
                        new Vector2(-2, 2), new Vector2(2, 2)
                    };

                    // Draw thick rainbow border
                    foreach (Vector2 offset in offsets)
                    {
                        Terraria.UI.Chat.ChatManager.DrawColorCodedString(
                            Main.spriteBatch, font, text, position + (offset * scale), borderColor, 0f, origin, new Vector2(scale)
                        );
                    }

                    // Draw the core text on top
                    Terraria.UI.Chat.ChatManager.DrawColorCodedString(
                        Main.spriteBatch, font, text, position, textColor, 0f, origin, new Vector2(scale)
                    );
                }
            }
        }
    }
}
