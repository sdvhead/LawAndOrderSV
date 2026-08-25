using HarmonyLib;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Delegates;
using StardewValley.Extensions;
using StardewValley.Inventories;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Tools;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;
using Object = StardewValley.Object;



namespace LawAndOrderSV.SecretLab
{
    //contains logic for dealing with power stations as well as other interactable objects in the secret lab maps
    public static class PowerStation
    {
        internal static readonly string QuestionKey_UseBattery = ModEntry.ModId + "_SecretLab_PowerStation_UseBattery";
        internal static readonly string BatteryId = "(O)787";
        internal static readonly string InsertBatteryQuestion = I18n.SecretLab_PowerStation_InsertBatteryQuestion();

        internal static readonly string DeliverPowerAnswerLightsOn = I18n.SecretLab_PowerStation_DeliverPowerAnswerLightsOn();
        internal static readonly string DeliverPowerAnswerElevatorOn = I18n.SecretLab_PowerStation_DeliverPowerAnswerElevatorOn();
        //internal static readonly string DeliverPowerAnswerConveyorOn = I18n.SecretLab_PowerStation_DeliverPowerAnswerConveyorOn();
        internal static readonly string DeliverPowerAnswerDoorsBeltsOn = I18n.SecretLab_PowerStation_DeliverPowerAnswerDoorsBeltsOn();
        internal static readonly string DeliverPowerAnswerOutletsOn = I18n.SecretLab_PowerStation_DeliverPowerAnswerOutletsOn();
        internal static readonly string DeliverPowerAnswerCommsOn = I18n.SecretLab_PowerStation_DeliverPowerAnswerCommsOn();

        internal static readonly string DeliverPowerAnswerLightsOff = I18n.SecretLab_PowerStation_DeliverPowerAnswerLightsOff();
        internal static readonly string DeliverPowerAnswerElevatorOff = I18n.SecretLab_PowerStation_DeliverPowerAnswerElevatorOff();
        //internal static readonly string DeliverPowerAnswerConveyorOff = I18n.SecretLab_PowerStation_DeliverPowerAnswerConveyorOff();
        internal static readonly string DeliverPowerAnswerDoorsBeltsOff = I18n.SecretLab_PowerStation_DeliverPowerAnswerDoorsBeltsOff();
        internal static readonly string DeliverPowerAnswerOutletsOff = I18n.SecretLab_PowerStation_DeliverPowerAnswerOutletsOff();
        internal static readonly string DeliverPowerAnswerCommsOff = I18n.SecretLab_PowerStation_DeliverPowerAnswerCommsOff();

        internal static readonly string DeliverPowerAnswerNothing = I18n.SecretLab_PowerStation_DeliverPowerAnswerNothing();


        public static readonly string station1_tileActionID = ModEntry.ModId + "_SecretLab_PowerStation1_Interact";
        public static readonly string station2_tileActionID = ModEntry.ModId + "_SecretLab_PowerStation2_Interact";
        //public static readonly string conveyorSwitch_TileActionID = ModEntry.ModId + "_SecretLab_ConveyorSwitch_Interact";
        public static readonly string station1_batteryQuestionKey = ModEntry.ModId + "_SecretLab_PowerStation1_BatteryQuestionKey";
        public static readonly string station2_batteryQuestionKey = ModEntry.ModId + "_SecretLab_PowerStation2_BatteryQuestionKey";

        //should change these to methods that get/set host mail flags to support multiplayer
        public static bool station1_powered = false;
        public static bool station2_powered = false;
        public static bool powered_lights = false;
        public static bool powered_doorsbelts = false;
        public static bool powered_elevator = false;
        public static bool powered_comms = false;
        public static bool powered_outlets = false;

        public static int totalPowerPercent = 0;
        public static int unusedPowerPercent = 0;
        public static string deliverPowerQuestionKey = ModEntry.ModId + "_SecretLab_DeliverPowerQuestionKey";



        public struct TileInfo
        {
            public Microsoft.Xna.Framework.Vector2 Coordinates { get; set; }
            public int TileId { get; set; }

            public TileInfo(Microsoft.Xna.Framework.Vector2 coords, int tileId)
            {
                Coordinates = coords;
                TileId = tileId;
            }
        }

