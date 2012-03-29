using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace GameData
{
    class MapBlock
    {
        int mX = 0;
        [XmlAttribute("X"), DefaultValue(0)]
        public int X { get { return mX; } set { mX = value; } }

        int mY = 0;
        [XmlAttribute("Y"), DefaultValue(0)]
        public int Y { get { return mY; } set { mY = value; } }
    }
}
