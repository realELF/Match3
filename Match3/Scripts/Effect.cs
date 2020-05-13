using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3.Scripts
{
    abstract class Effect
    {
        public abstract void Update(Gameplay scene, Microsoft.Xna.Framework.GameTime gameTime);
        public abstract void Draw();
    }
}
