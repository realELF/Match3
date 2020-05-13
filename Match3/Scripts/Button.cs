using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3.Scripts
{
    class Button
    {
        private readonly Action onClick;
        private readonly Vector2 position;
        private readonly string text;

        private bool underMouse;

        public Button(Action onClick,Vector2 position, string text)
        {
            this.onClick = onClick;
            this.position = position;
            this.text = text;
        }

        public void Update()
        {
            Rectangle rectangle = new Rectangle(position.ToPoint(), new Point(Match3.Instance.ContentContainer.button.Width, Match3.Instance.ContentContainer.button.Height));
            MouseState mouseState = Mouse.GetState();
            underMouse = rectangle.Contains(new Point(mouseState.X, mouseState.Y));

            if (underMouse && mouseState.LeftButton == ButtonState.Pressed)
                onClick();
        }

        public void Draw()
        {
            Match3.Instance.SpriteBatch.Draw(Match3.Instance.ContentContainer.button, position, null, underMouse ? Color.Black : Color.White, 0, Vector2.One, Vector2.One, SpriteEffects.None, 0.2f);
            Match3.Instance.SpriteBatch.DrawString(Match3.Instance.ContentContainer.font, text, new Vector2(position.X + 60, position.Y + 20), Color.White, 0, Vector2.One, 1, SpriteEffects.None, 0.2f);
        }
    }
}