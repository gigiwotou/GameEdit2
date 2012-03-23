using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace GameData
{
    [Serializable]
    public class AnimationTrack
    {
        string mName = "";
        [XmlAttribute("Name"), DefaultValue("")]
        public string Name { get { return mName; } set { mName = value; } }

        bool mEnabled = true;
        [XmlAttribute("Enabled"), DefaultValue(true)]
        public bool Enabled { get { return mEnabled; } set { mEnabled = value; } }
        
        String mImageName = "";
        [XmlAttribute("ImageName"), DefaultValue(""), Category("Image")]
        public string ImageName { get { return mImageName; } set { mImageName = value; } }

        Image mCachedImage = null;
        [XmlIgnore, Browsable(false)]
        public Image CachedImage { get { return mCachedImage; } }

        int mImageColum = 1;
        [XmlAttribute("ImageColum"), DefaultValue(1), Category("Image")]
        public int ImageColum { get { return mImageColum; } set { mImageColum = value; } }

        int mImageRow = 1;
        [XmlAttribute("ImageRow"), DefaultValue(1), Category("Image")]
        public int ImageRow { get { return mImageRow; } set { mImageRow = value; } }

        int mImageStart = 0;
        [XmlAttribute("ImageStart"), DefaultValue(0), Category("Image")]
        public int ImageStart { get { return mImageStart; } set { mImageStart = value; } }

        Matrix mParentMatrix = new Matrix();
        [XmlIgnore, Browsable(false)]
        public Matrix ParentMatrix { get { return mParentMatrix; } }

        Matrix mCachedMatrix = new Matrix();
        [XmlIgnore, Browsable(false)]
        public Matrix CachedMatrix { get { return mCachedMatrix; } }

        List<AnimationTrack> mAnimTracks = new List<AnimationTrack>();
        public List<AnimationTrack> AnimTracks { get { return mAnimTracks; } }

        List<AnimationKey> mAnimKeys = new List<AnimationKey>();
        public List<AnimationKey> AnimKeys { get { return mAnimKeys; } }

        AnimationKey mCachedKey = new AnimationKey();
        public AnimationKey CachedKey { get { return mCachedKey; } set { mCachedKey = value; } }

        public void FlushImage()
        {
            mCachedImage = ImagesetManager.Instance.GetImage(mImageName);
        }

        AnimationKey CacheAnimationKey(int time)
        {
            if (mAnimKeys.Count == 0)
                return mCachedKey;
            
            if (mAnimKeys.Count == 1 || time <= mAnimKeys[0].Time)
                return mAnimKeys[0].Clone();
            
            if (time >= mAnimKeys[mAnimKeys.Count - 1].Time)
                return mAnimKeys[mAnimKeys.Count - 1].Clone();

            int left = 0, right = mAnimKeys.Count - 1;
            while (left < right - 1)
            {
                int middle = (left + right) / 2;
                AnimationKey key = mAnimKeys[middle];
                if (key.Time == time)
                    return key.Clone();

                if (key.Time < time)
                    left = middle;
                else
                    right = middle;
            }

            AnimationKey k0 = mAnimKeys[left];
            AnimationKey k1 = mAnimKeys[right];
            if (k1.Time == k0.Time)
                return k0.Clone();

            float alpha = (float)(time - k0.Time) / (k1.Time - k0.Time);
            AnimationKey result = k0 * (1.0f - alpha) + k1 * alpha;

            return result;
        }
        
        public void SetTimePosition(int time)
        {
            mCachedKey = CacheAnimationKey(time);

            // set the sub tracks.
            foreach (AnimationTrack track in mAnimTracks)
                track.SetTimePosition(time);
        }


        public void FlushTransform()
        {
            FlushTransform(mParentMatrix);
        }

        public void FlushTransform(Matrix parentTransform)
        {
            mParentMatrix = parentTransform;

            // setup cached matrix.
            mCachedMatrix = mParentMatrix.Clone();
            mCachedMatrix.Multiply(mCachedKey.GetTransform());

            // set the sub tracks.
            foreach (AnimationTrack track in mAnimTracks)
                track.FlushTransform(mCachedMatrix);
        }
    }
}
