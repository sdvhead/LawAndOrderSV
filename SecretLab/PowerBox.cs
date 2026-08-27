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

        public int ID { get; set; }
        public string MapName { get; set; }

        public bool Powered { get; set; }

        private static readonly string BatteryId = "(O)787";
        public PowerBox(int x, int y, string mapName)
        {
            //x and y are the coordinates for the upper left corner of power box
            //this.ID = id;
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


        //private void SetSpriteOpen(int phase)
        //{
        //    int x1 = this.x;
        //    int x2 = this.x + 1;
        //    int y1 = this.y;
        //    int y2 = this.y + 1;

        //    int tile1 = 686;
        //    int tile2 = 687;
        //    int tile3 = 734;
        //    int tile4 = 735;

        //    GameLocation location = this.location();

        //    if (phase == 4)
        //    {
        //        location.removeMapTile(x1, y1, "Front");
        //        location.removeMapTile(x2, y1, "Front");
        //        location.removeMapTile(x1, y2, "Buildings");
        //        location.removeMapTile(x2, y2, "Buildings");
        //    }
        //    else
        //    {
        //        switch (phase)
        //        {
        //            case 1:
        //                Game1.playSound("cut");
        //                break;
        //            case 2:
        //                tile1 += 2;
        //                tile2 += 2;
        //                tile3 += 2;
        //                tile4 += 2;
        //                break;
        //            case 3:
        //                tile1 += 4;
        //                tile2 += 4;
        //                tile3 += 4;
        //                tile4 += 4;
        //                break;
        //        }
        //        location.setMapTile(x1, y1, tile1, "Front", "SecretLab");
        //        location.setMapTile(x2, y1, tile2, "Front", "SecretLab");
        //        location.setMapTile(x1, y2, tile3, "Buildings", "SecretLab");
        //        location.setMapTile(x2, y2, tile4, "Buildings", "SecretLab");
        //    }
        //}


        //private void SetSpriteClose(int phase)
        //{
        //    int x1 = this.x;
        //    int x2 = this.x + 1;
        //    int y1 = this.y;
        //    int y2 = this.y + 1;

        //    int tile1 = 686;
        //    int tile2 = 687;
        //    int tile3 = 734;
        //    int tile4 = 735;

        //    GameLocation location = this.location();

        //    switch (phase)
        //    {
        //        case 1:
        //            Game1.playSound("cut");
        //            break;
        //        case 2:
        //            tile1 -= 2;
        //            tile2 -= 2;
        //            tile3 -= 2;
        //            tile4 -= 2;
        //            break;
        //        case 3:
        //            tile1 -= 4;
        //            tile2 -= 4;
        //            tile3 -= 4;
        //            tile4 -= 4;
        //            break;
        //        case 4:
        //            tile1 -= 7;
        //            tile2 -= 7;
        //            tile3 -= 7;
        //            tile4 -= 7;
        //            break;
        //    }
        //    location.setMapTile(x1, y1, tile1, "Front", "SecretLab");
        //    location.setMapTile(x2, y1, tile2, "Front", "SecretLab");
        //    location.setMapTile(x1, y2, tile3, "Buildings", "SecretLab");
        //    location.setMapTile(x2, y2, tile4, "Buildings", "SecretLab");
        //}


    }

}
