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
        internal const string DeliverPowerQuestion = "Power Station capacity is available. Supply power to facility?";
        internal const string DeliverPowerAnswerLights = "Activate lights.";
        internal const string DeliverPowerAnswerElevator = "Activate elevator.";
        internal const string DeliverPowerAnswerConveyor = "Activate conveyor belt.";


        public static string station1_tileActionID = ModEntry.ModId + "_SecretLab_PowerStation1_Interact";
        public static string station1_batteryQuestionKey = ModEntry.ModId + "_SecretLab_PowerStation1_BatteryQuestionKey";
        public static bool station1_powered = false;

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
                return new Response[2]
                {
                    new Response("lights", DeliverPowerAnswerLights),
                    new Response("elevator", DeliverPowerAnswerElevator)
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
                //Game1.currentLocation.reloadMap();

                DelayedAction.functionAfterDelay(() => {
                    Game1.currentLocation.createQuestionDialogue(DeliverPowerQuestion, createResponses("managePower"), deliverPowerQuestionKey);
                },200);


            }else if (questionAndAnswer == deliverPowerQuestionKey + "_lights")
            {
                ModEntry.Log("turn on lights");
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
