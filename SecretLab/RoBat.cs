using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;

namespace LawAndOrderSV.SecretLab
{
    public class RoBat : Bat
    {
        public RoBat(): base()
        {
            InitStats();
        }

        public RoBat(Vector2 position) : base(position)
        {
            InitStats();
        }

        private void InitStats()
        {
            this.Name = "RoBat";
            this.displayName = "RoBat";

            this.Health = 50;
            this.MaxHealth = 50;

            this.DamageToFarmer = 50;

            this.speed = 4;
            this.missChance.Value = 0.1f;

            Texture2D customTexture = ModEntry.imh.ModContent.Load<Texture2D>("assets/monsters/robat.png");
            this.Sprite.spriteTexture = customTexture;
        }
        
        public override void update(GameTime time, GameLocation location)
        {
            base.update(time, location);

            /*
            // Custom logic per tick (e.g., sound effects or particle tweaks)
            if (Game1.random.NextDouble() < 0.005)
            {
                this.currentLocation?.playSound("slimedead");
            }
            */
        }


        public override int takeDamage(int damage, int xTile, int yTile, bool isBomb, double addedPrecision, Farmer who)
        {
            int actualDamage = base.takeDamage(damage, xTile, yTile, isBomb, addedPrecision, who);

            // Check if the monster has just died from this hit
            if (this.Health <= 0 && this.currentLocation != null)
            {
                // Custom loot drop logic on death
                // Stardew Valley 1.6 uses ItemRegistry for streamlined object creation
                //Game1.createItemDebris(ItemRegistry.Create("(O)680"), this.Position, 1, this.currentLocation);
            }

            return actualDamage;
        }

    }
}
