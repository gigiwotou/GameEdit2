using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GameData;
using System.Drawing.Drawing2D;
using GameEditor.Properties;
using GameEditor.Utility;
using System.IO;

namespace GameEditor.Controls
{
    public partial class AnimationPreviewPanel : DoubleBuffControl
    {
        enum TransformMode
        {
            Center = 0,
            Move,
            Edge0,
            Edge1,
            Edge2,
            Edge3,
            Rotate,
        };

        TransformMode mTransformMode;
        Cursor mRotateCursor;
        Pen mLinePen = new Pen(Color.Black);
        SolidBrush mLineBrush = new SolidBrush(Color.Black);
        Matrix mViewMatrix = new Matrix();

        int mTimePosition = 0;
        Animation mAnimation = null;
        AnimationTrack mAnimTrack = null;
        AnimationControl mAnimationControl;

        public AnimationPreviewPanel(AnimationControl control)
        {
            mAnimationControl = control;

            InitializeComponent();

            MemoryStream stream = new MemoryStream(Resources.Rotate);
            mRotateCursor = new Cursor(stream);
        }

        public int TimePosition { get { return mTimePosition; } }
        public void SetTimePosition(int time)
        {
            mTimePosition = time;

            if (mAnimation != null)
            {
                foreach (AnimationTrack track in mAnimation.AnimTracks)
                {
                    track.SetTimePosition(mTimePosition);
                    track.FlushTransform();
                }
            }

            ForceUpdate();
        }

        public override void ForceUpdate()
        {
            base.ForceUpdate();

            Graphics g = Graphics;

            // setup the view matrix, no projection matrix?
            mViewMatrix = new Matrix();
            g.Transform = mViewMatrix;

            // render the background.
            g.DrawLine(mLinePen, 0.0f, Size.Height * 0.5f, Size.Width, Size.Height * 0.5f);
            g.DrawLine(mLinePen, Size.Width * 0.5f, 0, Size.Width * 0.5f, Size.Height);

            mViewMatrix.Translate(Size.Width * 0.5f, Size.Height * 0.5f);

            // render the animation.
            if (mAnimation != null && mAnimation.AnimTracks.Count > 0)
            {
                Draw(mAnimation.AnimTracks, g);

                if (mAnimTrack != null)
                    DrawHelper(mAnimTrack, g);
            }

            Invalidate();
        }

        void Draw(List<AnimationTrack> tracks, Graphics g)
        {
            // reverse render, the lower the first.
            for (int i = tracks.Count - 1; i >= 0; i--)
            {
                AnimationTrack track = tracks[i];
                if (!track.Enabled)
                    continue;

                Draw(track, g);
                
                // child is upon the parent.
                if (track.AnimTracks.Count > 0)
                    Draw(track.AnimTracks, g);
            }
        }

        void Draw(AnimationTrack track, Graphics g)
        {
            Matrix worldMatrix = track.CachedMatrix;

            // fetch the image out.
            System.Drawing.Image texture = GetTexture(track);

            Matrix finalTransform = mViewMatrix.Clone();
            finalTransform.Multiply(worldMatrix);
            g.Transform = finalTransform;

            // draw the image.
            g.DrawImage(texture, new PointF(-texture.Width * 0.5f, -texture.Height * 0.5f));
        }

        System.Drawing.Image GetTexture(AnimationTrack track)
        {
            GameData.Image image = track.CachedImage;
            System.Drawing.Image texture = image != null ? image.Imageset.Tag as System.Drawing.Image : null;
           
            if (texture != null)
            {
                Bitmap srcBitmap1 = new Bitmap(texture);
                Rectangle rect1 = new Rectangle(image.X, image.Y, image.Width, image.Height);
                texture = srcBitmap1.Clone(rect1, srcBitmap1.PixelFormat);

                int row = track.ImageRow;
                int col = track.ImageColum;
                int start = track.ImageStart;
                int offset = (int)track.CachedKey.ImageIndexOffset;
                
                Bitmap srcBitmap = new Bitmap(texture);
                int width = texture.Width / col;
                int height = texture.Height / row;
                int remainder;
                int quotient = Math.DivRem(offset, col, out remainder);
                if (quotient >= row)
                    quotient = row - 1;

                int x = remainder * width;
                int y = quotient * height;

                Rectangle rect = new Rectangle(x, y, width, height);
                texture = srcBitmap.Clone(rect, srcBitmap.PixelFormat);

                return texture;
            }
            return Resources.Missing;
        }

