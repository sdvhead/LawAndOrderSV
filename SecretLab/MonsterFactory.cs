using LawAndOrderSV.SecretLab.Monsters;
using Microsoft.Xna.Framework;

using StardewModdingAPI.Events;
using StardewValley;

using StardewValley.Monsters;


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
            //warehouse_spawn_points.Add(new Point(24, 17));
            //warehouse_spawn_points.Add(new Point(24, 19));

            warehouse_spawn_points.Add(new Point(40, 35));
            warehouse_spawn_points.Add(new Point(42, 35));
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
                    var monster = new Robug(Utility.PointToVector2(spawn_point) * 64f);

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
