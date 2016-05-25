using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2DPlatformer.GL
{
    static class Art
    {
        static ContentManager Content;

        public static Texture2D Cursor;
        public static Texture2D Save;
        public static Texture2D SaveMO;
        public static Texture2D Import;
        public static Texture2D ImportMO;
        public static Texture2D New;
        public static Texture2D NewMO;
        public static Texture2D CharacterFront;
        public static Texture2D CharacterWalkRight;

        public static Texture2D HeartFull;
        public static Texture2D HeartHalf;
        public static Texture2D HeartHalfRight;
        public static Texture2D HeartEmpty;

        public static Texture2D Coin;

        public static Texture2D Cloud;

        public static SpriteFont Font;
        public static SpriteFont BoldFont;

        public static void LoadContent(ContentManager content)
        {
            Content = content;
            Cursor = Content.Load<Texture2D>("art/cursor");
            Save = Content.Load<Texture2D>("art/UI/Editor/btnSave");
            SaveMO = Content.Load<Texture2D>("art/UI/Editor/btnSave_Click");
            Import = Content.Load<Texture2D>("art/UI/Editor/btnImport");
            ImportMO = Content.Load<Texture2D>("art/UI/Editor/btnImport_Click");
            New = Content.Load<Texture2D>("art/UI/Editor/btnNew");
            NewMO = Content.Load<Texture2D>("art/UI/Editor/btnNew_Click");
            CharacterFront = Content.Load<Texture2D>("art/character/front");
            CharacterWalkRight = Content.Load<Texture2D>("art/character/walkRight");

            HeartFull = Content.Load<Texture2D>("art/HUD/heartFull");
            HeartHalf = Content.Load<Texture2D>("art/HUD/heartHalf");
            HeartHalfRight = Content.Load<Texture2D>("art/HUD/heartHalf_right");
            HeartEmpty = Content.Load<Texture2D>("art/HUD/heartEmpty");

            Coin = Content.Load<Texture2D>("art/items/coin");

            Cloud = Content.Load<Texture2D>("art/cloud");


            Font = Content.Load<SpriteFont>("font/font");
            BoldFont = Content.Load<SpriteFont>("font/boldFont");
        }

        public static T GetContent<T>(string Path)
        {
            return Content.Load<T>(Path);
        }
    }
}
