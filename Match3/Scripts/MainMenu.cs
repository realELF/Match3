using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3.Scripts
{
    public class MainMenu : Scene
    {
        private Button button;

        public MainMenu()
        {
            button = new Button(() => Match3.Instance.scene = new Gameplay(), new Vector2(300, 200), "Play");
        }

        public override void Update(GameTime gameTime)
        {
            button.Update();
        }

        public override void Draw()
        {
            button.Draw();
        }
    }
}