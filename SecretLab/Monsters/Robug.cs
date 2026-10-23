using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Monsters;
using System;

namespace LawAndOrderSV.SecretLab.Monsters
{
    public class Robug : ShadowBrute
    {
        private double activationTimer;
        private int activationFrameIndex;
        private bool activated;
        private const int activationRadius = 4;

        public Robug(Vector2 position)
            : base(position)
        {
            Texture2D tex = ModEntry.imh.ModContent.Load<Texture2D>("assets/monsters/robug.png");
            this.Sprite.spriteTexture = tex;

            this.Name = "Robug";
            this.Sprite.currentFrame = 16;

            // Prevent ANY AI from running before activation
            this.moveTowardPlayerThreshold.Value = int.MaxValue;
            this.controller = null;
        }

        public override void behaviorAtGameTick(GameTime time)
        {
            if (!activated)
            {
                float dist = Vector2.Distance(Game1.player.Position, this.Position);

                if (dist <= activationRadius * 64)
                {
                    activationTimer += time.ElapsedGameTime.TotalMilliseconds;

                    if (activationTimer >= 120)
                    {
                        activationTimer = 0;
                        activationFrameIndex++;

                        if (activationFrameIndex <= 4)
                        {
                            this.Sprite.currentFrame = 16 + activationFrameIndex;
                        }
                        else
                        {
                            this.Sprite.currentFrame = 0;
                            activated = true;

                            // Now allow ShadowBrute AI to run normally
                            this.moveTowardPlayerThreshold.Value = 999;
                        }

                        this.Sprite.UpdateSourceRect();
                    }
                }

                // BLOCK ALL SHADOWBRUTE AI
                this.controller = null;
                this.Halt();
                return;
            }

            // AFTER ACTIVATION: run ShadowBrute AI normally
            base.behaviorAtGameTick(time);
        }

        protected override void localDeathAnimation()
        {
            /*
            Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite(45, base.Position, Color.White, 10), base.currentLocation);
            for (int i = 1; i < 3; i++)
            {
                base.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, base.Position + new Vector2(0f, 1f) * 64f * i, Color.Gray * 0.75f, 10)
                {
                    delayBeforeAnimationStart = i * 159
                });
                base.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, base.Position + new Vector2(0f, -1f) * 64f * i, Color.Gray * 0.75f, 10)
                {
                    delayBeforeAnimationStart = i * 159
                });
                base.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, base.Position + new Vector2(1f, 0f) * 64f * i, Color.Gray * 0.75f, 10)
                {
                    delayBeforeAnimationStart = i * 159
                });
                base.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, base.Position + new Vector2(-1f, 0f) * 64f * i, Color.Gray * 0.75f, 10)
                {
                    delayBeforeAnimationStart = i * 159
                });
            }
            */
            base.currentLocation.localSound("shadowDie");
            return;
        }

        public override void updateMovement(GameLocation location, GameTime time)
        {
            if (!activated)
            {
                // BLOCK MOVEMENT BEFORE ACTIVATION
                this.controller = null;
                this.Halt();
                return;
            }

            base.updateMovement(location, time);
        }
    }
}
