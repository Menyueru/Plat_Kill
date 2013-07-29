using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SkinnedModel;

namespace plat_kill.GameModels
{
    public interface GameModel
    {
        void Load(ContentManager content, String path);

        void Draw(Matrix view, Matrix projection);

        void Update(GameTime gameTime);

    }
}
