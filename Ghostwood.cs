using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Objects;
using xTile;
using xTile.Dimensions;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace LawAndOrderSV
{
    internal static class Ghostwood
    {


        internal static Mazemap? maze1a;
        internal static Mazemap? maze1b;
        internal static Mazemap? maze2a;
        internal static Mazemap? maze2b;
        internal static Mazemap? maze3a;
        internal static Mazemap? maze3b;
        internal static Mazemap? maze4a;
        internal static Mazemap? maze4b;
        internal static Mazemap? maze5a;
        internal static Mazemap? maze5b;
        //internal static Mazemap? maze6;

        internal static List<Mazemap> mazemaps = new List<Mazemap>();

        internal class Mazemap
        {
            internal Vector2 NorthArrive1 { get; set; }
            internal Vector2 NorthArrive2 { get; set; }
            internal Vector2 EastArrive1 { get; set; }
            internal Vector2 EastArrive2 { get; set; }
            internal Vector2 SouthArrive1 { get; set; }
            internal Vector2 SouthArrive2 { get; set; }
            internal Vector2 WestArrive1 { get; set; }
            internal Vector2 WestArrive2 { get; set; }

            internal Vector2 NorthDepart1 { get; set; }
            internal Vector2 NorthDepart2 { get; set; }
            internal Vector2 EastDepart1 { get; set; }
            internal Vector2 EastDepart2 { get; set; }
            internal Vector2 SouthDepart1 { get; set; }
            internal Vector2 SouthDepart2 { get; set; }
            internal Vector2 WestDepart1 { get; set; }
            internal Vector2 WestDepart2 { get; set; }

            internal string? MapName { get; set; }
            internal string? NextMapRight { get; set; }
            internal string? NextMapWrong { get; set; }
            internal bool UseRandomGrave { get; set; }
            internal string MapAsset { get; set; }

            internal string? MapNorth { get; set; }
            internal string? MapEast { get; set; }
            internal string? MapWest { get; set; }
            internal string? MapArrival { get; set; }

            internal string? MapDeparture { get; set; }

            internal string? CaveMap { get; set; }
            



            public Mazemap(string mapName, string nextMapRight, string nextMapWrong, string caveMap, bool useRandomGrave)
            {
                NorthArrive1 = new Vector2(29,2);
                NorthArrive2 = new Vector2(30,2);
                EastArrive1 = new Vector2(47,19);
                EastArrive2 = new Vector2(47,20);
                SouthArrive1 = new Vector2(29,37);
                SouthArrive2 = new Vector2(30,37);
                WestArrive1 = new Vector2(12, 19);
                WestArrive2 = new Vector2(12, 20);

                NorthDepart1 = new Vector2(29,-1);
                NorthDepart2 = new Vector2(30,-1);
                EastDepart1 = new Vector2(50,19);
                EastDepart2 = new Vector2(50,20);
                SouthDepart1 = new Vector2(29,40);
                SouthDepart2 = new Vector2(30,40);
                WestDepart1 = new Vector2(9,19);
                WestDepart2 = new Vector2(9,20);

                MapName = ModEntry.ModId + "_" + mapName;
                NextMapRight = ModEntry.ModId + "_" + nextMapRight;
                NextMapWrong = ModEntry.ModId + "_" + nextMapWrong;
                UseRandomGrave = useRandomGrave;
                MapAsset = "Maps/" + MapName;
                CaveMap = caveMap;

                MapNorth = NextMapWrong;
                MapEast = NextMapWrong;
                MapWest = NextMapWrong;

                MapArrival = null;
                MapDeparture = null;

            }
        }

        internal static void Init()
        {
            maze1a = new Mazemap("gmx1a", "gmx2a", "gmx1b", "cliffMine0", true);
            maze1a.MapArrival = "South";
            mazemaps.Add(maze1a);
            maze1b = new Mazemap("gmx1b", "gmx1a", "gmx1a", "cliffMine1", false);
            mazemaps.Add(maze1b);
            maze2a = new Mazemap("gmx2a", "gmx3a", "gmx1b", "cliffMine2", true);
            mazemaps.Add(maze2a);
            maze2b = new Mazemap("gmx2b", "gmx1a", "gmx1a", "cliffMine3", false);
            mazemaps.Add(maze2b);
            maze3a = new Mazemap("gmx3a", "gmx4a", "gmx1b", "cliffMine4", true);
            mazemaps.Add(maze3a);
            maze3b = new Mazemap("gmx3b", "gmx2a", "gmx2a", "cliffMine5", false);
            mazemaps.Add(maze3b);
            maze4a = new Mazemap("gmx4a", "gmx5a", "gmx2b", "cliffMine6", true);
            mazemaps.Add(maze4a);
            maze4b = new Mazemap("gmx4b", "gmx3a", "gmx3a", "cliffMine7", false);
            mazemaps.Add(maze4b);
            maze5a = new Mazemap("gmx5a", "GhostwoodxFalls", "gmx3b", "cliffMine8", true);
            mazemaps.Add(maze5a);
            maze5b = new Mazemap("gmx5b", "gmx3a", "gmx3a", "cliffMine9", false);
            mazemaps.Add(maze5b);
            //maze6 = new Mazemap("gmx6", "GhostwoodxFalls", "gmx3b", false);
            //mazemaps.Add(maze6);


            ModEntry.imh.Events.GameLoop.DayStarted += OnDayStart;
            //ModEntry.imh.Events.Player.Warped += OnPlayerWarped;
            ModEntry.imh.Events.Content.AssetRequested += OnAssetRequested;

        }

        private static void GenerateMazeLayout()
        {
            Random rng = CreateMazeRandom();

            // reset all arrivals
            maze1a!.MapArrival = "South";

            maze2a!.MapArrival = null;
            maze3a!.MapArrival = null;
            maze4a!.MapArrival = null;
            maze5a!.MapArrival = null;
            //maze6!.MapArrival = null;

            maze1b!.MapArrival = null;
            maze2b!.MapArrival = null;
            maze3b!.MapArrival = null;
            maze4b!.MapArrival = null;
            maze5b!.MapArrival = null;

            GenerateMazeDeparture(maze1a, rng);
            GenerateMazeDeparture(maze2a, rng);
            GenerateMazeDeparture(maze3a, rng);
            GenerateMazeDeparture(maze4a, rng);
            //GenerateMazeDeparture(maze5a);
            maze5a.MapDeparture = "North";
        }

        private static void GenerateMazeDeparture(Mazemap maze, Random rng)
        {
            if (maze.MapArrival == null)
                return;

            maze.MapDeparture =
                GetRandomDepartureDirection(maze.MapArrival, rng);

            Mazemap nextMaze = GetMaze(maze.NextMapRight!);

            switch (maze.MapDeparture)
            {
                case "North":
                    nextMaze.MapArrival = "South";
                    break;

                case "East":
                    nextMaze.MapArrival = "West";
                    break;

                case "West":
                    nextMaze.MapArrival = "East";
                    break;
            }

        }

        //private static void OnPlayerWarped(object? sender, WarpedEventArgs e)
        //{
        //    if (!ModEntry.Instance.Config.cfgRandomizeMaze) return;

        //    Game1.addHUDMessage(new HUDMessage(e.NewLocation.Name)
        //    {
        //        noIcon = true,
        //        timeLeft = 4000f
        //    });
        //}

        private static void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (!ModEntry.Instance.Config.cfgRandomizeMaze) return;

            string mapPath = e.Name.Name;
            int pos = mapPath.LastIndexOf("/") + 1;
            string mapFile = mapPath.Substring(pos, mapPath.Length - pos);


            if (mapFile.ContainsIgnoreCase(ModEntry.ModId) && mapFile.ContainsIgnoreCase("_gmx") && !mapFile.Contains("gmx6")) 
            {
              
                Mazemap maze = GetMaze(mapFile);


                if (maze.UseRandomGrave)
                {
                    e.Edit(asset =>
                    {
                        IAssetDataForMap editor = asset.AsMap();
                        Map mapdata = asset.AsMap().Data;

                        Map gravemap = ModEntry.imh.ModContent.Load<Map>(ModEntry.mapdir + "CliffMazeGraveRandom.tmx");
                        editor.PatchMap(gravemap, new Microsoft.Xna.Framework.Rectangle(0, 0, 7, 7), new Microsoft.Xna.Framework.Rectangle(39, 7, 7, 7), PatchMapMode.ReplaceByLayer);

                        Map removemap = ModEntry.imh.ModContent.Load<Map>(ModEntry.mapdir + "CliffMazeGraveRandomReplace.tmx");


                        switch (maze.MapDeparture)
                        {
                            case "North":
                                editor.PatchMap(removemap, new Microsoft.Xna.Framework.Rectangle(0, 0, 2, 3), new Microsoft.Xna.Framework.Rectangle(41, 7, 2, 3), PatchMapMode.Replace);
                                break;
                            case "East":
                                editor.PatchMap(removemap, new Microsoft.Xna.Framework.Rectangle(0, 0, 2, 3), new Microsoft.Xna.Framework.Rectangle(44, 10, 2, 3), PatchMapMode.Replace);
                                break;
                            case "West":
                                editor.PatchMap(removemap, new Microsoft.Xna.Framework.Rectangle(0, 0, 2, 3), new Microsoft.Xna.Framework.Rectangle(39, 9, 2, 3), PatchMapMode.Replace);
                                break;
                        }
                        
                    });
                }
                


                }
        }

        private static Random CreateMazeRandom()
        {
            unchecked
            {
                int seed = HashCode.Combine(
                    Game1.uniqueIDForThisGame,
                    Game1.Date.TotalDays,
                    "GhostwoodMazeRng");

                return new Random(seed);
            }
        }

        private static string GetRandomDepartureDirection(string arrivalDirection, Random rng)
        {
            string[] validArrivals = {"South","East","West"};
            if (!validArrivals.Contains(arrivalDirection))
            {
                throw new ArgumentException("Arrival direction must be South, East, or West.", nameof(arrivalDirection));
            }
            string[] directions = { "North", "East", "West" };

            string[] choices = directions
                .Where(direction => direction != arrivalDirection)
                .ToArray();

            //return choices[Random.Shared.Next(choices.Length)];

            return choices[rng.Next(choices.Length)];
        }

        private static Mazemap GetMaze(string mapname)
        {
            foreach (Mazemap mm in mazemaps)
            {
                if (mapname.StartsWith(mm.MapName!,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return mm;
                }
            }

            throw new Exception("maze not found: " + mapname);
        }

        internal static void SetWarps(Mazemap mm)
        {
            GameLocation location = Game1.getLocationFromName(mm.MapName);
            GameLocation nextRight = Game1.getLocationFromName(mm.NextMapRight);
            GameLocation nextWrong = Game1.getLocationFromName(mm.NextMapWrong);

            string correctExit = "North";
            if (mm.UseRandomGrave) correctExit = mm.MapDeparture!;

            location.warps.Clear();

            //cliff mine
            location.warps.Add(new Warp(
                        14, 23,
                        $"{ModEntry.ModId}_{mm.CaveMap}",
                        12, 38,
                        flipFarmer: false
             ));

            //south exit
            location.warps.Add(new Warp(
                        29, 40,
                        $"{ModEntry.ModId}_GhostwoodxPass",
                        34, 1,
                        flipFarmer: false
             ));
            location.warps.Add(new Warp(
                        30, 40,
                        $"{ModEntry.ModId}_GhostwoodxPass",
                        34, 1,
                        flipFarmer: false
             ));

            switch (correctExit)
            {
                case "North":
                    mm.MapNorth = mm.NextMapRight;

                    location.warps.Add(new Warp(
                        (int)mm.NorthDepart1.X, (int)mm.NorthDepart1.Y,
                        nextRight.Name,
                        (int)mm.SouthArrive1.X, (int)mm.SouthArrive1.Y,
                        flipFarmer: false
                        ));
                    location.warps.Add(new Warp(
                        (int)mm.NorthDepart2.X, (int)mm.NorthDepart2.Y,
                        nextRight.Name,
                        (int)mm.SouthArrive2.X, (int)mm.SouthArrive2.Y,
                        flipFarmer: false
                        ));

                    location.warps.Add(new Warp(
                        (int)mm.EastDepart1.X, (int)mm.EastDepart1.Y,
                        nextWrong.Name,
                        (int)mm.WestArrive1.X, (int)mm.WestArrive1.Y,
                        flipFarmer: false
                        ));
                    location.warps.Add(new Warp(
                        (int)mm.EastDepart2.X, (int)mm.EastDepart2.Y,
                        nextWrong.Name,
                        (int)mm.WestArrive2.X, (int)mm.WestArrive2.Y,
                        flipFarmer: false
                        ));

                    location.warps.Add(new Warp(
                        (int)mm.WestDepart1.X, (int)mm.WestDepart1.Y,
                        nextWrong.Name,
                        (int)mm.EastArrive1.X, (int)mm.EastArrive1.Y,
                        flipFarmer: false
                        ));
                    location.warps.Add(new Warp(
                        (int)mm.WestDepart2.X, (int)mm.WestDepart2.Y,
                        nextWrong.Name,
                        (int)mm.EastArrive2.X, (int)mm.EastArrive2.Y,
                        flipFarmer: false
                        ));

                    break;
                case "East":
                    mm.MapEast = mm.NextMapRight;

                    location.warps.Add(new Warp(
                        (int)mm.NorthDepart1.X, (int)mm.NorthDepart1.Y,
                        nextWrong.Name,
                        (int)mm.SouthArrive1.X, (int)mm.SouthArrive1.Y,
                        flipFarmer: false
                        ));
                    location.warps.Add(new Warp(
                        (int)mm.NorthDepart2.X, (int)mm.NorthDepart2.Y,
                        nextWrong.Name,
                        (int)mm.SouthArrive2.X, (int)mm.SouthArrive2.Y,
                        flipFarmer: false
                        ));

                    location.warps.Add(new Warp(
                        (int)mm.EastDepart1.X, (int)mm.EastDepart1.Y,
                        nextRight.Name,
                        (int)mm.WestArrive1.X, (int)mm.WestArrive1.Y,
                        flipFarmer: false
                        ));
                    location.warps.Add(new Warp(
                        (int)mm.EastDepart2.X, (int)mm.EastDepart2.Y,
                        nextRight.Name,
                        (int)mm.WestArrive2.X, (int)mm.WestArrive2.Y,
                        flipFarmer: false
                        ));

                    location.warps.Add(new Warp(
                        (int)mm.WestDepart1.X, (int)mm.WestDepart1.Y,
                        nextWrong.Name,
                        (int)mm.EastArrive1.X, (int)mm.EastArrive1.Y,
                        flipFarmer: false
                        ));
                    location.warps.Add(new Warp(
                        (int)mm.WestDepart2.X, (int)mm.WestDepart2.Y,
                        nextWrong.Name,
                        (int)mm.EastArrive2.X, (int)mm.EastArrive2.Y,
                        flipFarmer: false
                        ));

                    break;
                case "West":
                    mm.MapWest = mm.NextMapRight;

                    location.warps.Add(new Warp(
                        (int)mm.NorthDepart1.X, (int)mm.NorthDepart1.Y,
                        nextWrong.Name,
                        (int)mm.SouthArrive1.X, (int)mm.SouthArrive1.Y,
                        flipFarmer: false
                        ));
                    location.warps.Add(new Warp(
                        (int)mm.NorthDepart2.X, (int)mm.NorthDepart2.Y,
                        nextWrong.Name,
                        (int)mm.SouthArrive2.X, (int)mm.SouthArrive2.Y,
                        flipFarmer: false
                        ));

                    location.warps.Add(new Warp(
                        (int)mm.EastDepart1.X, (int)mm.EastDepart1.Y,
                        nextWrong.Name,
                        (int)mm.WestArrive1.X, (int)mm.WestArrive1.Y,
                        flipFarmer: false
                        ));
                    location.warps.Add(new Warp(
                        (int)mm.EastDepart2.X, (int)mm.EastDepart2.Y,
                        nextWrong.Name,
                        (int)mm.WestArrive2.X, (int)mm.WestArrive2.Y,
                        flipFarmer: false
                        ));

                    location.warps.Add(new Warp(
                        (int)mm.WestDepart1.X, (int)mm.WestDepart1.Y,
                        nextRight.Name,
                        (int)mm.EastArrive1.X, (int)mm.EastArrive1.Y,
                        flipFarmer: false
                        ));
                    location.warps.Add(new Warp(
                        (int)mm.WestDepart2.X, (int)mm.WestDepart2.Y,
                        nextRight.Name,
                        (int)mm.EastArrive2.X, (int)mm.EastArrive2.Y,
                        flipFarmer: false
                        ));
                    break;
            }

            //location.updateWarps();
            //ModEntry.imh.GameContent.InvalidateCache(mm.MapAsset);
            
        }

        private static void OnDayStart(object? sender, DayStartedEventArgs e)
        {
            if (!ModEntry.Instance.Config.cfgRandomizeMaze) return;

            GenerateMazeLayout();

            ModEntry.imh.GameContent.InvalidateCache(
                asset =>
                    asset.Name.BaseName.Contains($"{ModEntry.ModId}_gmx")
                    && !asset.Name.BaseName.EndsWith($"{ModEntry.ModId}_gmx6"));

            SetWarps(maze1a!);
            SetWarps(maze2a!);
            SetWarps(maze3a!);
            SetWarps(maze4a!);
            SetWarps(maze5a!);

            SetWarps(maze1b!);
            SetWarps(maze2b!);
            SetWarps(maze3b!);
            SetWarps(maze4b!);
            SetWarps(maze5b!);
            SetWarps(maze5b!);


        }

    }
}
