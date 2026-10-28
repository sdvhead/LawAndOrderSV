//using HarmonyLib;
using ContentPatcher;
using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System.Diagnostics.CodeAnalysis;

namespace LawAndOrderSV
{
    internal sealed class ModEntry : Mod
    {
        public static ModEntry Instance = null!;
        public static IModHelper imh = null!;
        public static IManifest manifest = null!;
        
        public static string mapdir = "assets/maps/";
        internal const string ModId = "sdvhead.LawAndOrderSV";
        internal const LogLevel DEFAULT_LOG_LEVEL = LogLevel.Debug;

        internal ModConfig Config = null!;

        //public static Harmony harmony = new Harmony(ModEntry.ModId);

        public override void Entry(IModHelper helper)
        {
            imh = helper;
            Instance = this;
            manifest = this.ModManifest;


            //I18n.Init(imh.Translation);
            this.Config = imh.ReadConfig<ModConfig>();

            ModEntry.imh.Events.GameLoop.GameLaunched += OnGameLaunched!;


            ClearLand.Init();
            MurderMysteryFestival.Init();
            Ghostwood.Init();

            //CollectOBot.Init();
            //Conveyer.Init();
            //SecretLab.State.Init();
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );

            //add config option(s)
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("cfgRandomizeMaze.Name"),
                tooltip: () => this.Helper.Translation.Get("cfgRandomizeMaze.Description"),
                getValue: () => this.Config.cfgRandomizeMaze,
                setValue: value => this.Config.cfgRandomizeMaze = value
                );

            //could also add existing/additional murder mystery festival options here in the future


            //export c# config options to CP
            var cpapi = ModEntry.imh.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
            if (cpapi != null)
            {
                cpapi.RegisterToken(
                    ModEntry.manifest,
                    "csRandomizeMaze",
                    () => new[] { this.Config.cfgRandomizeMaze ? "true" : "false" }
                );
            }
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

