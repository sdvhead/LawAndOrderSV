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

        internal static void Init()
        {
            ModEntry.imh.Events.Content.AssetRequested += OnAssetRequested;
            ModEntry.imh.Events.GameLoop.DayStarted += OnDayStart;
            GameLocation.RegisterTileAction(coveDoor_tileActionID, CoveDoor_Interact);
        }

        private static bool CoveDoor_Interact(GameLocation location, string[] args, Farmer farmer, Microsoft.Xna.Framework.Point point)
        {
            ModEntry.Log("Cove door interact");

            if (!Game1.MasterPlayer.mailReceived.Contains(mailflag_sawDemetriusArrest))
            {
                
                // Display the text inside a standard Stardew Valley dialogue box
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

                    Map sourceMap = ModEntry.imh.ModContent.Load<Map>(ModEntry.mapdir + "SecretLab_ConveyorPatch.tmx");
                    ModEntry.Log("patching from map: " + sourceMap.Id);
                    editor.PatchMap(sourceMap, null, new Microsoft.Xna.Framework.Rectangle(14, 8, 1, 1), PatchMapMode.ReplaceByLayer);

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
                        
                    }
                });

            }
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
