using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3.Scripts
{
    public abstract class Scene
    {
        public abstract void Update(GameTime gameTime);
        public abstract void Draw();
    }
}
