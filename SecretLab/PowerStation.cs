using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Delegates;
using StardewValley.Extensions;
using StardewValley.Menus;
using StardewValley.Objects;
using System.Runtime.CompilerServices;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;
using Netcode;
using StardewModdingAPI.Utilities;
using StardewValley.Inventories;
using StardewValley.Network;
using StardewValley.Tools;
using Object = StardewValley.Object;
using System.Reflection.Metadata;



namespace LawAndOrderSV.SecretLab
{
    public static class PowerStation
    {
        internal static readonly string QuestionKey_UseBattery = ModEntry.ModId + "_SecretLab_PowerStation_UseBattery";
        internal static readonly string BatteryId = "(O)787";
        internal static readonly string InsertBatteryQuestion = I18n.SecretLab_PowerStation_InsertBatteryQuestion();

        internal static readonly string DeliverPowerAnswerLightsOn = I18n.SecretLab_PowerStation_DeliverPowerAnswerLightsOn();
        internal static readonly string DeliverPowerAnswerElevatorOn = I18n.SecretLab_PowerStation_DeliverPowerAnswerElevatorOn();
        internal static readonly string DeliverPowerAnswerConveyorOn = I18n.SecretLab_PowerStation_DeliverPowerAnswerConveyorOn();
        internal static readonly string DeliverPowerAnswerDoorsOn = I18n.SecretLab_PowerStation_DeliverPowerAnswerDoorsOn();
        internal static readonly string DeliverPowerAnswerOutletsOn = I18n.SecretLab_PowerStation_DeliverPowerAnswerOutletsOn();

        internal static readonly string DeliverPowerAnswerLightsOff = I18n.SecretLab_PowerStation_DeliverPowerAnswerLightsOff();
        internal static readonly string DeliverPowerAnswerElevatorOff = I18n.SecretLab_PowerStation_DeliverPowerAnswerElevatorOff();
        internal static readonly string DeliverPowerAnswerConveyorOff = I18n.SecretLab_PowerStation_DeliverPowerAnswerConveyorOff();
        internal static readonly string DeliverPowerAnswerDoorsOff = I18n.SecretLab_PowerStation_DeliverPowerAnswerDoorsOff();
        internal static readonly string DeliverPowerAnswerOutletsOff = I18n.SecretLab_PowerStation_DeliverPowerAnswerOutletsOff();

        internal static readonly string DeliverPowerAnswerNothing = I18n.SecretLab_PowerStation_DeliverPowerAnswerNothing();


        public static readonly string station1_tileActionID = ModEntry.ModId + "_SecretLab_PowerStation1_Interact";
        public static readonly string station1_batteryQuestionKey = ModEntry.ModId + "_SecretLab_PowerStation1_BatteryQuestionKey";

        public static bool station1_powered = false;
        public static bool powered_lights = false;
        public static bool powered_doors = false;
        public static bool powered_elevator = false;
        public static bool powered_conveyor = false;
        public static bool powered_outlets = false;

        public static int totalPowerPercent = 0;
        public static int unusedPowerPercent = 0;
        public static string deliverPowerQuestionKey = ModEntry.ModId + "_SecretLab_DeliverPowerQuestionKey";


