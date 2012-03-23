using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;
using GameData.Helper;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GameData
{
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AnimationKey
    {
        int mTime = 0;
        [XmlAttribute("Time"), DefaultValue(0), Category("Animation")]
        public int Time { get { return mTime; } set { mTime = value; } }

        Color mColor = Color.White;
        [Category("Display")]
        public Color Color { get { return mColor; } set { mColor = value; } }

        PointF mCenter = PointF.Empty;
        [TypeConverter(typeof(PointFConverter)), Category("Transform")]
        public PointF Center { get { return mCenter; } set { mCenter = value; } }

        PointF mLocation = PointF.Empty;
        [TypeConverter(typeof(PointFConverter)), Category("Transform")]
        public PointF Location { get { return mLocation; } set { mLocation = value; } }

        PointF mScale = new PointF(1, 1);
        [TypeConverter(typeof(PointFConverter)), Category("Transform")]
        public PointF Scale { get { return mScale; } set { mScale = value; } }

        float mAlpha = 1.0f;
        [XmlAttribute("Alpha"), DefaultValue(1.0f), Category("Display")]
        public float Alpha { get { return mAlpha; } set { mAlpha = value; } }

        float mRotate = 0.0f;
        [XmlAttribute("Rotate"), DefaultValue(0.0f), Category("Transform")]
        public float Rotate { get { return mRotate; } set { mRotate = value; } }

        float mImageIndexOffset = 0.0f;
        [XmlAttribute("ImageIndexOffset"), DefaultValue(0.0f), Category("Display")]
        public float ImageIndexOffset { get { return mImageIndexOffset; } set { mImageIndexOffset = value; } }

        // property helper.
        public bool ShouldSerializeColor() { return mColor != Color.White; }
        public bool ShouldSerializeCenter() { return mCenter.X != 0 || mCenter.Y != 0; }
        public bool ShouldSerializeLocation() { return mLocation.X != 0 || mLocation.Y != 0; }
        public bool ShouldSerializeScale() { return mScale.X != 1 || mScale.Y != 1; }

        public override string ToString()
        {
            return "AnimationKey: " + Time;
        }

        public Matrix GetTransform()
        {
            Matrix worldMatrix = new Matrix();
            worldMatrix.Translate(mLocation.X, mLocation.Y);
            worldMatrix.RotateAt(mRotate, new PointF(mCenter.X, mCenter.Y));
            worldMatrix.Scale(mScale.X, mScale.Y);
            return worldMatrix;
        }

        public void CopyFrom(AnimationKey src)
        {
            Time = src.Time;
            Color = src.Color;
            Center = src.Center;
            Location = src.Location;
            Scale = src.Scale;
            Alpha = src.Alpha;
            Rotate = src.Rotate;
            ImageIndexOffset = src.ImageIndexOffset;
        }

        public AnimationKey Clone()
        {
            AnimationKey copy = new AnimationKey();
            copy.CopyFrom(this);
            return copy;
        }

        public class AnimationKeyCompare : Comparer<AnimationKey>
        {
            public override int Compare(AnimationKey x, AnimationKey y)
            {
                return x.Time.CompareTo(y.Time);
            }
        }

        static int Round(int c)
        {
            return Math.Min(Math.Max(c, 0), 255);
        }

        static int Round(float c)
        {
            return Math.Min(Math.Max((int)(c + 0.5f), 0), 255);
        }

        public static AnimationKey operator +(AnimationKey k1, AnimationKey k2)
        {
            AnimationKey ret = new AnimationKey();
            ret.Color = Color.FromArgb(
                Round(k1.Color.A + k2.Color.A),
                Round(k1.Color.R + k2.Color.R),
                Round(k1.Color.G + k2.Color.G),
                Round(k1.Color.B + k2.Color.B));
            ret.Center = k1.Center + new SizeF(k2.Center);
            ret.Location = k1.Location + new SizeF(k2.Location);
            ret.Scale = k1.Scale + new SizeF(k2.Scale);
            ret.Alpha = k1.Alpha + k2.Alpha;
            ret.Rotate = k1.Rotate + k2.Rotate;
            ret.ImageIndexOffset = k1.ImageIndexOffset + k2.ImageIndexOffset;
            return ret;
        }

        public static AnimationKey operator *(AnimationKey k1, float f)
        {
            AnimationKey ret = new AnimationKey();
            ret.Color = Color.FromArgb(
                Round(k1.Color.A * f),
                Round(k1.Color.R * f),
                Round(k1.Color.G * f),
                Round(k1.Color.B * f));
            ret.Center = new PointF(k1.Center.X * f, k1.Center.Y * f);
            ret.Location = new PointF(k1.Location.X * f, k1.Location.Y * f);
            ret.Scale = new PointF(k1.Scale.X * f, k1.Scale.Y * f);
            ret.Alpha = k1.Alpha * f;
            ret.Rotate = k1.Rotate * f;
            ret.ImageIndexOffset = k1.ImageIndexOffset * f;
            return ret;
        }

        public static AnimationKey Lerp(AnimationKey k1, AnimationKey k2, float alpha)
        {
            return k1 * (1.0f - alpha) + k2;
        }
    }
}
