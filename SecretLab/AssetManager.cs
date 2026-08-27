using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Menus;
using System.IO;
using System.Runtime.CompilerServices;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;
using Object = StardewValley.Object;

namespace LawAndOrderSV.SecretLab
{
    internal static class AssetManager
    {
        internal static readonly string mailflag_sawDemetriusArrest = ModEntry.ModId + "_SawDemetriusArrest";
        internal static readonly string mailflag_openedCoveDoor = ModEntry.ModId + "_OpenedCoveDoor";

        public static readonly string coveDoor_tileActionID = ModEntry.ModId + "_SecretLab_CoveDoor_Interact";
        internal static readonly string coveDoor_lockedMessage = I18n.SecretLab_CoveDoor_LockedMessage();

        public static readonly string locationName_secretLab_cove = ModEntry.ModId + "_SecretLab_Cove";

        public static bool coveDoorUnlocked = false;

        public static Vector2[] coveLightCoords = new Vector2[] { new Vector2(6, 3), new Vector2(11, 3), new Vector2(19, 4), new Vector2(31, 4), new Vector2(39, 4), new Vector2(45, 8), new Vector2(51, 8), new Vector2(59, 8), new Vector2(63, 7), new Vector2(69, 3) };


        internal static void Init()
        {
            ModEntry.imh.Events.Content.AssetRequested += OnAssetRequested;
            ModEntry.imh.Events.GameLoop.DayStarted += OnDayStart;
            ModEntry.imh.Events.Player.Warped += OnPlayerWarped;
            GameLocation.RegisterTileAction(coveDoor_tileActionID, CoveDoor_Interact);
        }

        private static void OnPlayerWarped(object? sender, WarpedEventArgs e)
        {
            coveDoorUnlocked = Game1.MasterPlayer.mailReceived.Contains(mailflag_sawDemetriusArrest);
            //TEMP TESTING
            coveDoorUnlocked = true;
        }

        private static bool CoveDoor_Interact(GameLocation location, string[] args, Farmer farmer, Microsoft.Xna.Framework.Point point)
        {
            if (coveDoorUnlocked)
            {
                Game1.MasterPlayer.mailReceived.Add(mailflag_openedCoveDoor); //after entering once, stop spawning a barrel at the door each day
                Game1.player.completelyStopAnimatingOrDoingAction();
                Game1.playSound("doorClose");
                Game1.warpFarmer(locationName_secretLab_cove, 7, 13, 0);
                Game1.player.temporarilyInvincible = true;
                Game1.player.temporaryInvincibilityTimer = 0;
                Game1.player.flashDuringThisTemporaryInvincibility = false;
                Game1.player.currentTemporaryInvincibilityDuration = 1000;
            }
            else
            {
                Game1.drawObjectDialogue(coveDoor_lockedMessage);
            }

            return true;
        }

        private static void OnDayStart(object? sender, DayStartedEventArgs e)
        {
            if (!Game1.MasterPlayer.mailReceived.Contains(mailflag_openedCoveDoor))
            {
                AddBarrel(Game1.getLocationFromName("IslandSouthEastCave"));
            }
        }

        private static void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            //used for lights
            byte r = 95;
            byte g = 95;
            byte b = 95;

            string mapPath = e.Name.Name;
            int pos = mapPath.LastIndexOf("/") + 1;
            string mapFile = mapPath.Substring(pos, mapPath.Length - pos);

