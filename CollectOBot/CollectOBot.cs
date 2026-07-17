using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Machines;
using StardewValley.Inventories;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System.Runtime.CompilerServices;
using Object = StardewValley.Object;

namespace LawAndOrderSV
{
    public static class CollectOBot
    {
        internal const string CollectOBotType = "sdvhead.LawAndOrderSV_CollectOBot";
        internal const string CollectOBotID = "(BC)sdvhead.LawAndOrderSV_CollectOBot";
        internal const string BatteryId = "(O)787";
        internal const string OilId = "(O)247";
        internal const string modData_machineID = ModEntry.ModId + "_MachineID";
        internal const string modData_bagID = ModEntry.ModId + "_BagID";
        internal const string modData_usedBattery = ModEntry.ModId + "_usedBattery";
        internal const string modData_collectFrequency = ModEntry.ModId + "_collectFrequency";
        internal const string modData_working = ModEntry.ModId + "_working";
        internal const int CollectOBot_ChestSlots = 70;

        private static bool machinesHaveOil = false; //may not strictly be accurate at all times. strictly used to determine whether to start monitoring time changes to optimize performance
        private static bool trackingTimeChange = false;

        internal static void Init()
        {
            ModEntry.imh.Events.GameLoop.DayStarted += OnDayStart;
            ModEntry.imh.Events.GameLoop.DayEnding += OnDayEnding;
            ModEntry.imh.Events.Display.MenuChanged += OnMenuChanged;
        }
        
        //Add a 'bot' property to ItemGrabMenu which we can assign when the menu exits and update the CollectOBot that spawned the menu
        private static readonly ConditionalWeakTable<ItemGrabMenu, Holder> _table = new();
        private sealed class Holder
        {
            public Object? Bot { get; set; }
        }
        public static Object? bot(this ItemGrabMenu menu)
        {
            if (menu == null) return null;
            return _table.GetOrCreateValue(menu).Bot;
        }
        public static void bot(this ItemGrabMenu menu, Object value)
        {
            if (menu == null) return;
            _table.GetOrCreateValue(menu).Bot = value;
        }

        /// <summary>Return all CollectOBots from Indoor Locations.</summary>
        private static IEnumerable<Object> GetCollectObots()
        {
            foreach (GameLocation location in Game1.locations)
            {
                if (location == null || location.IsOutdoors) continue;

                foreach (var pair in location.Objects.Pairs)
                {
                    var obj = pair.Value;
                    if (obj != null && obj.ItemId == CollectOBotType)
                        yield return obj;
                }
            }

            var farm = Game1.getFarm();
            if (farm != null)
            {
                foreach (var building in farm.buildings)
                {
                    if (building?.indoors.Value == null) continue;

                    var interior = building.indoors.Value;
                    foreach (var pair in interior.Objects.Pairs)
                    {
                        var obj = pair.Value;
                        if (obj != null && obj.ItemId == CollectOBotType)
                            yield return obj;
                    }
                }
            }
        }

        /// <summary>Track when the CollectoBot inventory is changed or closed so we make sure the bot is only picke up when empty</summary>
        private static void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            //ModEntry.Log("MenuChanged");
            if (e.NewMenu != null && e.OldMenu != null && e.OldMenu is ItemGrabMenu)
            {
                ItemGrabMenu oldm = (ItemGrabMenu)e.OldMenu;
                if (oldm.bot() != null)
                {
                    //update the machine durability here just so that it can't be destroyed by another player while the inventory is being changed
                    CollectOBot_UpdateDurability(oldm.bot()!);

                    ItemGrabMenu newm = (ItemGrabMenu)e.NewMenu;
                    newm.bot(oldm.bot()!);

                    newm.exitFunction = (IClickableMenu.onExit)
                        Delegate.Combine(
                            newm.exitFunction,
                            (IClickableMenu.onExit)
                                delegate
                                {
                                    //ModEntry.Log("onExit");
                                    CollectOBot_UpdateDurability(newm.bot()!);
                                }
                        );
                }
            }
        }

        /// <summary>Reset machine states for the next day.</summary>
        private static void OnDayEnding(object? sender, DayEndingEventArgs e)
        {
            foreach (Object bot in GetCollectObots())
            {
                bot.modData[modData_usedBattery] = "no";
                bot.modData[modData_collectFrequency] = "daily";
            }
        }

