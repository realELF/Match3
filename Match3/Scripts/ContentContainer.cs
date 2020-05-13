using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3.Scripts
{
    public class ContentContainer
    {
        public readonly Texture2D cell;
        public readonly Texture2D triangle;
        public readonly Texture2D line;
        public readonly Texture2D bomb;
        public readonly Texture2D destroyer;
        public readonly Texture2D button;

        public readonly SpriteFont font;

        public ContentContainer()
        {
            cell = Match3.Instance.Content.Load<Texture2D>("cell");
            triangle = Match3.Instance.Content.Load<Texture2D>("triangle");
            line = Match3.Instance.Content.Load<Texture2D>("line");
            bomb = Match3.Instance.Content.Load<Texture2D>("bomb");
            destroyer = Match3.Instance.Content.Load<Texture2D>("destroyer");
            button = Match3.Instance.Content.Load<Texture2D>("button");

            font = Match3.Instance.Content.Load<SpriteFont>("font");
        }
    }
}
