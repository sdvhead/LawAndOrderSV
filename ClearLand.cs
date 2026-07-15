using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;

using StardewValley.TerrainFeatures;


namespace LawAndOrderSV
{
    internal class ClearLand
    {
        internal static void Init()
        {
            ModEntry.imh.Events.GameLoop.DayStarted += OnDayStart;
        }

        private static void OnDayStart(object? sender, DayStartedEventArgs e)
        {
            //remove any debris from previous SVE saves that might have spawned in the Sheriff's Office location
            GameLocation town = Game1.getLocationFromName("town");
            Microsoft.Xna.Framework.Rectangle sheriffRect = new Microsoft.Xna.Framework.Rectangle(70, 0, 8, 19);
            Microsoft.Xna.Framework.Rectangle fogwoodRect = new Microsoft.Xna.Framework.Rectangle(61, 0, 9, 9);

            foreach (Vector2 key in town.terrainFeatures.Keys)
            {
                TerrainFeature tf = town.terrainFeatures[key];
                if (sheriffRect.Contains(tf.Tile.X, tf.Tile.Y))
                {
                    town.terrainFeatures.Remove(key);
                }
                if (fogwoodRect.Contains(tf.Tile.X, tf.Tile.Y))
                {
                    town.terrainFeatures.Remove(key);
                }

            }

            List<LargeTerrainFeature> listLTF = new();
            foreach (LargeTerrainFeature ltf in town.largeTerrainFeatures)
            {
                if (sheriffRect.Contains(ltf.Tile.X, ltf.Tile.Y))
                {
                    listLTF.Add(ltf);
                }
                if (fogwoodRect.Contains(ltf.Tile.X, ltf.Tile.Y))
                {
                    listLTF.Add(ltf);
                }
            }
            foreach (LargeTerrainFeature ltf in listLTF)
            {
                bool hasltf = town.largeTerrainFeatures.Contains(ltf);
                if (hasltf)
                {
                    town.largeTerrainFeatures.Remove(ltf);
                }
            }
        }

    }
}
