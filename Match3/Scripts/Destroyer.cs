using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3.Scripts
{
    class Destroyer : Effect
    {
        public const float SPEED = 300;

        private Point pos;
        private Element.ElementType elementType;
        private bool direction;
        private float deltaPos;

        private Destroyer(Point pos, Element.ElementType elementType, bool direction)
        {
            this.pos = pos;
            this.elementType = elementType;
            this.direction = direction;

            deltaPos = 0;
        }

        public static void Create(Point pos, Element.ElementType elementType)
        {
            if (elementType != Element.ElementType.HorizontalLine && elementType != Element.ElementType.VerticalLine)
                return;

            Gameplay scene = Match3.Instance.scene as Gameplay;
            scene.effects.Add(new Destroyer(pos, elementType, false));
            scene.effects.Add(new Destroyer(pos, elementType, true));
        }

        public void Destroy()
        {
            (Match3.Instance.scene as Gameplay).effects.Remove(this);
        }

        public override void Update(Gameplay scene, GameTime gameTime)
        {
            if (deltaPos > Match3.Instance.ContentContainer.cell.Width)// TODO: Доработать, если, спрайт клетки будет другим [cellTexture.Width!=cellTexture.Height]
            {
                if (elementType == Element.ElementType.HorizontalLine)
                    if (direction)
                        if (pos.X > 0)
                            pos = new Point(pos.X - 1, pos.Y);
                        else
                        {
                            Destroy();
                            return;
                        }
                    else
                        if (pos.X < Gameplay.FIELD_SIZE - 1)
                            pos = new Point(pos.X + 1, pos.Y);
                        else
                        {
                            Destroy();
                            return;
                        }
                else
                    if (direction)
                        if (pos.Y > 0)
                            pos = new Point(pos.X, pos.Y - 1);
                        else
                        {
                            Destroy();
                            return;
                        }
                    else
                        if (pos.Y < Gameplay.FIELD_SIZE - 1)
                            pos = new Point(pos.X, pos.Y + 1);
                        else
                        {
                            Destroy();
                            return;
                        }

                deltaPos = 0;
                if (scene.field[pos.X, pos.Y] != null)
                    scene.field[pos.X, pos.Y].Destroy(pos, scene);
                scene.RefreshColumn(pos);
            }

            deltaPos += gameTime.ElapsedGameTime.Milliseconds / 1000f * SPEED;
        }

        public override void Draw()
        {
            Texture2D cellTexture = Match3.Instance.ContentContainer.cell;

            float deltaX = 0;
            float deltaY = 0;
            if (elementType == Element.ElementType.HorizontalLine)
                if (direction)
                    deltaX -= deltaPos;
                else
                    deltaX += deltaPos;
            else
                if (direction)
                    deltaY -= deltaPos;
                else
                    deltaY += deltaPos;

            Match3.Instance.SpriteBatch.Draw(
                Match3.Instance.ContentContainer.destroyer
                , new Vector2(cellTexture.Width * pos.X + Gameplay.LEFT_INDENT + deltaX, cellTexture.Height * pos.Y + Gameplay.TOP_INDENT + deltaY)
                , null
                , Color.White
                , 0
                , Vector2.One
                , Vector2.One
                , SpriteEffects.None
                , 0.15f
                );
        }
    }
}