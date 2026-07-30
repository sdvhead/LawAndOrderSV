using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Integrations.GenericModConfigMenu;
using StardewValley;
using StardewValley.Locations;
using System.Diagnostics.CodeAnalysis;
using xTile;

namespace LawAndOrderSV
{
    internal sealed class ModEntry : Mod
    {
        public static ModEntry Instance = null!;
        public static IModHelper imh = null!;
        public static IManifest manifest = null!;
        public static Harmony harmony = new Harmony(ModEntry.ModId);
        //public static string mapdir = "../Law and Order SV 1.5.0/[CP] Law and Order SV/assets/Locations/";
        public static string mapdir = "assets/maps/";
        internal const string ModId = "sdvhead.LawAndOrderSV";
        internal const LogLevel DEFAULT_LOG_LEVEL = LogLevel.Debug;

        public override void Entry(IModHelper helper)
        {
            imh = helper;
            Instance = this;
            manifest = this.ModManifest;

            I18n.Init(imh.Translation);
            CollectOBot.Init();
            ClearLand.Init();
            MurderMysteryFestival.Init();
            Conveyer.Init();
            SecretLab.AssetManager.Init();
            SecretLab.ConveyerBelt.Init();
            SecretLab.PowerStation.Init();
            
        }

        internal static void Log(string msg, LogLevel level = DEFAULT_LOG_LEVEL)
        {
            ModEntry.Instance.Monitor.Log(msg, level);
        }

        internal static bool HasMapProperty(
            GameLocation location,
            string propKey,
            [NotNullWhen(true)] out string? prop
        )
        {
            //from MMAP TryGetLocationalProperty https://github.com/Mushymato/MiscMapActionsProperties/blob/main/MiscMapActionsProperties/Framework/Wheels/CommonPatch.cs
            prop = null;
            if (location == null || (location.Name == null && location is not MineShaft))
                return false;
            if (location.GetData()?.CustomFields?.TryGetValue(propKey, out prop) ?? false)
            {
                return !string.IsNullOrWhiteSpace(prop);
            }
            if (location.Map != null && location.Map.Properties != null && location.TryGetMapProperty(propKey, out prop))
            {
                return !string.IsNullOrWhiteSpace(prop);
            }
            if (location.GetLocationContext()?.CustomFields?.TryGetValue(propKey, out prop) ?? false)
            {
                return !string.IsNullOrWhiteSpace(prop);
            }
            return false;
        }

    }


}