        /*
        public static List<TileInfo> GetAdjacentConveyorTiles(
            GameLocation location,
            string layerName,
            string tilesheetName,
            Point point,
            int[] matchingTiles)
        {
            List<TileInfo> results = new List<TileInfo>();

            // 1. Locate the specified layer using location.Map
            //Layer layer = location?.Map?.GetLayer(layerName);
            Map map = location.Map;
            if (map == null) return results;
            
            Layer layer = map.GetLayer(layerName);
            ModEntry.Log("layer is " + layer?.Id);
            if (layer == null) return results;

            // Boundary check for starting tile
            if (point.X < 0 || point.X >= layer.LayerWidth || point.Y < 0 || point.Y >= layer.LayerHeight)
            {
                return results;
            }

            // 3. Initialize BFS tracking collections to prevent infinite loops
            Queue<Point> tilesToVisit = new Queue<Point>();
            HashSet<Point> visitedTiles = new HashSet<Point>();

            // Start tracking from the initial point
            tilesToVisit.Enqueue(point);
            visitedTiles.Add(point);

            // Define 4-directional offsets for neighbors (Up, Down, Left, Right)
            Point[] directions = new Point[]
            {
                new Point(0, -1),
                new Point(0, 1),
                new Point(-1, 0),
                new Point(1, 0)
            };

            // 4. Scan neighbors
            while (tilesToVisit.Count > 0)
            {
                Point currentPoint = tilesToVisit.Dequeue();
                Tile currentTile = layer.Tiles[currentPoint.X, currentPoint.Y];

                if (currentTile != null && currentTile.TileSheet.Id == tilesheetName)
                {
                    // Add the valid tile to our results
                    results.Add(new TileInfo(
                        new Vector2(currentPoint.X, currentPoint.Y),
                        currentTile.TileIndex
                    ));
                }

                    // Check all 4 adjacent neighbors
                    foreach (Point dir in directions)
                    {
                        Point neighborPoint = new Point(currentPoint.X + dir.X, currentPoint.Y + dir.Y);

                        // Skip if we already evaluated this tile
                        if (visitedTiles.Contains(neighborPoint)) continue;

                        // Enforce layer grid boundaries to avoid IndexOutOfRangeException
                        if (neighborPoint.X < 0 || neighborPoint.X >= layer.LayerWidth ||
                            neighborPoint.Y < 0 || neighborPoint.Y >= layer.LayerHeight)
                        {
                            continue;
                        }

                        Tile neighborTile = layer.Tiles[neighborPoint.X, neighborPoint.Y];

                        // Verify neighbor exists, belongs to the correct sheet, and matches allowed IDs
                        if (neighborTile != null &&
                            neighborTile.TileSheet.Id == tilesheetName &&
                            matchingTiles.Contains(neighborTile.TileIndex))
                        {
                            visitedTiles.Add(neighborPoint);
                            tilesToVisit.Enqueue(neighborPoint);
                        }
                    }
            }

            return results;
        }
        */

