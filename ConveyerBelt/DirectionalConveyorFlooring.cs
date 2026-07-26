using LawAndOrderSV;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using StardewValley.TerrainFeatures;

public abstract class DirectionalConveyorFlooring : Flooring
{
    // Abstract properties (not fields)
    protected abstract float Magnitude { get; }
    protected abstract bool IsVerticalAxis { get; }
    protected abstract bool IsPositiveTravelBuffNorthOrEast { get; }

    public override void doCollisionAction(Rectangle positionOfCollider, int speedOfCollision, Vector2 tileLocation, Character who)
    {
        ModEntry.Log("collisionAction North");
        base.doCollisionAction(positionOfCollider, speedOfCollision, tileLocation, who);

        if (who is not Farmer player)
            return;

        float tileCenterX = tileLocation.X * 64f + 32f;
        float tileCenterY = tileLocation.Y * 64f + 32f;

        if (IsVerticalAxis)
        {
            bool travelingNorth = positionOfCollider.Center.Y > tileCenterY;
            player.temporarySpeedBuff = GetSignedBuff(travelingNorth ? 1f : -1f);
        }
        else
        {
            bool travelingEast = positionOfCollider.Center.X < tileCenterX;
            player.temporarySpeedBuff = GetSignedBuff(travelingEast ? 1f : -1f);
        }
    }

    // +Magnitude if travel matches the variant’s "positive" direction; otherwise -Magnitude
    private float GetSignedBuff(float travelPositive)
    {
        bool travelIsPositive = travelPositive > 0f;

        return (travelIsPositive == IsPositiveTravelBuffNorthOrEast)
            ? Magnitude
            : -Magnitude;
    }
}

// ---- 4 variants ----

public sealed class SpeedBuffFlooringNorth : DirectionalConveyorFlooring
{
    protected override float Magnitude => 4.00f;
    protected override bool IsVerticalAxis => true;
    protected override bool IsPositiveTravelBuffNorthOrEast => true;
}

public sealed class SpeedBuffFlooringSouth : DirectionalConveyorFlooring
{
    protected override float Magnitude => 0.10f;
    protected override bool IsVerticalAxis => true;
    protected override bool IsPositiveTravelBuffNorthOrEast => false;
}

public sealed class SpeedBuffFlooringEast : DirectionalConveyorFlooring
{
    protected override float Magnitude => 0.10f;
    protected override bool IsVerticalAxis => false;
    protected override bool IsPositiveTravelBuffNorthOrEast => true;
}

public sealed class SpeedBuffFlooringWest : DirectionalConveyorFlooring
{
    protected override float Magnitude => 0.10f;
    protected override bool IsVerticalAxis => false;
    protected override bool IsPositiveTravelBuffNorthOrEast => false;
}
