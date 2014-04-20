using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LevelGame
{
    class AnimatedSprite
    {
        public Rectangle rect;
        Texture2D idle;
        Texture2D run;
        Texture2D dig;
        Texture2D jump;

        bool isJumping;
        bool isRunning;
        bool isSlowMode;
        bool isRunningRight;
        bool isRunnignDown;
        public bool isDigging;

        float yVelocity;
        float maxYVelocity = 10;
        float g = 0.2f;

        int frameWidth;
        int frameHeight;

        public int Frames
        {
            get
            {
                return run.Width / frameWidth;
            }
        }

        int currentFrame;
        int timeElapsed;
        int timeForFrame = 100;

        Game1 game;

        public AnimatedSprite(Rectangle rect, Texture2D idle, Texture2D run, Texture2D dig,Texture2D jump, Game1 game)
        {
            this.rect = rect;
            this.idle = idle;
            this.run = run;
            this.dig = dig;
            this.jump = jump;

            frameWidth = 55;
            frameHeight = run.Height;

            this.game = game;
        }

        public void SwitchModes()
        {
            isSlowMode = !isSlowMode;
        }
        public void StartRun(bool isRight,bool isDown)
        {
            if (!isRunning)
            {
                isRunning = true;
                currentFrame = 0;
                timeElapsed = 0;
            }
            isRunningRight = isRight;
            isRunnignDown=isDown;
        }
        public void Stop()
        {
            isRunning = false;
        }
        public void Dig()
        {
            if (!isDigging)
            {
                isDigging = true;
                currentFrame = 0;
                timeElapsed = 0;
            }
            else
            {
                isDigging = false;
            }
            //isRunningRight = true;
        }
        public void Jump()
        {
            if (!isJumping && yVelocity == 0.0f)
            {
                isJumping = true;
                currentFrame = 0;
                timeElapsed = 0;
                yVelocity = maxYVelocity;
            }
        }
        public void ApplyGravity(GameTime gameTime)
        {
            yVelocity = yVelocity - g * gameTime.ElapsedGameTime.Milliseconds / 5;
            float dy = yVelocity * gameTime.ElapsedGameTime.Milliseconds / 10;

            Rectangle nextPosition = rect;
            nextPosition.Offset(0, -(int)dy);

            Rectangle boudingRect = GetBoundingRect(nextPosition);
            if (boudingRect.Top > 0 && boudingRect.Bottom < game.Height
                && !game.CollidesWithLevel(boudingRect))
                rect = nextPosition;

            bool collideOnFallDown = (game.CollidesWithLevel(boudingRect) && yVelocity < 0);

            if (boudingRect.Bottom > game.Height || collideOnFallDown)
            {
                yVelocity = 0;
                isJumping = false;
            }
        }
        public void Update(GameTime gameTime)
        {
            int counter=0;
            timeElapsed += gameTime.ElapsedGameTime.Milliseconds;
            int tempTime = timeForFrame;
            if (isSlowMode)
                tempTime *= 4;
            if (timeElapsed > tempTime)
            {
                currentFrame = (currentFrame + 1) % Frames;
                timeElapsed = 0;
                counter++;
            }

            if (isRunning)
            {
                int dx = 3 * gameTime.ElapsedGameTime.Milliseconds / 10;
                int dy = 3 * gameTime.ElapsedGameTime.Milliseconds / 10;
                if (!isRunningRight)
                    dx = -dx;
                Rectangle nextPosition = rect;
                if (isRunnignDown) nextPosition.Offset(0, dy);
                else nextPosition.Offset(dx, 0);
                Rectangle boudingRect = GetBoundingRect(nextPosition);
                Rectangle screenRect = Game1.GetScreenRect(boudingRect);

                if (screenRect.Left > 0 && screenRect.Right < game.Width
                    && !game.CollidesWithLevel(boudingRect))
                {
                    rect = nextPosition;
                    game.DestroyBlock(rect);
                }
            }
            ApplyGravity(gameTime);
        }

        private Rectangle GetBoundingRect(Rectangle rectangle)
        {
            int width = (int)(rectangle.Width * 0.4f);
            int x = rectangle.Left + (int)(rectangle.Width * 0.2f);

            return new Rectangle(x, rectangle.Top, width, rectangle.Height);
        }
        /*public bool CollideWith(AnimatedSprite b)
        {
            /*if (IsIntersect(this._boundingBox,
                            b._boundingBox))
            {
                return PerPixelCollisions(b);
            } else return false;

        }*/

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle r = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);
            SpriteEffects effects = SpriteEffects.None;
            if (!isRunningRight)
                effects = SpriteEffects.FlipHorizontally;

            Rectangle screenRect = Game1.GetScreenRect(rect);
            spriteBatch.Begin();
            if (isDigging)
            {
                spriteBatch.Draw(dig, screenRect, r, Color.White, 0, Vector2.Zero, effects, 0);
            }
            else if (isJumping)
            {
                spriteBatch.Draw(jump, screenRect, r, Color.White, 0, Vector2.Zero, effects, 0);
            }
            else if (isRunning)
            {
                spriteBatch.Draw(run, screenRect, r, Color.White, 0, Vector2.Zero, effects, 0);
            }
            else
            {
                spriteBatch.Draw(idle, screenRect, Color.White);
            }
            spriteBatch.End();
        }
    }
}