        PointF[] GetHelperPoints(AnimationTrack track)
        {
            Matrix worldMatrix = track.CachedMatrix;
            Matrix finalTransform = mViewMatrix.Clone();
            finalTransform.Multiply(worldMatrix);

            // fetch the image out.
            System.Drawing.Image texture = GetTexture(track);
            PointF[] helperPoints = new PointF[]{
                new PointF(-texture.Width * 0.5f, -texture.Height * 0.5f), // top-left
                new PointF(+texture.Width * 0.5f, -texture.Height * 0.5f), // top-right
                new PointF(+texture.Width * 0.5f, +texture.Height * 0.5f), // bottom-right
                new PointF(-texture.Width * 0.5f, +texture.Height * 0.5f), // bottom-left
                new PointF(track.CachedKey.Center.X / track.CachedKey.Scale.X,
                    track.CachedKey.Center.Y / track.CachedKey.Scale.Y)};

            finalTransform.TransformPoints(helperPoints);

            return helperPoints;
        }

        const float HelperKitWidth = 5.0f;
        Pen mHelperPen = new Pen(Color.Red);
        void DrawHelper(AnimationTrack track, Graphics g)
        {
            PointF[] helperPoints = GetHelperPoints(track);

            // helper bounds.
            PointF[] helperBound = new PointF[] {
                helperPoints[0],
                helperPoints[1],
                helperPoints[2],
                helperPoints[3],
                helperPoints[0]};

            // helper corner bounds.
            RectangleF[] helperRects = new RectangleF[]{
                new RectangleF( // top-left
                    helperPoints[0].X - HelperKitWidth * 0.5f, helperPoints[0].Y - HelperKitWidth * 0.5f,
                    HelperKitWidth, HelperKitWidth),
                new RectangleF( // top-right
                    helperPoints[1].X - HelperKitWidth * 0.5f, helperPoints[1].Y - HelperKitWidth * 0.5f,
                    HelperKitWidth, HelperKitWidth),
                new RectangleF( // bottom-left
                    helperPoints[2].X - HelperKitWidth * 0.5f, helperPoints[2].Y - HelperKitWidth * 0.5f,
                    HelperKitWidth, HelperKitWidth),
                new RectangleF( // bottom-right
                    helperPoints[3].X - HelperKitWidth * 0.5f, helperPoints[3].Y - HelperKitWidth * 0.5f,
                    HelperKitWidth, HelperKitWidth)};

            // helper center.
            PointF[] helperCenter = new PointF[] {
                helperPoints[4] - new SizeF(-HelperKitWidth, 0),
                helperPoints[4] - new SizeF(0, +HelperKitWidth),
                helperPoints[4] - new SizeF(+HelperKitWidth, 0),
                helperPoints[4] - new SizeF(0, -HelperKitWidth),
                helperPoints[4] - new SizeF(-HelperKitWidth, 0),
                helperPoints[4] - new SizeF(+HelperKitWidth, 0),
                helperPoints[4] - new SizeF(0, +HelperKitWidth),
                helperPoints[4] - new SizeF(0, -HelperKitWidth)};

            // draw the image.
            g.Transform = new Matrix();
            g.DrawLines(mHelperPen, helperBound);
            g.DrawRectangles(mHelperPen, helperRects);
            g.DrawLines(mHelperPen, helperCenter);
        }

        public void EditAnimation(Animation anim)
        {
            mAnimation = anim;
            mAnimTrack = null;
            SetTimePosition(0);
        }

