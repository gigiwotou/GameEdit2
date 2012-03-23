using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace GameEditor.Data
{
    public class ImagesetInfo
    {
        string mName = "";
        [XmlAttribute("Name"), DefaultValue("")]
        public string Name { get { return mName; } set { mName = value; } }

        string mFileName = "";
        [XmlAttribute("FileName"), DefaultValue("")]
        public string FileName { get { return mFileName; } set { mFileName = value; } }

        public ImagesetInfo(string name, string file)
        {
            mName = name;
            mFileName = file;
        }
    }
}
