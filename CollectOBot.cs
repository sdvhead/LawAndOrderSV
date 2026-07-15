using ContentPatcher;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
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
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using xTile.Dimensions;
using static StardewValley.Minigames.BoatJourney;

namespace LawAndOrderSV
{



    public class CollectOBot
    {

        internal const string CollectOBotID = "(BC)sdvhead.LawAndOrderSV_CollectOBot";
        internal const string BatteryId = "(O)787";
        internal const string BatteryName = "Battery Pack";
        internal const string modData_machineID = ModEntry.ModId + "_MachineID";
        internal const string modData_bagID = ModEntry.ModId + "_BagID";

        internal static void Init()
        {
            //ModEntry.imh.Events.Player.Warped += OnPlayerWarped;
        }
        
        /*private static void OnPlayerWarped(object? sender, WarpedEventArgs e)
        {
            if (!Context.IsWorldReady || e.NewLocation == null) return;
            GameLocation location = e.NewLocation;
            if (location is Shed || (location.NameOrUniqueName != null && location.NameOrUniqueName.Contains("Shed")) || location.NameOrUniqueName.Equals("Cellar", StringComparison.OrdinalIgnoreCase))
            {
                ProcessLocationCollection(location);
            }
        }*/

        /*public static string uniqueMachineID(StardewValley.Object machine)
        {
            string myID;
            bool found = machine.modData.TryGetValue(modData_machineID, out myID);

            if (!found)
            {
                myID = Guid.NewGuid().ToString();
                ModEntry.Log("created identifier: " + myID);
                machine.modData[modData_machineID] = myID;
            }
            ModEntry.Log("found identifier: " + myID);
            return myID;
        }
        */

        internal static string GetBagInventoryId(string bagInvId)
        {
            //return string.Join('#', ModEntry.ModId, bagInvId);
            return bagInvId;
        }
        public static void CollectOBot_Collect(StardewValley.Object bot)
        {

            //collect all output from the machine's location
            GameLocation loc = bot.Location;
            string myID;
            bool found = bot.modData.TryGetValue(modData_machineID, out myID);

            ModEntry.Log("attempting to collect: " + myID);
            if (found && loc.getNumberOfMachinesReadyForHarvest() > 0)
            {
                foreach (var pair in loc.Objects.Pairs)
                {
                    StardewValley.Object obj = pair.Value;
                    if (obj != null && obj.QualifiedItemId != myID && obj.readyForHarvest.Value && obj.heldObject.Value != null)
                    {
                        Item producedItem = obj.heldObject.Value;     
                        Item myItem = producedItem.getOne();
                        if (myItem != null)
                        {
                            bool added = false;
                            Game1.player.team.GetOrCreateGlobalInventoryMutex(myID).RequestLock(() =>
                            {
                                try
                                {
                                    Inventory items = Game1.player.team.GetOrCreateGlobalInventory(myID);
                                    items.Add(myItem);
                                    added = true;
                                }
                                catch (Exception ex)
                                {
                                    ModEntry.Log("failed to add collected item: " + ex.Message);
                                    added = false;
                                }
                            });
                            if (added)
                            {
                                obj.heldObject.Value = null;
                                obj.readyForHarvest.Value = false;
                                obj.showNextIndex.Value = false;
                            }
                        }


                    }
                }
            }
        }