        /// <summary>Initiate collection for all (indoor) CollectOBots (requires a battery)</summary>
        private static void OnDayStart(object? sender, DayStartedEventArgs e)
        {
            foreach (Object bot in GetCollectObots())
            {
                if (CollectOBot_CountItem(bot, OilId) > 0)
                {
                    machinesHaveOil=true;
                }
                if (CollectOBot_CountItem(bot, BatteryId) > 0)
                {
                    CollectOBot_Collect(bot);
                }
            }

            if (machinesHaveOil && !trackingTimeChange)
            {
                trackingTimeChange=true;
                ModEntry.imh.Events.GameLoop.TimeChanged += OnTimeChanged;
            }
        }

        /// <summary>Retry collection when the game time changes for all machines that have oil stocked.</summary>
        private static void OnTimeChanged(object? sender, TimeChangedEventArgs e)
        {
            //batteries are consumed inside CollectOBot_Collect. Oil is consumed in this function.
            if(e.NewTime >= 610)
            {
                foreach (Object bot in GetCollectObots())
                {
                    bool foundBatteryProp = bot.modData.TryGetValue(modData_usedBattery, out string usedBattery);

                    bool powered = false;
                    if ((foundBatteryProp && usedBattery == "yes") || CollectOBot_CountItem(bot, BatteryId) > 0) powered = true;
                    bool oiled = false;
                    if (CollectOBot_CountItem(bot, OilId) > 0) oiled = true;

                    if (powered && oiled)
                    {
                        int numCollected = CollectOBot_Collect(bot);
                        if (numCollected > 0)
                        {
                            bot.modData[modData_usedBattery] = "yes";
                            CollectOBot_ConsumeInventory(bot, OilId);

                        }
                    }
                }
            }
        }

        private static int CollectOBot_ItemCount(Object bot)
        {
            bool found = bot.modData.TryGetValue(modData_bagID, out string myID);
            Inventory items = Game1.player.team.GetOrCreateGlobalInventory(myID);
            return items.Count;
        }

        /// <summary>Called whenever the contents of the CollectOBot change, since we need to make sure a CollectOBot with items can't be destroyed. Also requests a sprite update since the sprite is affected by the contents.</summary>
        private static void CollectOBot_UpdateDurability(Object bot)
        {
            int count = CollectOBot_ItemCount(bot);
            if (count > 0) bot.Fragility = 2;
            else bot.Fragility = 0;
            //ModEntry.Log("item count is " + count + ". Fragility is now " + bot.Fragility + ".");

            CollectOBot_UpdateSprite(bot);
        }

        private static void CollectOBot_UpdateSprite(Object bot)
        {
            int sheetIndex = 0;
            int batt = CollectOBot_CountItem(bot,BatteryId);
            bool f = bot.modData.TryGetValue(modData_usedBattery, out string usedBattery);
            if (batt == 1 || usedBattery == "yes")
            {
                sheetIndex = 1;
            }
            if (batt > 1)
            {
                sheetIndex = 2;
            }
            if(sheetIndex>0 && CollectOBot_CountItem(bot, OilId) > 0)
            {
                sheetIndex = 3;

                if (!trackingTimeChange)
                {
                    //ModEntry.Log("adding onTimeChanged");
                    machinesHaveOil = true;
                    trackingTimeChange = true;
                    ModEntry.imh.Events.GameLoop.TimeChanged += OnTimeChanged;
                }

                bot.modData.TryGetValue(modData_working, out string working);
                if (working == "no")
                {
                    CollectOBot_StartWorkingAnimation(bot);
                }
            }
            bot.ParentSheetIndex = sheetIndex;
            if (sheetIndex< 3){
                CollectOBot_StopWorkingAnimation(bot);
            }
        }

        private static void CollectOBot_StartWorkingAnimation(Object bot)
        {
            //ModEntry.Log("StartWorkingAnimation");
            bot.modData[modData_working] = "yes";
            DelayedAction.functionAfterDelay(
                () =>
                {
                    CollectOBot_SetWorkingSprite(bot, 1);
                },
                200
            );
        }
        private static void CollectOBot_StopWorkingAnimation(Object bot)
        {
            bot.modData[modData_working] = "no";
        }

        /// <summary>Animate an oiled CollectOBot</summary>
        /// <param name="bot">The CollectOBot being animated</param>
        /// <param name="workingStage">the current animation stage</param>
        private static void CollectOBot_SetWorkingSprite(Object bot, int workingStage)
        {
            //ModEntry.Log("SetWorking Sprite [" + workingStage + "]");
            bot.modData.TryGetValue(modData_working, out string working);
            if (working == "yes") {
                if (workingStage > 5) workingStage = 0;
                bot.ParentSheetIndex = workingStage + 3;//indexes 0-2 are for machines that don't animate (updating daily instead of on time change)

                DelayedAction.functionAfterDelay(
                    () =>
                    {
                        CollectOBot_SetWorkingSprite(bot, workingStage + 1);
                    },
                    200
                );
            }
        }

