using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SummonUpgrade.GlobalBuffs
{
    public class SummonCountBuff : GlobalBuff
    {
        public override void PostDraw(SpriteBatch spriteBatch, int type, int buffIndex, BuffDrawParams drawParams)
        {
            if (SummonCountSystem.BuffToProjectiles.TryGetValue(type, out List<int> projectiles))
            {
                Player player = Main.LocalPlayer;
                int count = 0;
                
                foreach (int projType in projectiles)
                {
                    if (projType == Terraria.ID.ProjectileID.StardustDragon1)
                    {
                        int head = player.ownedProjectileCounts[Terraria.ID.ProjectileID.StardustDragon1];
                        int body1 = player.ownedProjectileCounts[Terraria.ID.ProjectileID.StardustDragon2];
                        int body2 = player.ownedProjectileCounts[Terraria.ID.ProjectileID.StardustDragon3];
                        int tail = player.ownedProjectileCounts[Terraria.ID.ProjectileID.StardustDragon4];
                        
                        int totalSegments = head + body1 + body2 + tail;
                        if (totalSegments > 0)
                        {
                            count += System.Math.Max(1, (totalSegments - 2) / 2);
                        }
                    }
                    else
                    {
                        count += player.ownedProjectileCounts[projType];
                    }
                }

                if (count > 0)
                {
                    string text = "x" + count;
                    // Draw on the bottom right corner of the buff icon instead of below it
                    Vector2 textPosition = drawParams.Position + new Vector2(drawParams.Texture.Width - 10, drawParams.Texture.Height - 12);
                    
                    Terraria.UI.Chat.ChatManager.DrawColorCodedStringWithShadow(
                        spriteBatch, 
                        FontAssets.MouseText.Value, 
                        text, 
                        textPosition, 
                        Color.White, 
                        0f, 
                        Vector2.Zero, 
                        new Vector2(0.8f) 
                    );
                }
            }
        }
    }
}
