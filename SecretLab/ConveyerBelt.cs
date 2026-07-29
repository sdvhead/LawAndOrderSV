using HarmonyLib;
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
    public static class ConveyerBelt
    {
        private const string partialMapName = "SecretLab";
        //private const string assetFolder = "assets/Include/secretlab/";
        private const string mapprop = ModEntry.ModId + "_SecretLab";
        private const float moveBoost = 4.0f;
        private const float moveReduce = 4.0f;
        private const float moveMin = 0.25f;
        private const float moveIdle = 3.0f;
        
        private const int dirNorth = 0;
        private const int dirEast = 1;
        private const int dirSouth = 2;
        private const int dirWest = 3;

        private const int tileNorthX = 10;
        private const int tileNorthY = 0;
        private const int tileEastX = 12;
        private const int tileEastY = 0;
        private const int tileSouthX = 11;
        private const int tileSouthY = 0;
        private const int tileWestX = 13;
        private const int tileWestY = 0;

        private const string BackLayerId = "Back";

        private static bool watchingTicks = false;
        private static bool harmonyPatched = false;


        public static void LawAndOrderSV_getMovementSpeed(Farmer __instance, ref float __result)
        {
            Farmer who = __instance;


            if (IsStandingOnConveyorNorth(who))
            {
                if (who.isMoving())
                {
                    if (who.movementDirections.Contains(dirNorth))
                    {
                        __result += moveBoost;
                    }
                    else if (who.movementDirections.Contains(dirSouth))
                    {
                        __result -= moveReduce;
                        if (__result <= moveMin)
                        {
                            __result = moveMin;
                        }
                    }
                    else
                    {
                        who.yVelocity = moveIdle;
                    }
                }
            }
            if (IsStandingOnConveyorSouth(who))
            {
                if (who.isMoving())
                {
                    if (who.movementDirections.Contains(dirSouth))
                    {
                        __result += moveBoost;
                    }
                    else if (who.movementDirections.Contains(dirNorth))
                    {
                        __result -= moveReduce;
                        if (__result <= moveMin)
                        {
                            __result = moveMin;
                        }
                    }
                    else
                    {
                        who.yVelocity = -moveIdle;
                    }
                }
            }
            else if (IsStandingOnConveyorEast(who))
            {
                if (who.isMoving())
                {
                    if (who.movementDirections.Contains(dirEast))
                    {
                        __result += moveBoost;
                    }
                    else if (who.movementDirections.Contains(dirWest))
                    {
                        __result -= moveReduce;
                        if (__result <= moveMin)
                        {
                            __result = moveMin;
                        }
                    }
                    else
                    {
                        who.xVelocity = moveIdle;
                    }
                }
            }
            else if (IsStandingOnConveyorWest(who))
            {
                if (who.isMoving())
                {
                    if (who.movementDirections.Contains(dirWest))
                    {
                        __result += moveBoost;
                    }
                    else if (who.movementDirections.Contains(dirEast))
                    {
                        __result -= moveReduce;
                        if (__result <= moveMin)
                        {
                            __result = moveMin;
                        }
                    }
                    else
                    {
                        who.xVelocity = -moveIdle;
                    }
                }
            }

        }

        internal static void Init()
        {
            ModEntry.imh.Events.Player.Warped += OnPlayerWarped;
        }

        



        private static void OnPlayerWarped(object? sender, WarpedEventArgs e)
        {
            if (!harmonyPatched)
            {
                ModEntry.harmony.Patch(
                   original: AccessTools.Method(typeof(Farmer), nameof(Farmer.getMovementSpeed)),
                   postfix: new HarmonyMethod(typeof(ConveyerBelt),
                                              nameof(ConveyerBelt.LawAndOrderSV_getMovementSpeed))
                   );
                harmonyPatched = true;
            }
            if (ModEntry.HasMapProperty(e.NewLocation, mapprop, out _) && !watchingTicks){
                ModEntry.imh.Events.GameLoop.UpdateTicked += OnUpdateTicked;
                watchingTicks = true;
            }
            else if(watchingTicks && ModEntry.HasMapProperty(e.NewLocation, mapprop, out _))
            {
                ModEntry.imh.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
                watchingTicks = false;
            }
        }

        private static void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsPlayerFree) return;
            Farmer who = Game1.player;

            if(who != null && who.currentLocation!=null && !who.isMoving())
            {
                if (IsStandingOnConveyorNorth(who))
                {
                    who.yVelocity = moveIdle;
                }
                else if (IsStandingOnConveyorSouth(who))
                {
                    who.yVelocity = -moveIdle;
                }
                else if (IsStandingOnConveyorEast(who))
                {
                    who.xVelocity = moveIdle;
                }
                else if (IsStandingOnConveyorWest(who))
                {
                    who.xVelocity = -moveIdle;
                }
            }
            

        }

        private static bool IsStandingOnConveyorNorth(Farmer who)
        {
            GameLocation currentLocation = (who).currentLocation;
            if (currentLocation == null)
            {
                return false;
            }
            Vector2 standingPosition = (who).getStandingPosition();
            int num = 64;
            int tileX = (int)(standingPosition.X / (float)num);
            int tileY = (int)(standingPosition.Y / (float)num);
            return IsTileAtLayer(currentLocation, tileX, tileY, tileNorthX, tileNorthY, "Back");
        }
        private static bool IsStandingOnConveyorSouth(Farmer who)
        {
            GameLocation currentLocation = (who).currentLocation;
            if (currentLocation == null)
            {
                return false;
            }
            Vector2 standingPosition = (who).getStandingPosition();
            int num = 64;
            int tileX = (int)(standingPosition.X / (float)num);
            int tileY = (int)(standingPosition.Y / (float)num);
            return IsTileAtLayer(currentLocation, tileX, tileY, tileSouthX, tileSouthY, "Back");
        }
        private static bool IsStandingOnConveyorEast(Farmer who)
        {
            GameLocation currentLocation = (who).currentLocation;
            if (currentLocation == null)
            {
                return false;
            }
            Vector2 standingPosition = (who).getStandingPosition();
            int num = 64;
            int tileX = (int)(standingPosition.X / (float)num);
            int tileY = (int)(standingPosition.Y / (float)num);
            return IsTileAtLayer(currentLocation, tileX, tileY, tileEastX, tileEastY, "Back");
        }
        private static bool IsStandingOnConveyorWest(Farmer who)
        {
            GameLocation currentLocation = (who).currentLocation;
            if (currentLocation == null)
            {
                return false;
            }
            Vector2 standingPosition = (who).getStandingPosition();
            int num = 64;
            int tileX = (int)(standingPosition.X / (float)num);
            int tileY = (int)(standingPosition.Y / (float)num);
            return IsTileAtLayer(currentLocation, tileX, tileY, tileWestX, tileWestY, "Back");
        }

        private static bool IsStandingOnConveyor(Farmer who)
        {
            GameLocation currentLocation = (who).currentLocation;
            if (currentLocation == null)
            {
                return false;
            }
            Vector2 standingPosition = (who).getStandingPosition();
            int num = 64;
            int tileX = (int)(standingPosition.X / (float)num);
            int tileY = (int)(standingPosition.Y / (float)num);
            return IsTileAtLayer(currentLocation, tileX, tileY, 10, 0, "Back");
        }


        private static bool IsTileAtLayer(GameLocation location, int tileX, int tileY, int wantedSheetX, int wantedSheetY, string layerId)
        {
            int tileIndex = location.map.GetTileIndexAt(tileX, tileY, layerId);
            if (tileIndex < 0)
            {
                return false;
            }
            int num = tileIndex % 16;
            int num2 = tileIndex / 16;
            return num == wantedSheetX && num2 == wantedSheetY;
        }

    }
}
