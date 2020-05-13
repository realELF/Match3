using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3.Scripts
{
    class Explosion:Effect
    {
        private int timer = 250;
        private Point pos;

        private Explosion(Point pos)
        {
            this.pos = pos;
        }

        public static void Create(Point pos)
        {
            (Match3.Instance.scene as Gameplay).effects.Add(new Explosion(pos));
        }

        public override void Update(Gameplay scene, GameTime gameTime)
        {
            if (timer < 0)
            {
                for (int x = Clamp(pos.X - 1, 0, Gameplay.FIELD_SIZE - 1); x <= Clamp(pos.X + 1, 0, Gameplay.FIELD_SIZE - 1); x++)
                    for (int y = Clamp(pos.Y - 1, 0, Gameplay.FIELD_SIZE - 1); y <= Clamp(pos.Y + 1, 0, Gameplay.FIELD_SIZE - 1); y++)
                        if (scene.field[x, y] != null)
                        {
                            Point tempPoint=new Point(x, y);
                            scene.field[x, y].Destroy(tempPoint, scene);
                            scene.RefreshColumn(tempPoint);
                        }

                scene.effects.Remove(this);
            }
            else
                timer -= gameTime.ElapsedGameTime.Milliseconds;
        }
        public override void Draw()
        {

        }

        public static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
    }
}
