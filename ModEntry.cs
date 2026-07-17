using StardewModdingAPI;

namespace LawAndOrderSV
{
    internal sealed class ModEntry : Mod
    {
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

