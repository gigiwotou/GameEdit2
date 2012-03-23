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
        Microsoft.Xna.Framework.Matrix mViewMatrix = new Microsoft.Xna.Framework.Matrix();

        public Player(Game1 game)
        {
            mGame = game;
            mAnimationset = AnimationSetManager.Instance.Get("tuzi");
            mAnimation = mAnimationset.Animations[1];
        }
        public void Draw(SpriteBatch spBatch)
        {
            if (mAnimation != null && mAnimation.AnimTracks.Count > 0)
                Draw(mAnimation.AnimTracks, spBatch);

            throw new NotImplementedException();
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
            Microsoft.Xna.Framework.Matrix worldMatrix = track.CachedMatrix;

            // fetch the image out.
            Microsoft.Xna.Framework.Rectangle rect;
            Texture2D texture = GetTexture(track, out rect);

            Microsoft.Xna.Framework.Matrix finalTransform = mViewMatrix.Clone();
            finalTransform.Multiply(worldMatrix);
            //g.Transform = finalTransform;

            // draw the image.
            //g.DrawImage(texture, new PointF(-texture.Width * 0.5f, -texture.Height * 0.5f));
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

                int width = texture.Width / col;
                int height = texture.Height / row;
                int remainder;
                int quotient = Math.DivRem(offset, col, out remainder);
                if (quotient >= row)
                    quotient = row - 1;

                int x = remainder * width;
                int y = quotient * height;
                rect1 = new Microsoft.Xna.Framework.Rectangle(x, y, width, height);
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
            foreach (AnimationTrack track in mAnimation.AnimTracks)
            {
                track.SetTimePosition(gameTime.ElapsedGameTime.Milliseconds);
                track.FlushTransform();
            }
            throw new NotImplementedException();
        }
    }
}
