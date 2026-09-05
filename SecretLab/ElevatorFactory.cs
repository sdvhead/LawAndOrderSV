using HarmonyLib;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.Locations;
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
    internal static class ElevatorFactory
    {

        internal static List<Elevator> Elevators = new List<Elevator>();

        public static string elevatorQuestionKey = ModEntry.ModId + "_SecretLab_ElevatorQuestionKey";

        private static void AddElevators()
        {
            //identify all elevators on maps. coordinates are upper left corner of 4x3 box.
            Elevators.Add(new Elevator(7, 2, "sdvhead.LawAndOrderSV_SecretLab_Cove")); //cove/'maintenance tunnel'
            Elevators.Add(new Elevator(7, 29, "sdvhead.LawAndOrderSV_SecretLab_Warehouse")); //warehouse/'factory floor'
            Elevators.Add(new Elevator(7, 29, "sdvhead.LawAndOrderSV_SecretLab_Operations")); //operations (maze)
            //future floors: 'Operations','Research','Rooftop'
            //Elevators.Add(new Elevator(2, 41, "sdvhead.LawAndOrderSV_SecretLab_Warehouse")); //south of elevator
        }

        public static void DailyReset()
        {
            Elevators.Clear();
            AddElevators();
        }

        internal static void Init()
        {
            GameLocation.RegisterTileAction("sdvhead.LawAndOrderSV_SecretLab_Elevator_Interact", Elevator_Interact);
            AddElevators();

            ModEntry.harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.answerDialogueAction)),
               prefix: new HarmonyMethod(typeof(ElevatorFactory),
                                          nameof(ElevatorFactory.AnswerElevatorQuestion))
               );
        }



        public static List<Elevator> GetElevators(string mapName)
        {
            List<Elevator> els = new();
            for (int i = 0; i < Elevators.Count; i++)
            {
                var el = Elevators[i];
                if (el.MapName == mapName)
                {
                    els.Add(el);
                }
            }
            return els;
        }

        private static bool Elevator_Interact(GameLocation location, string[] args, Farmer farmer, Microsoft.Xna.Framework.Point point)
        {
            Elevator? elevator = FindClosestElevator(point, location);

            if (elevator != null)
            {
                return elevator.Interact(location, args, farmer, point);
            }
            return true;

        }

        public static Elevator? FindClosestElevator(Point point, GameLocation location)
        {
            int x = (int)point.X;
            int y = (int)point.Y;

            List<Elevator> localElevators = new();
            for (var i = 0; i < Elevators.Count; i++)
            {
                if (Elevators[i].location() == location)
                {
                    localElevators.Add(Elevators[i]);
                }
            }

            if (localElevators.Count < 1)
            {
                return null;
            }

            var closestElevator = localElevators[0];

            if (localElevators.Count > 1)
            {
                double closestDistance = Utils.GetDistance(x, y, closestElevator.X, closestElevator.Y);

                for (var i = 1; i < localElevators.Count; i++)
                {
                    var elevator = localElevators[i];
                    double distance = Utils.GetDistance(x, y, elevator.X, elevator.Y);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestElevator = elevator;
                    }
                }
            }
            return closestElevator;
        }

        public static void ElevatorWarp(string mapName)
        {
            var elevators = GetElevators(mapName);
            if (elevators.Count == 1)
            {
                Elevator elevator = elevators[0];
                int x = elevator.X + 1;
                int y = elevator.Y + 3;

                Game1.player.completelyStopAnimatingOrDoingAction();
                Game1.playSound("busDoorOpen");
                DelayedAction.playSoundAfterDelay("crystal", 1200, Game1.getLocationFromName(mapName));
                Game1.warpFarmer(mapName, x, y, 2);
            }

        }
        public static bool AnswerElevatorQuestion(GameLocation __instance, string questionAndAnswer, string[] questionParams)
        {
            if(questionAndAnswer == elevatorQuestionKey+"_Warehouse")
            {
                ElevatorWarp("sdvhead.LawAndOrderSV_SecretLab_Warehouse");
            }else if (questionAndAnswer == elevatorQuestionKey + "_Cove")
            {
                ElevatorWarp("sdvhead.LawAndOrderSV_SecretLab_Cove");
            }
            else if (questionAndAnswer == elevatorQuestionKey + "_Operations")
            {
                ElevatorWarp("sdvhead.LawAndOrderSV_SecretLab_Operations");
            }

            return true;
        }

    }
}