        public static string CollectOBot_ID(StardewValley.Object bot)
        {
            string myID = null!;
            bool found = bot.modData.TryGetValue(modData_bagID, out myID);
            if (!found)
            {
                myID = ModEntry.ModId + Guid.NewGuid().ToString();
                bot.modData[modData_machineID] = myID;
                bot.modData[modData_bagID] = myID;
                var globalInv = Game1.player.team.GetOrCreateGlobalInventory(myID);
            }
            return myID;
        }
        public static bool CollectOBot_Interact(StardewValley.Object machine, GameLocation location, Farmer player)
        {

            string bagID = CollectOBot_ID(machine);

            CollectOBot_Collect(machine);

            Game1.playSound("dwop");
            LawAndOrderSV.ModEntry.Log("CollectOBot clicked.");

            StardewValley.Object held = player.ActiveObject;
            if (held != null)
            {
                if (held.Name.Equals(BatteryName)) {
                    LawAndOrderSV.ModEntry.Log("Battery is active object.");
                }
                else
                {
                    LawAndOrderSV.ModEntry.Log("Active Object:"+held.Name);
                }
            }
            else
            {
                LawAndOrderSV.ModEntry.Log("no active object");
            }

            //string bagID = GetBagInventoryId(uniqueMachineID(machine));
            //machine.modData[modData_bagID] = bagID;
            //var consumed = consumeBattery(machine);

            //ModEntry.Log("getting or creating inventory: " + bagID);
            //var globalInv = Game1.player.team.GetOrCreateGlobalInventory(bagID); // Inventory

            var args = new[] { "test", bagID };
            ShowBag(args, out _);

            //var consumed = consumeBattery(machine);

            var result = TestInventory(machine);

            return true;
        }


        private static bool ShowBag(string[] args, out string? error)
        {
            if (
                !ArgUtility.TryGet(args, 1, out string? bagInvId, out error, allowBlank: false, "string bagInvId")
                || !ArgUtility.TryGetOptionalEnum(
                    args,
                    2,
                    out Chest.SpecialChestTypes bagInvType,
                    out error,
                    defaultValue: Chest.SpecialChestTypes.None,
                    "Chest.SpecialChestTypes bagInvType"
                )
            )
            {
                ModEntry.Log(error, LogLevel.Error);

                return false;
            }
            Chest phChest = new(playerChest: true);

            bool before = Game1.player.showChestColorPicker;
            Game1.player.showChestColorPicker = false;

            phChest.GlobalInventoryId = GetBagInventoryId(bagInvId);
            phChest.SpecialChestType = bagInvType;
            ModEntry.Log($"Open global inventory {phChest.GlobalInventoryId} ({phChest.SpecialChestType})");
            phChest
                .GetMutex()
                .RequestLock(() =>
                {
                    phChest.ShowMenu();
                    if (Game1.activeClickableMenu is ItemGrabMenu igm)
                    {
                        igm.exitFunction = (IClickableMenu.onExit)
                            Delegate.Combine(
                                igm.exitFunction,
                                (IClickableMenu.onExit)
                                    delegate
                                    {
                                        Game1.player.showChestColorPicker = before;
                                        phChest.GetMutex().ReleaseLock();
                                    }
                            );
                    }
                });
            return true;
        }

        //mushymato.MMAP_AddItemToBag <bagInventoryId> <qualifiedItemId> [amount] [quality]
        //ModifyItemsInBag(args, AddItems, out _);
        //modifyBy(items, qId, amount, quality);
        private static bool ogModifyItemsInBag(string[] args, Action<Inventory, string, int, int> modifyBy, out string? error)
        {
            if (
                !ArgUtility.TryGet(args, 1, out string? bagInvId, out error, allowBlank: false, "string bagInvId")
                || !ArgUtility.TryGet(args, 2, out string? qId, out error, allowBlank: false, "string qualifiedItemId")
                || !ArgUtility.TryGetOptionalInt(args, 3, out int amount, out error, defaultValue: 1, name: "int amount")
                || !ArgUtility.TryGetOptionalInt(args, 4, out int quality, out error, defaultValue: 0, name: "int quality")
            )
            {
                ModEntry.Log(error, LogLevel.Error);
                return false;
            }
            string globalInvId = GetBagInventoryId(bagInvId);
            Game1
                .player.team.GetOrCreateGlobalInventoryMutex(globalInvId)
                .RequestLock(() =>
                {
                    Inventory items = Game1.player.team.GetOrCreateGlobalInventory(globalInvId);
                    modifyBy(items, qId, amount, quality);
                });
            return true;
        }