            if (mapFile.ContainsIgnoreCase(ModEntry.ModId) && mapFile.ContainsIgnoreCase("SecretLab"))
            {
                //handles cove, warehouse, etc

                ModEntry.Log("handling map:" +  mapFile);
                
                e.Edit(asset =>
                {
                    IAssetDataForMap editor = asset.AsMap();
                    Map mapdata = asset.AsMap().Data;

                    List<PowerBox> pbs = PowerBoxFactory.GetPowerBoxes(mapFile);
                    foreach(PowerBox pb in pbs)
                    {
                        if (pb.Powered)
                        {
                            Map powermap = ModEntry.imh.ModContent.Load<Map>(ModEntry.mapdir + "SecretLab_PowerStation.tmx");
                            editor.PatchMap(powermap, new Microsoft.Xna.Framework.Rectangle(1, 0, 1, 1), new Microsoft.Xna.Framework.Rectangle(pb.X, pb.Y + 2, 1, 1), PatchMapMode.ReplaceByLayer);
                            Microsoft.Xna.Framework.Rectangle rectLevel = new Microsoft.Xna.Framework.Rectangle(pb.X, pb.Y + 1, 1, 1);
                            switch (PowerBoxFactory.totalPowerPercent)
                            {
                                case 20:
                                    editor.PatchMap(powermap, new Microsoft.Xna.Framework.Rectangle(3, 0, 1, 1), rectLevel);
                                    break;
                                case 40:
                                    editor.PatchMap(powermap, new Microsoft.Xna.Framework.Rectangle(4, 0, 1, 1), rectLevel);
                                    break;
                                case 60:
                                    editor.PatchMap(powermap, new Microsoft.Xna.Framework.Rectangle(5, 0, 1, 1), rectLevel);
                                    break;
                                case 80:
                                    editor.PatchMap(powermap, new Microsoft.Xna.Framework.Rectangle(6, 0, 1, 1), rectLevel);
                                    break;
                                case 100:
                                    editor.PatchMap(powermap, new Microsoft.Xna.Framework.Rectangle(7, 0, 1, 1), rectLevel);
                                    break;
                            }
                        }
                    }
                    if (PowerBoxFactory.powered_lights)
                    {
                        mapdata.Properties["AmbientLight"] = $"{r} {g} {b}";
                        if (mapFile.ContainsIgnoreCase("Cove"))
                        {
                            mapdata.Properties["Light"] = string.Join(" ", Array.ConvertAll(coveLightCoords, v => $"{v.X} {v.Y} 4"));
                            for (int i = 0; i < coveLightCoords.Length; i++)
                            {
                                Vector2 coords = coveLightCoords[i];
                                Map lightmap = ModEntry.imh.ModContent.Load<Map>(ModEntry.mapdir + "SecretLab_CoveLightPatch.tmx");
                                editor.PatchMap(lightmap, null, new Microsoft.Xna.Framework.Rectangle((int)coords.X, (int)coords.Y, 1, 1), PatchMapMode.ReplaceByLayer);
                            }
                        }
                    }
                    if (PowerBoxFactory.powered_doorsbelts)
                    {
                        //doors
                        var doors = DoorFactory.GetDoors(mapFile);
                        Map doorpatch = ModEntry.imh.ModContent.Load<Map>(ModEntry.mapdir + "SecretLab_DoorPatch.tmx");
                        Microsoft.Xna.Framework.Rectangle doorPatchRect = new Microsoft.Xna.Framework.Rectangle(0, 0, 4, 3);
                        foreach (var door in doors)
                        {
                            editor.PatchMap(doorpatch, doorPatchRect, new Microsoft.Xna.Framework.Rectangle(door.X - 1, door.Y - 1, 4, 3), PatchMapMode.ReplaceByLayer);
                        }

                        //belts
                        Vector2[] NorthConveyors = GetSecretLabTileCoordinates(asset, "Back", "SecretLab", 10);
                        Vector2[] SouthConveyors = GetSecretLabTileCoordinates(asset, "Back", "SecretLab", 11);
                        Vector2[] EastConveyors = GetSecretLabTileCoordinates(asset, "Back", "SecretLab", 12);
                        Vector2[] WestConveyors = GetSecretLabTileCoordinates(asset, "Back", "SecretLab", 13);
                        Map conveyorpatch = ModEntry.imh.ModContent.Load<Map>(ModEntry.mapdir + "SecretLab_ConveyorPatch.tmx");

                        for (int i = 0; i < NorthConveyors.Length; i++)
                        {
                            editor.PatchMap(conveyorpatch, new Microsoft.Xna.Framework.Rectangle(0, 1, 1, 1), new Microsoft.Xna.Framework.Rectangle((int)NorthConveyors[i].X, (int)NorthConveyors[i].Y, 1, 1), PatchMapMode.ReplaceByLayer);
                        }
                        for (int i = 0; i < SouthConveyors.Length; i++)
                        {
                            editor.PatchMap(conveyorpatch, new Microsoft.Xna.Framework.Rectangle(1, 1, 1, 1), new Microsoft.Xna.Framework.Rectangle((int)SouthConveyors[i].X, (int)SouthConveyors[i].Y, 1, 1), PatchMapMode.ReplaceByLayer);
                        }
                        for (int i = 0; i < EastConveyors.Length; i++)
                        {
                            editor.PatchMap(conveyorpatch, new Microsoft.Xna.Framework.Rectangle(2, 1, 1, 1), new Microsoft.Xna.Framework.Rectangle((int)EastConveyors[i].X, (int)EastConveyors[i].Y, 1, 1), PatchMapMode.ReplaceByLayer);
                        }
                        for (int i = 0; i < WestConveyors.Length; i++)
                        {
                            editor.PatchMap(conveyorpatch, new Microsoft.Xna.Framework.Rectangle(3, 1, 1, 1), new Microsoft.Xna.Framework.Rectangle((int)WestConveyors[i].X, (int)WestConveyors[i].Y, 1, 1), PatchMapMode.ReplaceByLayer);
                        }
                        
                    }
                    if (PowerBoxFactory.powered_comms)
                    {

                        var comms = CommFactory.GetComms(mapFile);
                        Map commspatch = ModEntry.imh.ModContent.Load<Map>(ModEntry.mapdir + "SecretLab_CommsPatch_Speaker.tmx");
                        Microsoft.Xna.Framework.Rectangle commsPatchRect = new Microsoft.Xna.Framework.Rectangle(0, 0, 2, 1);
                        foreach(var comm in comms)
                        {
                            editor.PatchMap(commspatch, commsPatchRect, new Microsoft.Xna.Framework.Rectangle(comm.X, comm.Y, 2, 1), PatchMapMode.ReplaceByLayer);
                            if (comm.ReceivingMessage && comm.SpeakerTile != null)
                            {
                                Map videopatch = ModEntry.imh.ModContent.Load<Map>(ModEntry.mapdir + "SecretLab_CommsPatch_Video.tmx");
                                editor.PatchMap(videopatch, new Microsoft.Xna.Framework.Rectangle((int)comm.SpeakerTile, 0, 1, 1), new Microsoft.Xna.Framework.Rectangle(comm.X, comm.Y, 1, 1), PatchMapMode.ReplaceByLayer);

                                //Layer? backLayer = map?.GetLayer(LayerName);
                                //if (backLayer == null)
                                //    return Array.Empty<Vector2>();

                                //// Use a list to dynamically gather matching tile positions
                                //List<Vector2> matchedTiles = new List<Vector2>();

                                //// 2. Loop through the bounds of the layer grid
                                //for (int x = 0; x < backLayer.LayerWidth; x++)
                                //{
                                //    for (int y = 0; y < backLayer.LayerHeight; y++)
                                //    {
                                //        Tile tile = backLayer.Tiles[x, y];

                            }
                        }
                    }

                });
                
            }
        }

