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

namespace LawAndOrderSV.SecretLab
{
    internal class Door
    {
        public int x { get; set; }
        public int y { get; set; }

        public bool canOpen { get; set; }
        public bool canClose { get; set; }

        public string mapName { get; set; }

        private static readonly int spriteSpeed = 120; //ms delay between open/close animations

        public Door(int x, int y, string mapName)
        {
            //x and y are the coordinates for the upper left corner of the 4-tile door
            this.x = x;
            this.y = y;
            this.mapName = mapName;
            this.canOpen = true;
            this.canClose = false;
        }

        public GameLocation location()
        {
            return Game1.getLocationFromName(this.mapName);
        }
        public void Open()
        {
            if (this.canOpen)
            {
                this.canOpen = false;
                this.SetSpriteOpen(1);
                DelayedAction.functionAfterDelay(() =>
                {
                    this.SetSpriteOpen(2);
                }, spriteSpeed);
                DelayedAction.functionAfterDelay(() =>
                {
                    this.SetSpriteOpen(3);
                }, spriteSpeed*2);
                DelayedAction.functionAfterDelay(() =>
                {
                    this.SetSpriteOpen(4);
                    this.canClose = true;
                }, spriteSpeed * 3);
            }
        }

        private void SetSpriteOpen(int phase)
        {
            int x1 = this.x;
            int x2 = this.x + 1;
            int y1 = this.y;
            int y2 = this.y + 1;

            int tile1 = 686;
            int tile2 = 687;
            int tile3 = 734;
            int tile4 = 735;

            GameLocation location = this.location();

            if (phase == 4)
            {
                location.removeMapTile(x1, y1, "Front");
                location.removeMapTile(x2, y1, "Front");
                location.removeMapTile(x1, y2, "Buildings");
                location.removeMapTile(x2, y2, "Buildings");
            }
            else
            {
                switch (phase)
                {
                    case 1:
                        Game1.playSound("cut");
                        break;
                    case 2:
                        tile1 += 2;
                        tile2 += 2;
                        tile3 += 2;
                        tile4 += 2;
                        break;
                    case 3:
                        tile1 += 4;
                        tile2 += 4;
                        tile3 += 4;
                        tile4 += 4;
                        break;
                }
                location.setMapTile(x1, y1, tile1, "Front", "SecretLab");
                location.setMapTile(x2, y1, tile2, "Front", "SecretLab");
                location.setMapTile(x1, y2, tile3, "Buildings", "SecretLab");
                location.setMapTile(x2, y2, tile4, "Buildings", "SecretLab");
            }
        }


        private void SetSpriteClose(int phase)
        {
            int x1 = this.x;
            int x2 = this.x + 1;
            int y1 = this.y;
            int y2 = this.y + 1;

            int tile1 = 686;
            int tile2 = 687;
            int tile3 = 734;
            int tile4 = 735;

            GameLocation location = this.location();

            switch (phase)
            {
                case 1:
                    Game1.playSound("cut");
                    break;
                case 2:
                    tile1 -= 2;
                    tile2 -= 2;
                    tile3 -= 2;
                    tile4 -= 2;
                    break;
                case 3:
                    tile1 -= 4;
                    tile2 -= 4;
                    tile3 -= 4;
                    tile4 -= 4;
                    break;
                case 4:
                    tile1 -= 7;
                    tile2 -= 7;
                    tile3 -= 7;
                    tile4 -= 7;
                    break;
            }
            location.setMapTile(x1, y1, tile1, "Front", "SecretLab");
            location.setMapTile(x2, y1, tile2, "Front", "SecretLab");
            location.setMapTile(x1, y2, tile3, "Buildings", "SecretLab");
            location.setMapTile(x2, y2, tile4, "Buildings", "SecretLab");
    }

        public void Close()
        {
            if (this.canClose)
            {
                this.canClose = false;
                this.SetSpriteClose(1);
                DelayedAction.functionAfterDelay(() =>
                {
                    this.SetSpriteClose(2);
                }, spriteSpeed);
                DelayedAction.functionAfterDelay(() =>
                {
                    this.SetSpriteClose(3);
                }, spriteSpeed * 2);
                DelayedAction.functionAfterDelay(() =>
                {
                    this.SetSpriteClose(4);
                    this.canOpen = true;
                }, spriteSpeed * 3);
            }
        }
    }

}
