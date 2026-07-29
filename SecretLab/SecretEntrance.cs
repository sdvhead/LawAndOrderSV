using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Object = StardewValley.Object;


namespace LawAndOrderSV.SecretLab
{
    internal class SecretEntrance
    {
        private const string mapname = "IslandSouthEastCave";

        internal static void Init()
        {
            ModEntry.imh.Events.Player.Warped += OnPlayerWarped;
        }
        private static void OnPlayerWarped(object? sender, WarpedEventArgs e)
        {

            if (e.NewLocation != null && e.NewLocation.Name.Equals(mapname))
            {
                AddBarrel(e.NewLocation);
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
