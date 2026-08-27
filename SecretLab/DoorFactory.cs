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
    internal static class DoorFactory
    {

        internal static List<Door> doors = new List<Door>();

        internal static void Init()
        {
            GameLocation.RegisterTouchAction("sdvhead.LawAndOrderSV_SecretLab_Door_TouchOpen", Door_TouchOpen);
            GameLocation.RegisterTouchAction("sdvhead.LawAndOrderSV_SecretLab_Door_TouchClose", Door_TouchClose);
            AddDoors();

        }

        private static void AddDoors()
        {
            doors.Add(new Door(51, 41, "sdvhead.LawAndOrderSV_SecretLab_Warehouse")); //by cove stairs
            doors.Add(new Door(41, 30, "sdvhead.LawAndOrderSV_SecretLab_Warehouse")); //entrance to factory
            doors.Add(new Door(68, 13, "sdvhead.LawAndOrderSV_SecretLab_Warehouse")); //panicked researcher
            doors.Add(new Door(8, 41, "sdvhead.LawAndOrderSV_SecretLab_Warehouse"));  //elevator
            doors.Add(new Door(30, 38, "sdvhead.LawAndOrderSV_SecretLab_Warehouse")); //southeast of elevator
        }

        public static List<Door> GetDoors(string mapName)
        {
            List<Door> ds = new();
            for (int i = 0; i < doors.Count; i++)
            {
                var d = doors[i];
                if (d.MapName == mapName)
                {
                    ds.Add(d);
                }
            }
            return ds;
        }

        public static Door? FindClosestDoor(Vector2 vector2, GameLocation location)
        {
            int x = (int)vector2.X;
            int y = (int)vector2.Y;

            List<Door> localDoors = new();
            for(var i = 0; i<doors.Count; i++)
            {
                if (doors[i].location() == location)
                {
                    localDoors.Add(doors[i]);
                }
            }

            if (localDoors.Count < 1)
            {
                return null;
            }

            var closestDoor = localDoors[0];     

            if (localDoors.Count > 1) {
                double closestDistance = Utils.GetDistance(x,y,closestDoor.X, closestDoor.Y);

                for (var i = 1; i < localDoors.Count; i++)
                {
                    var door = localDoors[i];
                    double distance = Utils.GetDistance(x, y, door.X, door.Y);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestDoor = door;
                    }
                }
            }
            return closestDoor;
        }

        private static void Door_TouchOpen(GameLocation location, string[] args, Farmer farmer, Vector2 vector2)
        {
            var door = FindClosestDoor(vector2, location);
            if (door != null && door.CanOpen)
            {
                if (PowerBoxFactory.powered_doorsbelts)
                {
                    door.Open();
                }
            }
        }

        private static void Door_TouchClose(GameLocation location, string[] args, Farmer farmer, Vector2 vector2)
        {
            var door = FindClosestDoor(vector2, location);
            if (door != null && door.CanClose)
            {
                if (PowerBoxFactory.powered_doorsbelts)
                {
                    door.Close();
                }
            }
        }

    }
}
