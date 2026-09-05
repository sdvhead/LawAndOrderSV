using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Collections;

namespace LawAndOrderSV.SecretLab
{
    internal class MiscInteracts
    {
        public static void Init()
        {
            GameLocation.RegisterTileAction("sdvhead.LawAndOrderSV_SecretLab_BigRedButton", BigRedButton_Interact);
        }

        private static bool BigRedButton_Interact(GameLocation location, string[] args, Farmer farmer, Microsoft.Xna.Framework.Point point)
        {
            ModEntry.Log("big red button pressed");

            List<Vector2> vectors = new List<Vector2>();
            vectors.Add(new Vector2(18.7f, 22.2f) * 64f);
            vectors.Add(new Vector2(20.7f, 22.2f) * 64f);
            vectors.Add(new Vector2(22.7f, 22.2f) * 64f);
            vectors.Add(new Vector2(30.7f, 22.2f) * 64f);
            vectors.Add(new Vector2(33.7f, 22.2f) * 64f);
            vectors.Add(new Vector2(41.7f, 22.2f) * 64f);

            vectors.Add(new Vector2(18.3f, 22.2f) * 64f);
            vectors.Add(new Vector2(20.3f, 22.2f) * 64f);
            vectors.Add(new Vector2(22.3f, 22.2f) * 64f);
            vectors.Add(new Vector2(30.3f, 22.2f) * 64f);
            vectors.Add(new Vector2(33.3f, 22.2f) * 64f);
            vectors.Add(new Vector2(41.3f, 22.2f) * 64f);

            Game1.playSound("Meteorite");
            for (var i = 2; i < 15; i++)
            {
                DelayedAction.functionAfterDelay(() =>
                {
                    Game1.playSound("fireball");
                    foreach (Vector2 vector in vectors)
                    {
                        Utility.addSmokePuff(location, vector);
                    }

                }, i*150);
            }
            return true;

        }

    }
}
