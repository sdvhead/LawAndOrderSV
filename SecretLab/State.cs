using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawAndOrderSV.SecretLab
{
    internal static class State
    {

        //notes: coveDoorUnlocked in AssetManager is temporarily locked to true (on player warp) for testing.

        public static void Init()
        {
            ModEntry.imh.Events.GameLoop.DayStarted += OnDayStart;
            SecretLab.AssetManager.Init();
            SecretLab.DoorFactory.Init();
            SecretLab.PowerBoxFactory.Init();
            SecretLab.ConveyerBelt.Init();
            SecretLab.CommFactory.Init();
            SecretLab.ElevatorFactory.Init();
            SecretLab.MonsterFactory.Init();
            SecretLab.MiscInteracts.Init();
        }
        private static void OnDayStart(object? sender, DayStartedEventArgs e)
        {
            PowerBoxFactory.DailyReset();
            ElevatorFactory.DailyReset();

        }
    }
}