        /// <summary>Reset the machine, so it's ready to accept a new input.</summary>
        /// <param name="obj">The machine object that was harvested</param>
        /// <remarks>This implementation is based on <see cref="Object.CheckForActionOnMachine"/>.</remarks>
        private static void ResetMachine(Object obj)
        {

            var mtype = obj.GetType();

            Object objectThatWasHeld = obj.heldObject.Value;
            obj.heldObject.Value = null;
            obj.readyForHarvest.Value = false;
            obj.showNextIndex.Value = false;
            obj.ResetParentSheetIndex();
            
            var machineData = obj.GetMachineData();
            if (MachineDataUtility.TryGetMachineOutputRule(obj, machineData, MachineOutputTrigger.OutputCollected, objectThatWasHeld.getOne(), null, obj.Location, out var outputCollectedRule, out var _, out var _, out var _))
            {
                obj.OutputMachine(machineData, outputCollectedRule, objectThatWasHeld, null, obj.Location, probe: false);
            }
            
            if (obj.IsTapper() && obj.Location.terrainFeatures.TryGetValue(obj.TileLocation, out var terrainFeature) && terrainFeature is Tree tree)
            {
                tree.UpdateTapperProduct(obj, objectThatWasHeld);
            }
            
            if (machineData != null && machineData.ExperienceGainOnHarvest != null)
            {
                string[] expSplit = machineData.ExperienceGainOnHarvest.Split(' ');
                for (int i = 0; i < expSplit.Length; i += 2)
                {
                    int skill = Farmer.getSkillNumberFromName(expSplit[i]);
                    if (skill != -1 && ArgUtility.TryGetInt(expSplit, i + 1, out var amount, out var _))
                    {
                        Game1.player.gainExperience(skill, amount);
                    }
                }
            }

            obj.AttemptAutoLoad(null);

        }

        /// <summary>Attempts to add the entire stack of Obj (machine output) to an existing stack (if available) of the bot, otherwise a new stack</summary>
        /// <param name="bot">The CollectOBot that is storing the object stack</param>
        /// <param name="obj">The Object (machine output item) that is being added</param>
        private static bool TryAddObjectToBot(Object obj, Object bot)
        {
            int incomingStackSize = obj.Stack;
            if (incomingStackSize <= 0) return false;

            bool found = bot.modData.TryGetValue(modData_machineID, out string myID);
            if (!found) return false;

            bool success = false;
            Game1.player.team.GetOrCreateGlobalInventoryMutex(myID).RequestLock(() =>
            {
                Inventory inventory = Game1.player.team.GetOrCreateGlobalInventory(myID);
                inventory.RemoveEmptySlots();

                bool matchingStack = false;
                for(int i=0; i<inventory.Count; i++)
                {
                    Item invStack = inventory[i];
                    if(invStack is Object myInv && invStack.canStackWith(obj) && (myInv.Stack+ incomingStackSize) <= myInv.maximumStackSize())
                    {
                        matchingStack = true;
                        myInv.Stack += incomingStackSize;
                        success = true;
                        return;
                    }
                }
                if (inventory.Count< CollectOBot_ChestSlots && !matchingStack)
                {
                    Item newStack = obj.getOne();
                    newStack.Stack = incomingStackSize;
                    inventory.Add(newStack);
                    success = true;
                    return;
                }
            });
            return success;
        }

        /// <summary>Harvest the output from one machine into the current running CollectOBot</summary>
        /// <param name="bot">The CollectOBot that is harvesting output</param>
        /// <param name="obj">The machine that is being harvested</param>
        public static bool HarvestMachine(Object bot, Object obj)
        {
            //prevent collecting unhatched eggs from incubators
            MachineData mdata = obj.GetMachineData();
            if (mdata.IsIncubator) return false;

            Object machineOutput = obj.heldObject.Value;
            //ModEntry.Log("Attempting to harvest: " + machineOutput.Name + " (" + machineOutput.Stack + ") from " + obj.GetType().FullName + " into bot " + myID);
            if (TryAddObjectToBot(machineOutput, bot))
            {
                ResetMachine(obj);
                return true;
            }
            return false;
        }

