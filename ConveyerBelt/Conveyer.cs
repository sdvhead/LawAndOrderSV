using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Machines;
using StardewValley.Inventories;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System.Runtime.CompilerServices;
using Object = StardewValley.Object;
using LawAndOrderSV;
using Microsoft.Xna.Framework;
using StardewValley.Characters;

namespace LawAndOrderSV
{
    public static class Conveyer
    {
        private const string NorthId = "sdvhead.LawAndOrdersV_conveyorPath_north";
        private const string SouthId = "sdvhead.LawAndOrdersV_conveyorPath_south";
        private const string EastId = "sdvhead.LawAndOrdersV_conveyorPath_east";
        private const string WestId = "sdvhead.LawAndOrdersV_conveyorPath_west";

        internal static void Init()
        {
            ModEntry.imh.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private static void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            ModEntry.Log("GameLaunch Conveyor init");
            var harmony = new Harmony(ModEntry.ModId);
            harmony.Patch(
                            original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.placementAction)),
                            prefix: new HarmonyMethod(typeof(Conveyer), nameof(PlacementAction_Prefix))
                        );
        }
        public static bool PlacementAction_Prefix(
            StardewValley.Object __instance,
            GameLocation location,
            int x,
            int y,
            Farmer who,
            ref bool __result)
        {
            ModEntry.Log("placement action prefix");
            try
            {
                if (__instance == null || location == null)
                    return true;

                var id = __instance.ItemId;

                ModEntry.Log("ID is " + id);

                TerrainFeature? feature = id switch
                {
                    NorthId => new SpeedBuffFlooringNorth(),
                    SouthId => new SpeedBuffFlooringSouth(),
                    EastId => new SpeedBuffFlooringEast(),
                    WestId => new SpeedBuffFlooringWest(),
                    _ => null
                };

                if (feature == null)
                    return true; // not our item, let vanilla run

                var key = new Vector2(x, y);

                // Optional: clear existing feature if you need to
                // location.terrainFeatures.Remove(key);

                location.terrainFeatures[key] = feature;

                __result = true;
                return false; // IMPORTANT: skip original placementAction
            }
            catch (Exception ex)
            {
                ModEntry.Log($"Conveyor placement failed: {ex}", LogLevel.Error);
                return true;
            }
        }
    }
}
