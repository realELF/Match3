using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3.Scripts
{
    class Element
    {
        public enum ElementType { Usual, HorizontalLine, VerticalLine, Bomb }//Можно было создать дочерний класс и перегрузить метод Destroy, но мне показалось, что слишком мало изменений для создания нового класса

        public const float FALL_SPEED = 250;
        public const float SWAP_SPEED = 15;

        public readonly ElementType elementType;
        public readonly Color color;
        public bool inFall;

        public Element(ElementType elementType)
        {
            this.elementType = elementType;

            switch (Match3.Instance.rnd.Next(5))
            {
                case 1:
                    color = Color.Purple;
                    break;
                case 2:
                    color = Color.Yellow;
                    break;
                case 3:
                    color = Color.Green;
                    break;
                case 4:
                    color = Color.Blue;
                    break;
                default:
                    color = Color.Red;
                    break;
            }
        }

        public void Destroy(Point pos, Gameplay scene)
        {
            scene.points++;

            switch (elementType)
            {
                case ElementType.HorizontalLine:
                case ElementType.VerticalLine:
                    Destroyer.Create(pos, elementType);
                    break;
                case ElementType.Bomb:
                    Explosion.Create(pos);
                    break;
                default:
                    break;
            }

            scene.field[pos.X, pos.Y] = null;
        }

        public void Draw(Vector2 pos)
        {
            Texture2D texture2D;
            switch (elementType)
            {
                case ElementType.HorizontalLine:
                case ElementType.VerticalLine:
                    texture2D = Match3.Instance.ContentContainer.line;
                    break;
                case ElementType.Bomb:
                    texture2D = Match3.Instance.ContentContainer.bomb;
                    break;
                default:
                    texture2D = Match3.Instance.ContentContainer.triangle;
                    break;
            }

            Match3.Instance.SpriteBatch.Draw(texture2D, pos, null, color, 0, Vector2.One, Vector2.One, SpriteEffects.None, 0.1f);
        }
    }
}