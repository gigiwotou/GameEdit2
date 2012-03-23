using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WindowsGame1
{
    interface Unit
    {
        public Unit(Game1 game);
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spBatch);
    }
}
