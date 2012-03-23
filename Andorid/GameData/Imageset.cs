using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace GameData
{
    [Serializable]
    public class Imageset
    {
        List<Image> mImages = new List<Image>();
        public List<Image> Images { get { return mImages; } }

        public bool Contains(string name)
        {
            foreach (Image image in mImages)
            {
                if (image.Name == name)
                    return true;
            }
            return false;
        }

        public bool Delete(string name)
        {
            foreach (Image image in mImages)
            {
                if (image.Name == name)
                {
                    mImages.Remove(image);
                    return true;
                }
            }
            return false;
        }

        public Image Get(string name)
        {
            foreach (Image image in mImages)
            {
                if (image.Name == name)
                    return image;
            }
            return null;
        }

        public void Add(Image image)
        {
            mImages.Add(image);
        }

        string mName = "";
        [XmlAttribute("Name"), DefaultValue("")]
        public string Name { get { return mName; } set { mName = value; } }

        string mFileName = "";
        [XmlAttribute("FileName"), DefaultValue("")]
        public string FileName { get { return mFileName; } set { mFileName = value; } }

        float mWidth = 1.0f;
        [XmlAttribute("Width"), DefaultValue(1.0f)]
        public float Width { get { return mWidth; } set { mWidth = value; } }

        float mHeight = 1.0f;
        [XmlAttribute("Height"), DefaultValue(1.0f)]
        public float Height { get { return mHeight; } set { mHeight = value; } }

        Object mTag = null;
        [XmlIgnore, Browsable(false)]
        public Object Tag { get { return mTag; } set { mTag = value; } }
    }
}
