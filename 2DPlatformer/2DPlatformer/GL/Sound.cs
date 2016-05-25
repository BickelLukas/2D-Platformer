using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2DPlatformer.GL
{
    static class Sound
    {
        public static SoundEffect coin;

        public static void Load(ContentManager content)
        {
            coin = content.Load<SoundEffect>("sound/coin");
        }
    }
}
