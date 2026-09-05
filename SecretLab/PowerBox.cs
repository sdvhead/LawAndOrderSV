using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Menus;
using System.Runtime.CompilerServices;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace LawAndOrderSV.SecretLab
{
    internal class PowerBox
    {
        public int X { get; set; }
        public int Y { get; set; }

        public string MapName { get; set; }

        public bool Powered { get; set; }

        private static readonly string BatteryId = "(O)787";
        public PowerBox(int x, int y, string mapName)
        {
            //x and y are the coordinates for the upper left corner of power box
            this.X = x;
            this.Y = y;
            this.MapName = mapName;
            this.Powered = false;
            //this.BatteryQuestionKey = ModEntry.ModId + "_SecretLab_BatteryQuestionKey_"+id;
        }

        private static bool PlayerHasBattery()
        {
            if (Game1.player.Items.ContainsId(BatteryId, 1)) return true;
            return false;
        }

        public bool PowerMenu(GameLocation location, string[] args, Farmer farmer, Microsoft.Xna.Framework.Point point)
        {
            if (!this.Powered)
            {
                if (PlayerHasBattery())
                {
                    location.createQuestionDialogue(I18n.SecretLab_PowerStation_InsertBatteryQuestion(), location.createYesNoResponses(), PowerBoxFactory.batteryQuestionKey);
                }
                else
                {
                    Game1.drawObjectDialogue(I18n.SecretLab_PowerStation_NoBatteryInventory());
                }
            }
            else //station already has power
            {
                if(PowerBoxFactory.unusedPowerPercent > 0)
                {
                    location.createQuestionDialogue(I18n.SecretLab_PowerStation_DeliverPowerQuestionAvailable(), PowerBoxFactory.ManagePowerResponses(), PowerBoxFactory.deliverPowerQuestionKey);
                }else if (PowerBoxFactory.totalPowerPercent >= 100) //all stations online
                {
                    location.createQuestionDialogue(I18n.SecretLab_PowerStation_DeliverPowerQuestionMax(), PowerBoxFactory.ManagePowerResponses(), PowerBoxFactory.deliverPowerQuestionKey);
                }
                else //all available power being used
                {
                    location.createQuestionDialogue(I18n.SecretLab_PowerStation_DeliverPowerQuestionFull(), PowerBoxFactory.ManagePowerResponses(), PowerBoxFactory.deliverPowerQuestionKey);
                }
            }

            return true;
        }

        public GameLocation location()
        {
            return Game1.getLocationFromName(this.MapName);
        }

    }

}
