using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace GameData
{
    [Serializable]
    public class Image
    {
        string mName = "";
        [XmlAttribute("Name"), DefaultValue("")]
        public string Name { get { return mName; } set { mName = value; } }

        int mX = 0;
        [XmlAttribute("X"), DefaultValue(0)]
        public int X { get { return mX; } set { mX = value; } }

        int mY = 0;
        [XmlAttribute("Y"), DefaultValue(0)]
        public int Y { get { return mY; } set { mY = value; } }

        int mHeight = 100;
        [XmlAttribute("Height"), DefaultValue(100)]
        public int Height { get { return mHeight; } set { mHeight = value; } }

        int mWidth = 100;
        [XmlAttribute("Width"), DefaultValue(100)]
        public int Width { get { return mWidth; } set { mWidth = value; } }
        
        Imageset mImageset = null;
        [XmlIgnore, Browsable(false)]
        public Imageset Imageset { get { return mImageset; } set { mImageset = value; } }
    }
}