        public void SelectAnimationTrack(Animation anim, AnimationTrack track)
        {
            mAnimation = anim;
            mAnimTrack = track;
            SetTimePosition(0);
        }

        const float CursorDetectDistance = 30;
        Cursor SetupTransformMode(PointF p)
        {
            // check the mouse position.
            PointF[] helperPoints = GetHelperPoints(mAnimTrack);
            PointF[] helperBound = new PointF[] {
                helperPoints[0],
                helperPoints[1],
                helperPoints[2],
                helperPoints[3],
                helperPoints[0]};

            // check the center.
            mTransformMode = TransformMode.Center;
            if (MathHelper.DistanceSqr(helperPoints[4], p) < CursorDetectDistance)
                return Cursors.Hand;

            // inside: move mode.
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddLines(helperBound);
            path.CloseFigure();

            mTransformMode = TransformMode.Move;
            if (path.IsVisible(p))
                return Cursors.SizeAll;

            // outside: 
            mTransformMode = TransformMode.Edge0;
            for (int i = 0; i < 4; i++)
            {
                PointF p0 = helperBound[i];
                PointF p1 = helperBound[i + 1];

                double distanceSqr = MathHelper.LinePointDistSqr(p0, p1, p, true);
                if (distanceSqr < CursorDetectDistance)
                {
                    double angle = Math.Atan2(p1.Y - p0.Y, p1.X - p0.X);
                    if (angle < 0) angle += Math.PI;

                    double range = angle * 8 / Math.PI;
                    if (range < 1)
                        return Cursors.SizeNS;

                    if (range < 3)
                        return Cursors.SizeNESW;

                    if (range < 5)
                        return Cursors.SizeWE;

                    if (range < 7)
                        return Cursors.SizeNWSE;

                    return Cursors.SizeNS;
                }
                mTransformMode++;
            }

            mTransformMode = TransformMode.Rotate;
            return mRotateCursor;
        }

        Point mLastMousePos = Point.Empty;
        Point mLastMouseDownPos = Point.Empty;
        private void AnimationPreviewPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Capture = true;