        /// <summary>Initiate collection of all machines in the area, consuming a battery if required</summary>
        /// <param name="bot">The CollectOBot that is harvesting output</param>
        public static int CollectOBot_Collect(Object bot)
        {
            //collect all output from the machine's location
            GameLocation loc = bot.Location;
            string myID;
            bool found = bot.modData.TryGetValue(modData_machineID, out myID);

            //ModEntry.Log("attempting to collect: " + myID);
            int machinesHarvested = 0;
            if (found && loc.getNumberOfMachinesReadyForHarvest() > 0)
            {
                foreach (var pair in loc.Objects.Pairs)
                {
                    Object obj = pair.Value;
                    if (obj != null && obj.QualifiedItemId != myID && obj.readyForHarvest.Value && obj.heldObject.Value != null)
                    {
                        if(HarvestMachine(bot, obj)) machinesHarvested++;
                    }
                }
            }
            if (machinesHarvested > 0)
            {
                //ModEntry.Log("Machines harvested, checking for battery status");
                bool f = bot.modData.TryGetValue(modData_usedBattery, out string usedBattery);
                if (f && usedBattery=="no")
                {
                    //ModEntry.Log("Battery not used yet today, consuming battery");
                    CollectOBot_ConsumeBattery(bot);
                    bot.modData[modData_usedBattery] = "yes";
                }
            }
            CollectOBot_UpdateDurability(bot);

            return machinesHarvested;
        }

        private static void CollectOBot_ConsumeBattery(Object bot)
        {
            CollectOBot_ConsumeInventory(bot,BatteryId);
        }

        private static void CollectOBot_ConsumeInventory(Object bot, string ItemID)
        {
            string myID = null!;
            bool found = bot.modData.TryGetValue(modData_bagID, out myID);
            if (!found) return;

            Game1.player.team.GetOrCreateGlobalInventoryMutex(myID).RequestLock(() =>
            {
                Inventory items = Game1.player.team.GetOrCreateGlobalInventory(myID);
                items.ReduceId(ItemID, 1);
            });
            CollectOBot_UpdateDurability(bot);
        }

        /// <summary>Returns a CollectOBot's internal ID, or instantiates the bot with all required properties the first time the inventory is accessed.</summary>
        /// <param name="bot">The CollectOBot to retrieve or create the ID</param>
        public static string CollectOBot_ID(Object bot)
        {
            string myID = null!;
            bool found = bot.modData.TryGetValue(modData_bagID, out myID);
            if (!found)
            {
                myID = ModEntry.ModId + Guid.NewGuid().ToString();
                bot.modData[modData_machineID] = myID;
                bot.modData[modData_bagID] = myID;
                bot.modData[modData_usedBattery] = "no";
                bot.modData[modData_collectFrequency] = "daily";
                bot.modData[modData_working] = "no";
                var globalInv = Game1.player.team.GetOrCreateGlobalInventory(myID);
            }
            return myID;
        }


        /// <summary>Called by the game when interacting with the bot to open it's inventory menu.</summary>
        /// <param name="bot">The machine being interacted with</param>
        /// <param name="location">The machine's current location</param>
        /// <param name="player">The current player</param>
        public static bool CollectOBot_Interact(Object bot, GameLocation location, Farmer player)
        {
            string bagID = CollectOBot_ID(bot);

            var args = new[] { "test", bagID };
            //ShowBag(args, bot, out _);
            ShowBotMenu(bot);
            return true;
        }

        private static bool ShowBotMenu(Object bot)
        //heavily based on MMAP function ShowGlobalInventory / TileShowBag
        {
            Chest phChest = new(playerChest: true);
            phChest.SpecialChestType = Chest.SpecialChestTypes.BigChest;

            bool before = Game1.player.showChestColorPicker;
            Game1.player.showChestColorPicker = false;
            phChest.GlobalInventoryId = CollectOBot_ID(bot);

            phChest.GetMutex().RequestLock(() =>
            {
                phChest.ShowMenu();

                if (Game1.activeClickableMenu is ItemGrabMenu igm)
                {
                    igm.bot(bot);
                    igm.exitFunction = (IClickableMenu.onExit)
                        Delegate.Combine(
                            igm.exitFunction,
                            (IClickableMenu.onExit)
                                delegate
                                {
                                    Game1.player.showChestColorPicker = before;
                                    phChest.GetMutex().ReleaseLock();
                                    CollectOBot_UpdateDurability(bot);
                                }
                        );
                }
                });
            return true;
        }

        private static int CollectOBot_CountItem(Object bot, string itemId)
        {
            bool found = bot.modData.TryGetValue(modData_bagID, out string myID);
            if (found)
            {
                if (!Game1.player.team.globalInventories.TryGetValue(myID, out Inventory inventory))
                    return 0;

                return inventory.CountId(itemId);
                //return inventory.ContainsId(BatteryId, 1);
            }
            return 0;
        }
    }
}