    internal static void Init()
        {
            GameLocation.RegisterTileAction(station1_tileActionID, PowerStation1_Interact);

            ModEntry.harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.answerDialogueAction)),
               prefix: new HarmonyMethod(typeof(PowerStation),
                                          nameof(PowerStation.LawAndOrderSV_answerDialogue))
               );

        }

        public static Response[] createPowerStationResponses(string responseType)
        {
            List<Response> responses = new List<Response>();

            if (responseType == "managePower")
            {
                Response r1 = !powered_lights ? new Response("lightsOn", DeliverPowerAnswerLightsOn) : new Response("lightsOff", DeliverPowerAnswerLightsOff);
                Response r2 = !powered_elevator ? new Response("elevatorOn", DeliverPowerAnswerElevatorOn) : new Response("elevatorOff", DeliverPowerAnswerElevatorOff);
                Response r3 = !powered_conveyor ? new Response("conveyorOn", DeliverPowerAnswerConveyorOn) : new Response("conveyorOff", DeliverPowerAnswerConveyorOff);
                Response r4 = !powered_doors ? new Response("doorsOn", DeliverPowerAnswerDoorsOn) : new Response("doorsOff", DeliverPowerAnswerDoorsOff);
                Response r5 = !powered_outlets ? new Response("outletsOn", DeliverPowerAnswerOutletsOn) : new Response("outletsOff", DeliverPowerAnswerOutletsOff);

                if (powered_lights || unusedPowerPercent > 0) responses.Add(r1);
                if (powered_elevator || unusedPowerPercent > 0) responses.Add(r2);
                if (powered_conveyor || unusedPowerPercent > 0) responses.Add(r3);
                if (powered_doors || unusedPowerPercent > 0) responses.Add(r4);
                if (powered_outlets || unusedPowerPercent > 0) responses.Add(r5);
                responses.Add(new Response("nothing", DeliverPowerAnswerNothing));

                return responses.ToArray();
                /*
                return new Response[6]
                {     
                    r1,r2,r3,r4,r5, new Response("nothing", DeliverPowerAnswerNothing)
                };
                */
            }
            return Game1.currentLocation.createYesNoResponses();
        }

        public static bool LawAndOrderSV_answerDialogue(GameLocation __instance, string questionAndAnswer, string[] questionParams)
        {
            ModEntry.Log("interact answer: " + questionAndAnswer);
            if (questionAndAnswer == (station1_batteryQuestionKey + "_Yes")){
                //player said 'yes' to 'insert battery':
                //consume the player's battery, increase the station power, and update the power station graphic.
                //Then prompt for power control operations

                station1_powered = true;
                totalPowerPercent += 20;
                unusedPowerPercent += 20;
                Game1.player.Items.ReduceId(BatteryId, 1);
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Cove");
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Main");

                DelayedAction.functionAfterDelay(() => {
                    Game1.currentLocation.createQuestionDialogue(I18n.SecretLab_PowerStation_DeliverPowerQuestionAvailable(), createPowerStationResponses("managePower"), deliverPowerQuestionKey);
                },200);
            }else if (questionAndAnswer == deliverPowerQuestionKey + "_lightsOn"){
                //player said to turn on lights
                ModEntry.Log("turn on lights");
                powered_lights = true;
                unusedPowerPercent -= 20;
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Cove");
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Main");
                Game1.currentLocation.resetForPlayerEntry();
            }
            else if (questionAndAnswer == deliverPowerQuestionKey + "_lightsOff")
            {
                //player said to turn off lights
                powered_lights = false ;
                unusedPowerPercent += 20;
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Cove");
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Main");
                Game1.currentLocation.resetForPlayerEntry();
            }
            else if (questionAndAnswer == deliverPowerQuestionKey + "_conveyorOn")
            {
                ModEntry.Log("turn on conveyor");
                powered_conveyor = true;
                unusedPowerPercent -= 20;
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Cove");
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Main");
                Game1.currentLocation.resetForPlayerEntry();
            }
            else if (questionAndAnswer == deliverPowerQuestionKey + "_conveyorOff")
            {
                powered_conveyor = false;
                unusedPowerPercent += 20;
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Cove");
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Main");
                Game1.currentLocation.resetForPlayerEntry();
            }

            //ModEntry.Log("function called (" + questionAndAnswer + ")");
            //ModEntry.Log("question params (" + questionParams.ToString() + ")");

            return true;
        }


        /*
        [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.answerDialogueAction))]
        public class AnswerDialogueActionPatch
        {
            public static bool Prefix(GameLocation __instance, string questionAndAnswer, string[] questionParams)
            {
                ModEntry.Log("function called ("+questionAndAnswer+")");
                // Check if the dialogue response matches our custom action key
                if (questionAndAnswer == QuestionKey_UseBattery)
                {
                    // Do something when the player clicks "Yes"
                    ModEntry.Log("player answered yes");
                    //Game1.player.addMana(50); // hypothetical action
                    //Game1.drawDialogue(Game1.getCharacterFromName("Lewis"), "Thanks for your help!");

                    // Return false to skip original execution if fully handled
                    return false;
                }

                // Let the game process vanilla dialogue actions normally
                return true;
            }
        }
        */
        private static bool playerHasBattery()
        {
            if (Game1.player.Items.ContainsId(BatteryId, 1)) return true;
            return false;
        }

        private static bool PowerStation1_Interact(GameLocation location, string[] args, Farmer farmer, Microsoft.Xna.Framework.Point point)
        {
            if (!station1_powered)
            {
                if (playerHasBattery())
                {
                    location.createQuestionDialogue(InsertBatteryQuestion, location.createYesNoResponses(), station1_batteryQuestionKey);
                }
                else
                {
                    Game1.drawObjectDialogue(I18n.SecretLab_PowerStation_NoBatteryInventory());
                }
            }
            else
            {

                if (unusedPowerPercent > 0){ //unused power is available
                    Game1.currentLocation.createQuestionDialogue(I18n.SecretLab_PowerStation_DeliverPowerQuestionAvailable(), createPowerStationResponses("managePower"), deliverPowerQuestionKey);
                }
                else if (totalPowerPercent >= 100){ //all stations are online
                    Game1.currentLocation.createQuestionDialogue(I18n.SecretLab_PowerStation_DeliverPowerQuestionMax(), createPowerStationResponses("managePower"), deliverPowerQuestionKey);
                } else { //all available power is being used
                    Game1.currentLocation.createQuestionDialogue(I18n.SecretLab_PowerStation_DeliverPowerQuestionFull(), createPowerStationResponses("managePower"), deliverPowerQuestionKey);
                }
            }

                return true;
        }

    }
}