                mLastMousePos = e.Location;
            }
            mLastMouseDownPos = e.Location;
        }

        PointF Transform(Matrix m, PointF p)
        {
            PointF[] ps = new PointF[] { p };
            m.TransformPoints(ps);
            return ps[0];
        }

        SizeF Transform(Matrix m, SizeF s)
        {
            PointF[] ps = new PointF[] { new PointF(s.Width, s.Height) };
            m.TransformVectors(ps);
            return new SizeF(ps[0].X, ps[0].Y);
        }

        void ProcessMouseMove(AnimationTrack track, PointF pre, PointF cur)
        {
            Matrix parent = track.ParentMatrix.Clone();
            parent.Invert();

            SizeF delta = new SizeF(cur.X - pre.X, cur.Y - pre.Y);
            AnimationKey defaultKey = mAnimTrack.CachedKey;

            delta = Transform(parent, delta);
            defaultKey.Location += new SizeF(delta.Width, delta.Height);
        }

        void ProcessMouseCenter(AnimationTrack track, PointF pre, PointF cur)
        {
            PointF delta = cur - new SizeF(pre.X, pre.Y);
            AnimationKey defaultKey = mAnimTrack.CachedKey;
            defaultKey.Center += new SizeF(delta.X, delta.Y);
        }

        void ProcessMouseRotate(AnimationTrack track, PointF pre, PointF cur)
        {
            // fetch the center position.
            Matrix worldMatrix = track.CachedMatrix;
            Matrix finalTransform = mViewMatrix.Clone();
            finalTransform.Multiply(worldMatrix);

            AnimationKey defaultKey = track.CachedKey;
            PointF center = Transform(finalTransform, new PointF(defaultKey.Center.X / defaultKey.Scale.X, defaultKey.Center.Y / defaultKey.Scale.Y));

            double angle = MathHelper.AngleBetween(cur, center, pre);
            defaultKey.Rotate += (float)(angle * 180.0f / Math.PI);
        }

        void ProcessMouseScale(AnimationTrack track, PointF pre, PointF cur, bool scaleX)
        {
            PointF[] baseLine = new PointF[] { PointF.Empty, PointF.Empty };
            if (scaleX)
                baseLine[1].Y = 100;
            else
                baseLine[1].X = 100;

            Matrix worldMatrix = track.CachedMatrix;
            Matrix finalTransform = mViewMatrix.Clone();
            finalTransform.Multiply(worldMatrix);
            finalTransform.TransformPoints(baseLine);

            double preDis = MathHelper.LinePointDist(baseLine[0], baseLine[1], pre, false);
            double curDis = MathHelper.LinePointDist(baseLine[0], baseLine[1], cur, false);
            float deltaMove = (float)(curDis - preDis);

            AnimationKey defaultKey = track.CachedKey;
            System.Drawing.Image texture = GetTexture(mAnimTrack);
            if (scaleX)
            {
                float preWidth = texture.Width * defaultKey.Scale.X;
                float curWidth = preWidth + deltaMove;
                float newScale = curWidth / texture.Width;
                defaultKey.Scale = new PointF(newScale, defaultKey.Scale.Y);
            }
            else
            {
                float preHeight = texture.Height * defaultKey.Scale.Y;
                float curHeight = preHeight + deltaMove;
                float newScale = curHeight / texture.Height;
                defaultKey.Scale = new PointF(defaultKey.Scale.X, newScale);
            }
        }

        void ProcessMouseDelta(PointF pre, PointF cur)
        {
            if (mAnimTrack == null)
                return;

            switch (mTransformMode)
            {
                case TransformMode.Center:
                    ProcessMouseCenter(mAnimTrack, pre, cur);
                    break;
                case TransformMode.Move:
                    ProcessMouseMove(mAnimTrack, pre, cur);
                    break;
                case TransformMode.Rotate:
                    ProcessMouseRotate(mAnimTrack, pre, cur);
                    break;
                case TransformMode.Edge0:
                case TransformMode.Edge2:
                    ProcessMouseScale(mAnimTrack, pre, cur, false);
                    break;
                case TransformMode.Edge1:
                case TransformMode.Edge3:
                    ProcessMouseScale(mAnimTrack, pre, cur, true);
                    break;
            }

            mAnimTrack.FlushTransform();

            ForceUpdate();
        }
        
        private void AnimationPreviewPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (mAnimTrack == null)
                return;

            if (e.Button == MouseButtons.Left && Capture)
            {
                // move mode.
                Point delta = e.Location - (Size)mLastMousePos;
                ProcessMouseDelta(mLastMousePos, e.Location);

                mLastMousePos = e.Location;
            }
            else
            {
                Capture = false;

                // detect mode.
                Cursor = SetupTransformMode(new PointF(e.Location.X, e.Location.Y));
            }
        }

        bool ProcessMouseClick(List<AnimationTrack> tracks, PointF p)
        {
            foreach (AnimationTrack track in tracks)
            {
                if (track.AnimTracks.Count > 0 && ProcessMouseClick(track.AnimTracks, p))
                    return true;

                // check the mouse position.
                PointF[] helperPoints = GetHelperPoints(track);
                PointF[] helperBound = new PointF[] {
                    helperPoints[0],
                    helperPoints[1],
                    helperPoints[2],
                    helperPoints[3]};

                // inside: move mode.
                GraphicsPath path = new GraphicsPath();
                path.StartFigure();
                path.AddLines(helperBound);
                path.CloseFigure();

                mTransformMode = TransformMode.Move;
                if (path.IsVisible(p))
                {
                    mAnimationControl.SelectAnimationTrack(track);
                    return true;
                }
            }
            return false;
        }

        private void AnimationPreviewPanel_MouseUp(object sender, MouseEventArgs e)
        {
            Capture = false;

            if (e.Location == mLastMouseDownPos)
            {
                if (mAnimation != null)
                    ProcessMouseClick(mAnimation.AnimTracks, e.Location);
            }
        }
    }
}
