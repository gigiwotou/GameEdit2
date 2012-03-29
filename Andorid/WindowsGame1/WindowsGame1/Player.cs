using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using Microsoft.Xna.Framework;
using GameData;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    class Player : Unit
    {
        AnimationSet mAnimationset;
        Animation mAnimation;
        Game1 mGame;

        int mTime = 0;
        public int Time { get { return mTime; } }

        System.Drawing.Drawing2D.Matrix mViewMatrix = new System.Drawing.Drawing2D.Matrix();

        public Player(Game1 game)
        {
            mGame = game;
            mAnimationset = AnimationSetManager.Instance.Get("tuzi");
            mAnimation = mAnimationset.Animations[0];
        }
        public void Draw(SpriteBatch spBatch)
        {
            if (mAnimation != null && mAnimation.AnimTracks.Count > 0)
                Draw(mAnimation.AnimTracks, spBatch);

            //throw new NotImplementedException();
        }

        public void Draw(List<AnimationTrack> tracks, SpriteBatch spBatch)
        {
            // reverse render, the lower the first.
            for (int i = tracks.Count - 1; i >= 0; i--)
            {
                AnimationTrack track = tracks[i];
                if (!track.Enabled)
                    continue;

                Draw(track, spBatch);

                // child is upon the parent.
                if (track.AnimTracks.Count > 0)
                    Draw(track.AnimTracks, spBatch);
            }
        }

        public void Draw(AnimationTrack track, SpriteBatch spBatch)
        {
            System.Drawing.Drawing2D.Matrix worldMatrix = new System.Drawing.Drawing2D.Matrix();
            worldMatrix = track.CachedMatrix;

            // fetch the image out.
            Microsoft.Xna.Framework.Rectangle rect;
            Texture2D texture = GetTexture(track, out rect);

            System.Drawing.Drawing2D.Matrix finalTransform = mViewMatrix.Clone();
            finalTransform.Multiply(worldMatrix);
            float[] gM = finalTransform.Elements;

            Microsoft.Xna.Framework.Matrix xM = new Microsoft.Xna.Framework.Matrix();
           
            xM.M11 = gM[0];
            xM.M12 = gM[1];
            xM.M21 = gM[2];
            xM.M22 = gM[3];
            xM.M41 = gM[4];
            xM.M42 = gM[5];
            xM.M33 = 1;
            xM.M44 = 1;
            Microsoft.Xna.Framework.Rectangle dRect = new Microsoft.Xna.Framework.Rectangle();
            dRect.X = 100;
            dRect.Y = 100;
            dRect.Width = rect.Width;
            dRect.Height = rect.Height;

            spBatch.Begin(SpriteSortMode.Immediate,BlendState.AlphaBlend,SamplerState.LinearClamp,DepthStencilState.None,RasterizerState.CullCounterClockwise,null,xM);
            spBatch.Draw(texture, dRect, rect, Microsoft.Xna.Framework.Color.White);
            spBatch.End();
        }

        Texture2D GetTexture(AnimationTrack track, out Microsoft.Xna.Framework.Rectangle rect1)
        {
            GameData.Image image = track.CachedImage;
            Texture2D texture = image != null ? image.Imageset.Tag as Texture2D : null;
            if (texture != null)
            {
                int row = track.ImageRow;
                int col = track.ImageColum;
                int start = track.ImageStart;
                int offset = (int)track.CachedKey.ImageIndexOffset;

                int width = image.Width / col;
                int height = image.Height / row;
                int remainder;
                int quotient = Math.DivRem(offset, col, out remainder);
                if (quotient >= row)
                    quotient = row - 1;

                int x = remainder * width;
                int y = quotient * height;
                rect1 = new Microsoft.Xna.Framework.Rectangle(image.X + x, image.Y + y, width, height);
                return texture;
            }
            Texture2D t2d = mGame.Content.Load<Texture2D>("Missing");
            rect1 = new Microsoft.Xna.Framework.Rectangle(0, 0, t2d.Width, t2d.Height);
            return mGame.Content.Load<Texture2D>("Missing"); 
        }

        public void Update(GameTime gameTime)
        {
            if (mAnimation == null)
                return;
            mTime += gameTime.ElapsedGameTime.Milliseconds;
            while (mTime > mAnimation.Time)
                mTime -= mAnimation.Time;

            foreach (AnimationTrack track in mAnimation.AnimTracks)
            {
                track.FlushImage();
                track.SetTimePosition(mTime);
                track.FlushTransform();
            }
            //throw new NotImplementedException();
        }
    }
}
