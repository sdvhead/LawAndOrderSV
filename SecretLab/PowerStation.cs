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
using Object = StardewValley.Object;


namespace LawAndOrderSV.SecretLab
{
    public static class PowerStation
    {
        internal const string QuestionKey_UseBattery = ModEntry.ModId + "_SecretLab_PowerStation_UseBattery";
        internal const string BatteryId = "(O)787";
        internal const string InsertBatteryQuestion = "This power station needs a battery to function. Insert a battery?";
        internal const string DeliverPowerQuestion = "Power Station capacity is available.";

        internal static string DeliverPowerAnswerLightsOn = "Activate lights";
        internal static string DeliverPowerAnswerElevatorOn = "Activate elevators";
        internal static string DeliverPowerAnswerConveyorOn = "Activate conveyor belts";
        internal static string DeliverPowerAnswerDoorsOn = "Activate doors";
        internal static string DeliverPowerAnswerOutletsOn = "Activate outlets";

        internal static string DeliverPowerAnswerLightsOff = "Deactivate lights";
        internal static string DeliverPowerAnswerElevatorOff = "Deactivate elevators";
        internal static string DeliverPowerAnswerConveyorOff = "Deactivate conveyor belts";
        internal static string DeliverPowerAnswerDoorsOff = "Deactivate doors";
        internal static string DeliverPowerAnswerOutletsOff = "Deactivate outlets";

        internal const string DeliverPowerAnswerNothing = "Exit";


        public static string station1_tileActionID = ModEntry.ModId + "_SecretLab_PowerStation1_Interact";
        public static string station1_batteryQuestionKey = ModEntry.ModId + "_SecretLab_PowerStation1_BatteryQuestionKey";

        public static bool station1_powered = false;
        public static bool powered_lights = false;
        public static bool powered_doors = false;
        public static bool powered_elevator = false;
        public static bool powered_conveyor = false;
        public static bool powered_outlets = false;

        public static string deliverPowerQuestionKey = ModEntry.ModId + "_SecretLab_DeliverPowerQuestionKey";

        



        internal static void Init()
        {
            //GameLocation.RegisterTouchAction(ModEntry.ModId+"_PowerStationInteract", Station1_Interact);
            //GameLocation.RegisterTileAction(ModEntry.ModId + "_PowerStationInteract", Station1_Interact);

            //RegisterTileAndTouch((ModEntry.ModId + "_PowerStationInteract", TileShowShipping);
            //GameLocation.RegisterTouchAction("")
            GameLocation.RegisterTileAction(station1_tileActionID, PowerStation1_Interact);

            ModEntry.harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.answerDialogueAction)),
               prefix: new HarmonyMethod(typeof(PowerStation),
                                          nameof(PowerStation.LawAndOrderSV_answerDialogue))
               );

        }

        public static Response[] createResponses(string responseType)
        {
            if (responseType == "managePower")
            {
                Response r1 = powered_lights ? new Response("lights", DeliverPowerAnswerLightsOn) : new Response("lights", DeliverPowerAnswerLightsOn);
                Response r2 = powered_elevator ? new Response("elevator", DeliverPowerAnswerElevatorOn) : new Response("elevator", DeliverPowerAnswerElevatorOff);
                Response r3 = powered_conveyor ? new Response("conveyor", DeliverPowerAnswerConveyorOn) : new Response("conveyor", DeliverPowerAnswerConveyorOff);
                Response r4 = powered_doors ? new Response("doors", DeliverPowerAnswerDoorsOn) : new Response("doors", DeliverPowerAnswerDoorsOff);
                Response r5 = powered_outlets ? new Response("outlets", DeliverPowerAnswerOutletsOn) : new Response("outlets", DeliverPowerAnswerOutletsOff);

                return new Response[6]
                {     
                    r1,r2,r3,r4,r5, new Response("nothing", DeliverPowerAnswerNothing)
                };
            }
            return Game1.currentLocation.createYesNoResponses();
        }

        public static bool LawAndOrderSV_answerDialogue(GameLocation __instance, string questionAndAnswer, string[] questionParams)
        {
            if (questionAndAnswer == (station1_batteryQuestionKey + "_Yes")){
                station1_powered = true;
                Game1.player.Items.ReduceId(BatteryId, 1);
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Cove");
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Main");
                //Game1.currentLocation.reloadMap();

                DelayedAction.functionAfterDelay(() => {
                    Game1.currentLocation.createQuestionDialogue(DeliverPowerQuestion, createResponses("managePower"), deliverPowerQuestionKey);
                },200);


            }else if (questionAndAnswer == deliverPowerQuestionKey + "_lights")
            {
                ModEntry.Log("turn on lights");
                powered_lights = true;
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Cove");
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Main");

                //Game1.currentLocation.map.Properties["AmbientLight"] = "95 95 95";
                //Game1.ambientLight = Game1.currentLocation.getAmbientLightForMap();

                //Game1.ambientLight = new Color(95, 95, 95);
                //Game1.currentLocation.map.Properties["AmbientLight"] = "95 95 95";
                Game1.currentLocation.resetForPlayerEntry();
                //Game1.outdoorLight = new Color(95, 95, 95);
                //Game1.currentLocation.updateMap();

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
            ModEntry.Log("Station1_Interact");
            if (!station1_powered)
            {
                ModEntry.Log("Station is not powered");
                if (playerHasBattery())
                {
                    ModEntry.Log("player has battery");

                    location.createQuestionDialogue(InsertBatteryQuestion, location.createYesNoResponses(), station1_batteryQuestionKey);



                    //Game1.drawObjectQuestionDialogue(InsertBatteryQuestion,location.createYesNoResponses());

                    /*
                    location.createQuestionDialogue(InsertBatteryQuestion, location.createYesNoResponses(), "ShrineOfSkullChallenge");
                    Response[] yesno = location.createYesNoResponses();

                    Game1.playSound("openBox");
                    who.reduceActiveItemByOne();
                    Game1.player.CanMove = false;
                    playSound("openBox");
                    DelayedAction.playSoundAfterDelay("doorCreakReverse", 500);
                    Game1.player.mailReceived.Add("TH_Tunnel");
                    Game1.multipleDialogues(new string[2]
                    {
                        Game1.content.LoadString("Strings\\Locations:Tunnel_TunnelSafe_ConsumeBattery"),
                        Game1.content.LoadString("Strings\\Locations:Tunnel_TunnelSafe_MrQiNote")
                    });
                    Game1.player.addQuest("2");

                    Game1.player.holdUpItemThenMessage(ItemRegistry.Create(BatteryId));
                    Game1.player.removeFirstOfThisItemFromInventory(BatteryId);
                    Game1.player.hasSpecialCharm = true;
                    Game1.player.mailReceived.Add("SecretNote20_done");

                    */
                }
            }

            return true;
        }

    }
}