        internal static void Init()
        {
            ModEntry.Log("PowerStation init: registering interacts");
            GameLocation.RegisterTileAction(station1_tileActionID, PowerStation1_Interact);
            GameLocation.RegisterTileAction(station2_tileActionID, PowerStation2_Interact);
            //GameLocation.RegisterTileAction(conveyorSwitch_TileActionID, ConveyorSwitch_Interact);
            

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
                //Response r3 = !powered_conveyor ? new Response("conveyorOn", DeliverPowerAnswerConveyorOn) : new Response("conveyorOff", DeliverPowerAnswerConveyorOff);
                Response r3 = !powered_doorsbelts ? new Response("doorsBeltsOn", DeliverPowerAnswerDoorsBeltsOn) : new Response("doorsBeltsOff", DeliverPowerAnswerDoorsBeltsOff);
                Response r4 = !powered_outlets ? new Response("outletsOn", DeliverPowerAnswerOutletsOn) : new Response("outletsOff", DeliverPowerAnswerOutletsOff);
                Response r5 = !powered_comms ? new Response("commsOn", DeliverPowerAnswerCommsOn) : new Response("commsOff", DeliverPowerAnswerCommsOff);

                if (powered_lights || unusedPowerPercent > 0) responses.Add(r1);
                if (powered_elevator || unusedPowerPercent > 0) responses.Add(r2);
                if (powered_doorsbelts || unusedPowerPercent > 0) responses.Add(r3);
                if (powered_outlets || unusedPowerPercent > 0) responses.Add(r4);
                if (powered_comms || unusedPowerPercent > 0) responses.Add(r5);
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
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Warehouse");

                DelayedAction.functionAfterDelay(() => {
                    Game1.currentLocation.createQuestionDialogue(I18n.SecretLab_PowerStation_DeliverPowerQuestionAvailable(), createPowerStationResponses("managePower"), deliverPowerQuestionKey);
                },200);
            }
            else if (questionAndAnswer == (station2_batteryQuestionKey + "_Yes"))
            {
                station2_powered = true;
                totalPowerPercent += 20;
                unusedPowerPercent += 20;
                Game1.player.Items.ReduceId(BatteryId, 1);
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Cove");
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Main");
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Warehouse");

                DelayedAction.functionAfterDelay(() => {
                    Game1.currentLocation.createQuestionDialogue(I18n.SecretLab_PowerStation_DeliverPowerQuestionAvailable(), createPowerStationResponses("managePower"), deliverPowerQuestionKey);
                }, 200);
            }
            else if (questionAndAnswer == deliverPowerQuestionKey + "_lightsOn")
            {
                //player said to turn on lights
                ModEntry.Log("turn on lights");
                powered_lights = true;
                unusedPowerPercent -= 20;
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Cove");
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Main");
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Warehouse");
                Game1.currentLocation.resetForPlayerEntry();
            }
            else if (questionAndAnswer == deliverPowerQuestionKey + "_lightsOff")
            {
                //player said to turn off lights
                powered_lights = false ;
                unusedPowerPercent += 20;
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Cove");
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Main");
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Warehouse");
                Game1.currentLocation.resetForPlayerEntry();
            }
            else if (questionAndAnswer == deliverPowerQuestionKey + "_doorsBeltsOn")
            {
                ModEntry.Log("turn on conveyor");
                powered_doorsbelts = true;
                unusedPowerPercent -= 20;
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Cove");
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Main");
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Warehouse");
                Game1.currentLocation.resetForPlayerEntry();
            }
            else if (questionAndAnswer == deliverPowerQuestionKey + "_doorsBeltsOff")
            {
                powered_doorsbelts = false;
                unusedPowerPercent += 20;
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Cove");
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Main");
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Warehouse");
                Game1.currentLocation.resetForPlayerEntry();
            }
            else if (questionAndAnswer == deliverPowerQuestionKey + "_commsOn")
            {
                powered_comms = true;
                unusedPowerPercent -= 20;
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Cove");
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Main");
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Warehouse");
                Game1.currentLocation.resetForPlayerEntry();
            }
            else if (questionAndAnswer == deliverPowerQuestionKey + "_commsOff")
            {
                powered_comms = false;
                unusedPowerPercent += 20;
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Cove");
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Main");
                ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Warehouse");
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
        private static bool PlayerHasBattery()
        {
            if (Game1.player.Items.ContainsId(BatteryId, 1)) return true;
            return false;
        }




        /*
        private static bool ConveyorSwitch_Interact(GameLocation location, string[] args, Farmer farmer, Microsoft.Xna.Framework.Point point)
        {
            int[] conveyorTiles = { 10, 11, 12, 13 }; //N, S, E, W

            List <TileInfo> infoList = GetAdjacentConveyorTiles(location, "Back", "SecretLab", point, conveyorTiles);
            foreach (TileInfo info in infoList)
            {
                ModEntry.Log("tile: " + info.Coordinates.X + "," + info.Coordinates.Y + "):" + info.TileId);
                int tileX = (int)info.Coordinates.X;
                int tileY = (int)info.Coordinates.Y;
                int newBackTileId = -1;
                int newBack2TileId = -1;
                switch (info.TileId)
                {
                    case 10: newBackTileId = 11; newBack2TileId = 15; break;//N to S
                    case 11: newBackTileId = 10; newBack2TileId = 14; break;//S to N
                    case 12: newBackTileId = 13; newBack2TileId = 17; break;//E to W
                    case 13: newBackTileId = 12; newBack2TileId = 16; break;//W to E
                }
                if (newBackTileId != -1 && newBack2TileId != -1)
                {
                    location.setMapTile(tileX, tileY, newBackTileId, "Back", "SecretLab");
                    location.setMapTile(tileX, tileY, newBack2TileId, "Back2", "SecretLab");
                }
            }
            ModEntry.imh.GameContent.InvalidateCache(location.Map.assetPath);
            ModEntry.Log("invalidated " + location.Map.assetPath);
            //ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Cove");
            //ModEntry.imh.GameContent.InvalidateCache("Maps/sdvhead.LawAndOrderSV_SecretLab_Main");
            Game1.currentLocation.resetForPlayerEntry();

            return true;
        }
        */

        private static bool PowerStation1_Interact(GameLocation location, string[] args, Farmer farmer, Microsoft.Xna.Framework.Point point)
        {
            if (!station1_powered)
            {
                if (PlayerHasBattery())
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
        private static bool PowerStation2_Interact(GameLocation location, string[] args, Farmer farmer, Microsoft.Xna.Framework.Point point)
        {
            if (!station2_powered)
            {
                if (PlayerHasBattery())
                {
                    location.createQuestionDialogue(InsertBatteryQuestion, location.createYesNoResponses(), station2_batteryQuestionKey);
                }
                else
                {
                    Game1.drawObjectDialogue(I18n.SecretLab_PowerStation_NoBatteryInventory());
                }
            }
            else
            {

                if (unusedPowerPercent > 0)
                { //unused power is available
                    Game1.currentLocation.createQuestionDialogue(I18n.SecretLab_PowerStation_DeliverPowerQuestionAvailable(), createPowerStationResponses("managePower"), deliverPowerQuestionKey);
                }
                else if (totalPowerPercent >= 100)
                { //all stations are online
                    Game1.currentLocation.createQuestionDialogue(I18n.SecretLab_PowerStation_DeliverPowerQuestionMax(), createPowerStationResponses("managePower"), deliverPowerQuestionKey);
                }
                else
                { //all available power is being used
                    Game1.currentLocation.createQuestionDialogue(I18n.SecretLab_PowerStation_DeliverPowerQuestionFull(), createPowerStationResponses("managePower"), deliverPowerQuestionKey);
                }
            }

            return true;
        }

    }
}