        public static Vector2[] GetSecretLabTileCoordinates(Map map, string LayerName, string TilesheetName, int TileIndex)
        {
            // 1. Locate the specific layer safely
            Layer? backLayer = map?.GetLayer(LayerName);
            if (backLayer == null)
                return Array.Empty<Vector2>();

            // Use a list to dynamically gather matching tile positions
            List<Vector2> matchedTiles = new List<Vector2>();

            // 2. Loop through the bounds of the layer grid
            for (int x = 0; x < backLayer.LayerWidth; x++)
            {
                for (int y = 0; y < backLayer.LayerHeight; y++)
                {
                    Tile tile = backLayer.Tiles[x, y];

                    // 3. Ensure the grid coordinate isn't an empty/null tile
                    if (tile == null)
                        continue;

                    // 4. Verify the tilesheet matches "SecretLab" and the local Tile Index is 12
                    if (tile.TileSheet != null &&
                        tile.TileSheet.Id.Equals(TilesheetName, StringComparison.OrdinalIgnoreCase) &&
                        tile.TileIndex == TileIndex)
                    {
                        // Adds the grid index coordinate (e.g. X: 5, Y: 10)
                        matchedTiles.Add(new Vector2(x, y));
                    }
                }
            }

            // Convert to a flat array for the return value
            return matchedTiles.ToArray();
        }
        public static Vector2[] GetSecretLabTileCoordinates(IAssetData asset, string layerName, string tilesheetName, int tileIndex)
        {
            // 1. Ensure the asset is loaded as a map and extract its Data property
            if (!asset.DataType.IsAssignableTo(typeof(Map)))
                return Array.Empty<Vector2>();

            Map map = asset.AsMap().Data;

            // 2. Locate the specific layer safely
            Layer? targetLayer = map?.GetLayer(layerName);
            if (targetLayer == null)
                return Array.Empty<Vector2>();

            List<Vector2> matchedTiles = new List<Vector2>();

            // 3. Loop through the bounds of the layer grid
            for (int x = 0; x < targetLayer.LayerWidth; x++)
            {
                for (int y = 0; y < targetLayer.LayerHeight; y++)
                {
                    Tile tile = targetLayer.Tiles[x, y];

                    if (tile == null)
                        continue;

                    // 4. Verify the tilesheet matches and the local Tile Index matches
                    if (tile.TileSheet != null &&
                        tile.TileSheet.Id.Equals(tilesheetName, StringComparison.OrdinalIgnoreCase) &&
                        tile.TileIndex == tileIndex)
                    {
                        matchedTiles.Add(new Vector2(x, y));
                    }
                }
            }

            return matchedTiles.ToArray();
        }

        private static void AddBarrel(GameLocation location)
        {
            Vector2 coords = new Vector2(35, 21);
            Object db = new Object(coords, "sdvhead.LawAndOrderSV_DebrisBarrel");
            db.Fragility = 1;
            location.objects.Remove(coords);
            location.objects.Add(coords, db);
        }

    }
}
