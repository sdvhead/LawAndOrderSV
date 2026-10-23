using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Network;
using System.Xml.Serialization;

namespace LawAndOrderSV.SecretLab.Monsters
{
    public class BasicMonster : Monster
    {
        /*
        public BasicMonster() : base()
        {
            InitStats();
        }
        */
        public BasicMonster()
        {

        }

        [XmlIgnore]
        public NetFarmerRef lastAttacker = new NetFarmerRef();

        [XmlIgnore]
        public NetBool activated = new NetBool();

        public BasicMonster(Vector2 position) : base("BasicMonster", position)
        {
            Sprite.SpriteHeight = 20;
            Sprite.SpriteWidth = 16;

            base.Slipperiness = 24 + Game1.random.Next(-10, 11);
            Halt();

            base.IsWalkingTowardPlayer = false;
            base.HideShadow = true;

            this.Name = "BasicMonster";
            this.displayName = "BasicMonster";

            this.activated.Value = false;

            this.Health = 50;
            this.MaxHealth = 50;

            this.DamageToFarmer = 50;

            this.speed = 4;
            this.missChance.Value = 0.1f;

            Texture2D customTexture = ModEntry.imh.ModContent.Load<Texture2D>("assets/monsters/robug.png");
            this.Sprite.spriteTexture = customTexture;

        }

        /*
        public ShadowBrute()
        {
        }

        public ShadowBrute(Vector2 position)
            : base("Shadow Brute", position)
        {
            Sprite.SpriteHeight = 32;
            Sprite.UpdateSourceRect();
        }
        */

        /*public DwarvishSentry(Vector2 position) : base("Dwarvish Sentry", position)
        {
            Sprite.SpriteHeight = 16;
            base.IsWalkingTowardPlayer = false;
            Sprite.UpdateSourceRect();
            base.HideShadow = true;
            isGlider.Value = true;
            base.Slipperiness = 1;
            pauseTimer = 10000f;
            DelayedAction.playSoundAfterDelay("DwarvishSentry", 500);
        }*/

        /*
        private void InitStats()
        {
            this.Name = "BasicMonster";
            this.displayName = "BasicMonster";

            this.activated.Value = false;

            this.Health = 50;
            this.MaxHealth = 50;

            this.DamageToFarmer = 50;

            this.speed = 4;
            this.missChance.Value = 0.1f;

            Texture2D customTexture = ModEntry.imh.ModContent.Load<Texture2D>("assets/monsters/robat.png");
            this.Sprite.spriteTexture = customTexture;
        }*/

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


        protected override void initNetFields()
        {
            base.initNetFields();
            base.NetFields.AddField(lastAttacker.NetFields, "lastAttacker.NetFields").AddField(activated, "activated");
        }
        public override void behaviorAtGameTick(GameTime time)
        {
            if (Game1.IsMasterGame && !this.activated.Value)
            {
                if (PowerBoxFactory.powered_doorsbelts)
                {
                    //Sprite.currentFrame = 16;
                    xVelocity = 4f;
                    yVelocity = 0f;
                }
                return;
                /*
                timeUntilExplode.Value -= (float)time.ElapsedGameTime.TotalSeconds;
                
                if (timeUntilExplode.Value <= 0f)
                {
                    base.currentLocation.netAudio.StopPlaying("fuse");
                    timeUntilExplode.Value = 0f;
                    DropBomb();
                    base.Health = -9999;
                    return;
                }
                */
            }
            base.behaviorAtGameTick(time);
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
