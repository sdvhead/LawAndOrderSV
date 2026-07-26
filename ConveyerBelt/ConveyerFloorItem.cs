using HarmonyLib;
using LawAndOrderSV;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.TerrainFeatures;

public class ConveyorFloorItem : StardewValley.Object
{
    public enum ConveyorType { North, South, East, West }

    private readonly ConveyorType conveyorType;

    public ConveyorFloorItem(string itemId, int initialStack, ConveyorType conveyorType)
        : base(itemId, initialStack)
    {
        this.conveyorType = conveyorType;
        ModEntry.Log("Item Id:" + itemId);
    }

    public override bool placementAction(GameLocation location, int x, int y, Farmer who)
    {
        Flooring flooring = conveyorType switch
        {
            ConveyorType.North => new SpeedBuffFlooringNorth(),
            ConveyorType.South => new SpeedBuffFlooringSouth(),
            ConveyorType.East => new SpeedBuffFlooringEast(),
            ConveyorType.West => new SpeedBuffFlooringWest(),
            _ => new SpeedBuffFlooringNorth()
        };

        Vector2 key = new Vector2(x, y);
        location.terrainFeatures[key] = flooring;

        return true;
    }
}
