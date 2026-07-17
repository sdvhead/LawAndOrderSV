using StardewModdingAPI;
/*
using ContentPatcher;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Delegates;
using StardewValley.GameData.Machines;
using StardewValley.Inventories;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.Objects.Trinkets;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using xTile.Dimensions;
*/


namespace LawAndOrderSV
{
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/

        public static ModEntry Instance = null!;
        public static IModHelper imh = null!;
        public static IManifest manifest = null!;

        internal const string ModId = "sdvhead.LawAndOrderSV";
        internal const LogLevel DEFAULT_LOG_LEVEL = LogLevel.Debug;

        public override void Entry(IModHelper helper)
        {
            imh = helper;
            Instance = this;
            manifest = this.ModManifest;

            CollectOBot.Init();
            ClearLand.Init();
            MurderMysteryFestival.Init();
        }

        internal static void Log(string msg, LogLevel level = DEFAULT_LOG_LEVEL)
        {
            ModEntry.Instance.Monitor.Log(msg, level);
        }
    }
}

