using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawAndOrderSV.SecretLab
{
    internal class Comms
    {
        internal static bool initialPowerMessage = false;
        internal static bool jiaIntroduced = false;

        internal static void Init()
        {
            GameLocation.RegisterTileAction("sdvhead.LawAndOrderSV_SecretLab_Comms_Interact", Comms_Interact);
        }

        private static bool Comms_Interact(GameLocation location, string[] args, Farmer farmer, Microsoft.Xna.Framework.Point point)
        {
            if ((PowerStation.totalPowerPercent > 0))
            {
                bool sentMessage = false;
                if (!initialPowerMessage)
                {
                    Game1.drawObjectDialogue(I18n.SecretLabCommsInitialPower());
                    sentMessage = true;
                    initialPowerMessage = true;
                }
                if (!jiaIntroduced)
                {
                    if(sentMessage)
                    {
                        Game1.drawObjectDialogue(I18n.SecretLabCommsJiaIntroduce());
                        Game1.afterDialogues = (Game1.afterFadeFunction)Delegate.Combine(Game1.afterDialogues, (Game1.afterFadeFunction)delegate
                        {
                            Game1.drawObjectDialogue(I18n.SecretLabCommsJiaIntroduceRespondFarmer());
                        });
                    }
                    else
                    {
                        Game1.drawObjectDialogue(I18n.SecretLabCommsJiaIntroduce());
                    }
                    jiaIntroduced = true;

                    Game1.afterDialogues = (Game1.afterFadeFunction)Delegate.Combine(Game1.afterDialogues, (Game1.afterFadeFunction)delegate
                    {
                        Game1.drawObjectDialogue(I18n.SecretLabCommsJiaIntroduceRespondFarmer());
                    });

                    /*
                    if (question_and_answer == "DesertScholar_Yes")
                    {
                        _currentScholarQuestion++;
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Scholar_Intro2"));
                        Game1.afterDialogues = (Game1.afterFadeFunction)Delegate.Combine(Game1.afterDialogues, (Game1.afterFadeFunction)delegate
                        {
                            generateNextScholarQuestion();
                        });
                    }
                    */
                }
            }
            else
            {
                Game1.drawObjectDialogue(I18n.SecretLabCommsNoPower());

            }

            return true;
        }

    }
}