        /*private static bool ModifyItemsInBag(string bagID, string itemID, int amount, )
        {
            if (
                !ArgUtility.TryGet(args, 1, out string? bagInvId, out error, allowBlank: false, "string bagInvId")
                || !ArgUtility.TryGet(args, 2, out string? qId, out error, allowBlank: false, "string qualifiedItemId")
                || !ArgUtility.TryGetOptionalInt(args, 3, out int amount, out error, defaultValue: 1, name: "int amount")
                || !ArgUtility.TryGetOptionalInt(args, 4, out int quality, out error, defaultValue: 0, name: "int quality")
            )
            {
                ModEntry.Log(error, LogLevel.Error);
                return false;
            }
            string globalInvId = GetBagInventoryId(bagInvId);
            Game1
                .player.team.GetOrCreateGlobalInventoryMutex(globalInvId)
                .RequestLock(() =>
                {
                    Inventory items = Game1.player.team.GetOrCreateGlobalInventory(globalInvId);
                    modifyBy(items, qId, amount, quality);
                });
            return true;
        }*/

        private static bool TestInventory(StardewValley.Object machine)
        {
            string myID;
            bool found = machine.modData.TryGetValue(modData_bagID, out myID);
            if (found)
            {
                var globalInv = Game1.player.team.GetOrCreateGlobalInventory(myID); // Inventory
                ModEntry.Log($"bag '{myID}': slotCount={globalInv.Count}, stackCount={globalInv.CountItemStacks()}, hasAny={globalInv.HasAny()}");
                //return inventory.ContainsId(itemId, minCount);

                bool hasItem = BAG_HAS_ITEM(myID, BatteryId, 1, 999);
                ModEntry.Log("HasItemCheck: " + hasItem);
            }
            return false;
        }
        private static bool BAG_HAS_ITEM(string bagInvId, string itemId, int minCount, int maxCount)
        {
            ModEntry.Log("BAG_HAS_ITEM(" + bagInvId + "," + itemId + "," + minCount + "," + maxCount + ")");
            if (!Game1.player.team.globalInventories.TryGetValue(GetBagInventoryId(bagInvId), out Inventory inventory))
                return false;
            if (maxCount != int.MaxValue)
            {
                int num = inventory.CountId(itemId);
                return num >= minCount && num <= maxCount;
            }
            return inventory.ContainsId(itemId, minCount);
        }

        private static bool consumeBattery(StardewValley.Object machine)
        {
            string bagID = CollectOBot_ID(machine);

            //string id = uniqueMachineID(machine);
            //string bagID = GetBagInventoryId(uniqueMachineID(machine));


            /*
                          ModEntry.Log($"bag '{bagID}': slotCount={inventory.Count}, stackCount={inventory.CountItemStacks()}, hasAny={inventory.HasAny()}");
            ModEntry.Log($"Battery stacks (CountId) = {inventory.CountId(BatteryId)}");

            for (int i = 0; i < inventory.Count; i++)
            {
                var item = inventory[i];
                if (item != null)
                    ModEntry.Log($"slot {i}: {item.Name} x{item.Stack}");
            }
            */

            if (Game1.player.team.globalInventories.TryGetValue(bagID, out Inventory inventory))

            {
                ModEntry.Log("retrieved inventory for bagID " + bagID + "(" + inventory.Count + ")");
                foreach (Item item in inventory)
                {
                    ModEntry.Log(item.Name + ": " + item.stack);
                }
                int num = inventory.CountId(BatteryId);

                if (inventory.ContainsId(BatteryId))
                {
                    ModEntry.Log("Contains battery: " + num);
                    return true;
                }
                else
                {
                    ModEntry.Log("no battery");
                    return false;
                }
            }
            else
            {
                ModEntry.Log("failed to retrieve inventory");
                return false;
            }

            //var globalInv = Game1.player.team.GetOrCreateGlobalInventory(bagID); // Inventory

            // if (!Game1.player.team.globalInventories.TryGetValue(GetBagInventoryId(bagInvId), out Inventory inventory))
            //     return false;


            //ModEntry.Log("checking inventory: " + bagID);
            //var args = new[] { bagID, BatteryId, "1" };
            //var hasbatt = BAG_HAS_ITEM(args);
            //ModEntry.Log("hasbatt: " + hasbatt);

        }


        public static void ProcessLocationCollection(GameLocation location)
        {
        }

    }
}
