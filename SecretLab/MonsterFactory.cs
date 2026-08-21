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
    internal static class MonsterFactory
    {

        internal const int maxMonsters_Warehouse = 8;
        internal static List<Point> warehouse_spawn_points = new List<Point>();
        public static Random generationRandom = new Random();
        internal static void Init()
        {
            AddSpawnPoints();
            ModEntry.imh.Events.Player.Warped += OnPlayerWarped;
        }

        private static void AddSpawnPoints()
        {
            warehouse_spawn_points.Add(new Point(24, 17));
            warehouse_spawn_points.Add(new Point(24, 19));
        }

        private static void OnPlayerWarped(object? sender, WarpedEventArgs e)
        {
            if (e == null || e.NewLocation == null) return;

            if (e.NewLocation.Name == "sdvhead.LawAndOrderSV_SecretLab_Warehouse")
            {
                var location = e.NewLocation;
                ModEntry.Log("monster factory location match");

                var monsters = GetMonsters(location);
                if (monsters.Count < maxMonsters_Warehouse) {
                    int index = generationRandom.Next(0, warehouse_spawn_points.Count);
                    Point spawn_point = warehouse_spawn_points[index];
                    Monster monster = new RoBat(Utility.PointToVector2(spawn_point) * 64f);
                    ModEntry.Log("adding RoBat to location");
                    location.characters.Add(monster);
                }

            }
        }

        private static List<StardewValley.Character> GetMonsters(GameLocation location)
        {
           var monsters = new List<StardewValley.Character>();
           foreach(Character character in location.characters)
            {
                if (character is Monster)
                {
                    monsters.Add(character);
                }
            }
            return monsters;
        }

    }
}
