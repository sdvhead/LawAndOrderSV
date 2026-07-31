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
            if (e.Name.IsEquivalentTo("Maps/IslandSouthEastCave"))
            {
                /*
                if (Game1.MasterPlayer.mailReceived.Contains(mailflag_openedCoveDoor))
                {
                   
                }
                else if (Game1.MasterPlayer.mailReceived.Contains(mailflag_sawDemetriusArrest))
                {
                    
                }
                else
                {

                }
                */
            }

            if (e.Name.IsEquivalentTo("Maps/sdvhead.LawAndOrderSV_SecretLab_Cove"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsMap();
                    var mapdata = asset.AsMap().Data;

                    //Map sourceMap = ModEntry.imh.ModContent.Load<Map>(ModEntry.mapdir + "SecretLab_ConveyorPatch.tmx");
                    //ModEntry.Log("patching from map: " + sourceMap.Id);
                    //editor.PatchMap(sourceMap, null, new Microsoft.Xna.Framework.Rectangle(14, 8, 1, 1), PatchMapMode.ReplaceByLayer);

                    if (PowerStation.station1_powered)
                    {
                        ModEntry.Log("patching powered station");
                        Map powermap = ModEntry.imh.ModContent.Load<Map>(ModEntry.mapdir + "SecretLab_PowerStation.tmx");

                        ModEntry.Log("replacing power station tile 1");
                        editor.PatchMap(powermap, new Microsoft.Xna.Framework.Rectangle(1, 0, 1, 1), new Microsoft.Xna.Framework.Rectangle(67, 6, 1, 1), PatchMapMode.ReplaceByLayer);
                        ModEntry.Log("replacing power station tile 2");
                        editor.PatchMap(powermap, new Microsoft.Xna.Framework.Rectangle(3, 0, 1, 1), new Microsoft.Xna.Framework.Rectangle(67, 5, 1, 1), PatchMapMode.ReplaceByLayer);
                    }
                    else
                    {
                        ModEntry.Log("no power station patching needed");
                    }

                    if (PowerStation.powered_lights)
                    {
                        ModEntry.Log("lights powered");
                        byte r = 95;
                        byte g = 95;
                        byte b = 95;

                        // Assign the value into xTile's custom properties dictionary
                        //mapdata.Properties["AmbientLight"] = "95 95 95";
                        mapdata.Properties["AmbientLight"] = $"{r} {g} {b}";
                        mapdata.Properties["Light"] = string.Join(" ", Array.ConvertAll(coveLightCoords, v => $"{v.X} {v.Y} 4"));
                        for (int i = 0; i < coveLightCoords.Length; i++)
                        {
                            Vector2 coords = coveLightCoords[i];
                            Map lightmap = ModEntry.imh.ModContent.Load<Map>(ModEntry.mapdir + "SecretLab_CoveLightPatch.tmx");
                            editor.PatchMap(lightmap, null, new Microsoft.Xna.Framework.Rectangle((int)coords.X, (int)coords.Y, 1, 1), PatchMapMode.ReplaceByLayer);
                        }
                    }
                    Vector2[] NorthConveyors = GetSecretLabTileCoordinates(asset, "Back", "SecretLab", 10);
                    Vector2[] SouthConveyors = GetSecretLabTileCoordinates(asset, "Back", "SecretLab", 11);
                    Vector2[] EastConveyors = GetSecretLabTileCoordinates(asset, "Back", "SecretLab", 12);
                    Vector2[] WestConveyors = GetSecretLabTileCoordinates(asset, "Back", "SecretLab", 13);
                    Map conveyorpatch = ModEntry.imh.ModContent.Load<Map>(ModEntry.mapdir + "SecretLab_ConveyorPatch.tmx");

                    if (PowerStation.powered_conveyor)
                    {
                        ModEntry.Log("Conveyor is powered - eastconveyor length ("+EastConveyors.Length+")");

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
                            editor.PatchMap(conveyorpatch, new Microsoft.Xna.Framework.Rectangle(2,1,1,1), new Microsoft.Xna.Framework.Rectangle((int)EastConveyors[i].X, (int)EastConveyors[i].Y, 1, 1), PatchMapMode.ReplaceByLayer);
                        }
                        for (int i = 0; i < WestConveyors.Length; i++)
                        {
                            editor.PatchMap(conveyorpatch, new Microsoft.Xna.Framework.Rectangle(3, 1, 1, 1), new Microsoft.Xna.Framework.Rectangle((int)WestConveyors[i].X, (int)WestConveyors[i].Y, 1, 1), PatchMapMode.ReplaceByLayer);
                        }
                    }

                });

            }
        }

        /*public static string lightProperty()
        {
            return string.Join(" ", Array.ConvertAll(coveLightCoords, v => $"{v.X} {v.Y} 4"));

        }*/
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
