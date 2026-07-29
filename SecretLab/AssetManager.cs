using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Menus;
using System.Runtime.CompilerServices;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;
using Object = StardewValley.Object;

namespace LawAndOrderSV.SecretLab
{
    internal static class AssetManager
    {

        internal static void Init()
        {
            ModEntry.imh.Events.Content.AssetRequested += OnAssetRequested;
        }

        private static void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.Name.IsEquivalentTo("Maps/sdvhead.LawAndOrderSV_SecretLab_Cove"))
            {

                e.Edit(asset =>
                {
                    var editor = asset.AsMap();


                    Map sourceMap = ModEntry.imh.ModContent.Load<Map>(ModEntry.mapdir + "SecretLab_ConveyorPatch.tmx");
                    ModEntry.Log("patching from map: " + sourceMap.Id);
                    editor.PatchMap(sourceMap, null, new Microsoft.Xna.Framework.Rectangle(14, 8, 1, 1), PatchMapMode.ReplaceByLayer);

                    if (PowerStation.station1_powered)
                    {
                        ModEntry.Log("patching powered station");
                        Map powermap = ModEntry.imh.ModContent.Load<Map>(ModEntry.mapdir + "SecretLab_PowerStation.tmx");

                        ModEntry.Log("replacing power station tile 1");
                        editor.PatchMap(powermap, new Microsoft.Xna.Framework.Rectangle(1, 0, 1, 1), new Microsoft.Xna.Framework.Rectangle(67, 6, 1, 1), PatchMapMode.ReplaceByLayer);
                        ModEntry.Log("replacing power station tile 2");
                        editor.PatchMap(powermap, new Microsoft.Xna.Framework.Rectangle(3, 0, 1, 1), new Microsoft.Xna.Framework.Rectangle(67, 5, 1, 1), PatchMapMode.ReplaceByLayer);
                    }
                    else
                    {
                        ModEntry.Log("no power station patching needed");
                    }
                });

            }
        }

    }
}
