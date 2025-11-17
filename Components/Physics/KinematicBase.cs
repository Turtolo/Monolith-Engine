using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using ConstructEngine.Area;
using ConstructEngine.Objects;

namespace ConstructEngine.Components
{
    public class KinematicBase : Component
    {
        //public Rectangle Hitbox;
        public Area2D Collider;
        public Vector2 Velocity;
        private float remainderX = 0;
        private float remainderY = 0;

        public Vector2 Position
        {
            get => new Vector2(Collider.Rect.X, Collider.Rect.Y);
            set
            {
                var rect = Collider.Rect;
                rect.X = (int)value.X;
                rect.Y = (int)value.Y;
                Collider.Rect = rect;
            }
        }


        public bool Locked;


        public KinematicBase(object root) : base(root) { }

    
        

        public void UpdateCollider(GameTime gameTime)
        {
            if (!Locked)
            {
                float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

                Move(Velocity.X * deltaTime, Velocity.Y * deltaTime);

                foreach (var collider in Area2D.AreaList)
                {

                    if (!collider.Enabled || collider.Velocity == Vector2.Zero || collider.Root.GetType() != typeof(CollisionObject))
                        continue;

                    Rectangle feetCheck = Collider.Rect;
                    feetCheck.Y += 1;

                    if (feetCheck.Intersects(collider.Rect))
                    {
                        Collider.Rect.X += (int)collider.Velocity.X;
                        Collider.Rect.Y += (int)collider.Velocity.Y;
                    }
                }
            }

            else
            {
                Velocity.X = 0;
                Velocity.Y = 0;
            }
        }


        public void Move(float moveX, float moveY)
        {
            remainderX += moveX;
            int mx = (int)MathF.Round(remainderX);
            remainderX -= mx;
            MoveX(mx);

            remainderY += moveY;
            
            int my = (int)MathF.Round(remainderY);
            remainderY -= my;
            MoveY(my);
            
        }

        public void MoveX(int amount)
        {
            int sign = Math.Sign(amount);
            
            while (amount != 0)
            {
                Rectangle test = Collider.Rect;
                
                test.X += sign;
                
                

                if (!IsColliding(test))
                    Collider.Rect = test;
                else
                {
                    Velocity.X = 0;
                    break;
                }

                amount -= sign;
                
                
            }
        }

        public void MoveY(int amount)
        {
            int sign = Math.Sign(amount);
            while (amount != 0)
            {
                Rectangle test = Collider.Rect;
                test.Y += sign;

                if (!IsColliding(test))
                    Collider.Rect = test;
                else
                {
                    Velocity.Y = 0;
                    break;
                }

                amount -= sign;
            }
        }

        public bool IsOnGround()
        {
            Rectangle test = Collider.Rect;
            test.Y += 1;
            return IsColliding(test);
        }

        public bool IsOnWall()
        {
            Rectangle leftCheck = Collider.Rect;
            leftCheck.X -= 1;

            Rectangle rightCheck = Collider.Rect;
            rightCheck.X += 1;

            foreach (var collider in Area2D.AreaList)
            {
                if (!collider.Enabled || collider.Root.GetType() != typeof(CollisionObject))
                    continue;

                if (leftCheck.Intersects(collider.Rect) || rightCheck.Intersects(collider.Rect))
                    return true;
            }

            return false;
        }

        public bool IsColliding(Rectangle rect)
        {
            foreach (var collider in Area2D.AreaList)
            {
                if (!collider.Enabled || collider.Root.GetType() != typeof(CollisionObject))
                {
                    continue;
                }
                if (rect.Intersects(collider.Rect))
                    return true;
            }
            return false;
        }
    }
}