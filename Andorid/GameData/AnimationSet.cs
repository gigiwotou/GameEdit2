using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace GameData
{
    [Serializable]
    public class AnimationSet
    {
        List<Animation> mAnimations = new List<Animation>();
        public List<Animation> Animations { get { return mAnimations; } }

        string mName = "";
        [XmlAttribute("Name"), DefaultValue("")]
        public string Name { get { return mName; } set { mName = value; } }
    }
}
