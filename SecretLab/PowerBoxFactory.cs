using HarmonyLib;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace LawAndOrderSV.SecretLab
{
    internal static class PowerBoxFactory
    {

        internal static List<PowerBox> PowerBoxes = new List<PowerBox>();

        public static bool powered_lights = false;
        public static bool powered_elevator = false;
        public static bool powered_doorsbelts = false;
        public static bool powered_outlets = false;
        public static bool powered_comms = false;
        public static int unusedPowerPercent = 0;
        public static int totalPowerPercent = 0;

        private static readonly string BatteryId = "(O)787";
        public static string batteryQuestionKey = ModEntry.ModId + "_SecretLab_BatteryQuestionKey";
        public static string deliverPowerQuestionKey = ModEntry.ModId + "_SecretLab_DeliverPowerQuestionKey";

        private static void AddPowerBoxes()
        {
            //identify all power boxes on maps. coordinates are upper left corner of 2x3 box.
            PowerBoxes.Add(new PowerBox(67, 4, "sdvhead.LawAndOrderSV_SecretLab_Cove")); //far right of cove
            PowerBoxes.Add(new PowerBox(49, 30, "sdvhead.LawAndOrderSV_SecretLab_Warehouse")); //before entering factory floor
            PowerBoxes.Add(new PowerBox(2, 41, "sdvhead.LawAndOrderSV_SecretLab_Warehouse")); //south of elevator
        }

        public static void DailyReset()
        {
            PowerBoxes.Clear();
            powered_lights = false;
            powered_elevator = false;
            powered_doorsbelts = false;
            powered_outlets = false;
            powered_comms = false;
            unusedPowerPercent = 0;
            totalPowerPercent = 0;
            AddPowerBoxes();
        }

        internal static void Init()
        {
            GameLocation.RegisterTileAction("sdvhead.LawAndOrderSV_SecretLab_PowerStation_Interact", PowerBox_Interact);
            AddPowerBoxes();

            ModEntry.harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.answerDialogueAction)),
               prefix: new HarmonyMethod(typeof(PowerBoxFactory),
                                          nameof(PowerBoxFactory.AnswerPowerQuestion))
               );
        }



        public static List<PowerBox> GetPowerBoxes(string mapName)
        {
            List<PowerBox> pbs = new();
            for (int i = 0; i < PowerBoxes.Count; i++)
            {
                var pb = PowerBoxes[i];
                if(pb.MapName== mapName)
                {
                    pbs.Add(pb);
                }
            }
            return pbs;
        }

        private static bool PowerBox_Interact(GameLocation location, string[] args, Farmer farmer, Microsoft.Xna.Framework.Point point)
        {
            PowerBox? powerBox = FindClosestPowerBox(point, location);

            if (powerBox != null)
            {
                return powerBox.PowerMenu(location, args, farmer, point);
            }
            return true;
            
        }

        public static PowerBox? FindClosestPowerBox(Point point, GameLocation location)
        {
            int x = (int)point.X;
            int y = (int)point.Y;

            List<PowerBox> localPowerBoxes = new();
            for (var i = 0; i < PowerBoxes.Count; i++)
            {
                if (PowerBoxes[i].location() == location)
                {
                    localPowerBoxes.Add(PowerBoxes[i]);
                }
            }

            if (localPowerBoxes.Count < 1)
            {
                return null;
            }

            var closestPowerBox = localPowerBoxes[0];

            if (localPowerBoxes.Count > 1)
            {
                double closestDistance = Utils.GetDistance(x, y, closestPowerBox.X, closestPowerBox.Y);

                for (var i = 1; i < localPowerBoxes.Count; i++)
                {
                    var powerBox = localPowerBoxes[i];
                    double distance = Utils.GetDistance(x, y, powerBox.X, powerBox.Y);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPowerBox = powerBox;
                    }
                }
            }
            return closestPowerBox;
        }

        public static Response[] ManagePowerResponses()
        {
            List<Response> responses = new List<Response>();

            Response r1 = !powered_lights ? new Response("lightsOn", I18n.SecretLab_PowerStation_DeliverPowerAnswerLightsOn()) : new Response("lightsOff", I18n.SecretLab_PowerStation_DeliverPowerAnswerLightsOff());
            Response r2 = !powered_elevator ? new Response("elevatorOn", I18n.SecretLab_PowerStation_DeliverPowerAnswerElevatorOn()) : new Response("elevatorOff", I18n.SecretLab_PowerStation_DeliverPowerAnswerElevatorOff());
            Response r3 = !powered_doorsbelts ? new Response("doorsBeltsOn", I18n.SecretLab_PowerStation_DeliverPowerAnswerDoorsBeltsOn()) : new Response("doorsBeltsOff", I18n.SecretLab_PowerStation_DeliverPowerAnswerDoorsBeltsOff());
            Response r4 = !powered_outlets ? new Response("outletsOn", I18n.SecretLab_PowerStation_DeliverPowerAnswerOutletsOn()) : new Response("outletsOff", I18n.SecretLab_PowerStation_DeliverPowerAnswerOutletsOff());
            Response r5 = !powered_comms ? new Response("commsOn", I18n.SecretLab_PowerStation_DeliverPowerAnswerCommsOn()) : new Response("commsOff", I18n.SecretLab_PowerStation_DeliverPowerAnswerCommsOff());

            if (powered_lights || unusedPowerPercent > 0) responses.Add(r1);
            if (powered_elevator || unusedPowerPercent > 0) responses.Add(r2);
            if (powered_doorsbelts || unusedPowerPercent > 0) responses.Add(r3);
            if (powered_outlets || unusedPowerPercent > 0) responses.Add(r4);
            if (powered_comms || unusedPowerPercent > 0) responses.Add(r5);
            responses.Add(new Response("nothing", I18n.SecretLab_PowerStation_DeliverPowerAnswerNothing()));

            return responses.ToArray();
        }

        public static bool AnswerPowerQuestion(GameLocation __instance, string questionAndAnswer, string[] questionParams)
        {
            if (questionAndAnswer == batteryQuestionKey + "_Yes")
            {
                //player said yes to 'insert battery': consume the battery, increase station power, update the graphic.
                //then prompt for power control operations
                var powerBox = FindClosestPowerBox(Game1.player.TilePoint, Game1.currentLocation);
                if (powerBox != null)
                {
                    powerBox.Powered = true;
                    totalPowerPercent += 20;
                    unusedPowerPercent += 20;
                    Game1.player.Items.ReduceId(BatteryId,1);
                    AssetManager.ResetMaps();

                    DelayedAction.functionAfterDelay(() =>
                    {
                        Game1.currentLocation.createQuestionDialogue(I18n.SecretLab_PowerStation_DeliverPowerQuestionAvailable(), ManagePowerResponses(), deliverPowerQuestionKey);
                    }, 200);
                }


            }
            else if (questionAndAnswer == deliverPowerQuestionKey + "_lightsOn")
            {
                powered_lights = true;
                unusedPowerPercent -= 20;
                AssetManager.ResetMaps();
            }
            else if (questionAndAnswer == deliverPowerQuestionKey + "_lightsOff")
            {
                powered_lights = false;
                unusedPowerPercent += 20;
                AssetManager.ResetMaps();
            }
            else if (questionAndAnswer == deliverPowerQuestionKey + "_doorsBeltsOn")
            {
                powered_doorsbelts = true;
                unusedPowerPercent -= 20;
                AssetManager.ResetMaps();
            }
            else if (questionAndAnswer == deliverPowerQuestionKey + "_doorsBeltsOff")
            {
                powered_doorsbelts = false;
                unusedPowerPercent += 20;
                AssetManager.ResetMaps();
            }
            else if (questionAndAnswer == deliverPowerQuestionKey + "_commsOn")
            {
                powered_comms = true;
                unusedPowerPercent -= 20;
                AssetManager.ResetMaps();
            }
            else if (questionAndAnswer == deliverPowerQuestionKey + "_commsOff")
            {
                powered_comms = false;
                unusedPowerPercent += 20;
                AssetManager.ResetMaps();
            }
            else if (questionAndAnswer == deliverPowerQuestionKey + "_elevatorOn")
            {
                powered_elevator=true;
                unusedPowerPercent -= 20;
                AssetManager.ResetMaps();
            }
            else if (questionAndAnswer == deliverPowerQuestionKey + "_elevatorOff")
            {
                powered_elevator = false;
                unusedPowerPercent += 20;
                AssetManager.ResetMaps();
            }
            else if (questionAndAnswer == deliverPowerQuestionKey + "_outletsOn")
            {
                powered_outlets = true;
                unusedPowerPercent -= 20;
                AssetManager.ResetMaps();
            }
            else if (questionAndAnswer == deliverPowerQuestionKey + "_outletsOff")
            {
                powered_outlets = false;
                unusedPowerPercent += 20;
                AssetManager.ResetMaps();
            }
            return true;
        }

    }
}
