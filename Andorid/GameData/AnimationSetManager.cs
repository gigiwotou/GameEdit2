using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace GameData
{
    public class AnimationSetManager : Singleton<AnimationSetManager>
    {
        Dictionary<string, AnimationSet> mAnimationSets = new Dictionary<string, AnimationSet>();
        public Dictionary<string, AnimationSet> AnimationSets { get { return mAnimationSets; } }

        public bool Contains(string name)
        {
            return mAnimationSets.ContainsKey(name);
        }

        public bool Delete(string name)
        {
            return mAnimationSets.Remove(name);
        }

        public AnimationSet Get(string name)
        {
            AnimationSet animationSet;
            mAnimationSets.TryGetValue(name, out animationSet);
            return animationSet;
        }

        public void Add(AnimationSet animationSet)
        {
            mAnimationSets[animationSet.Name] = animationSet;
        }

        public class Content
        {
            List<AnimationSet> mAnimationSets = new List<AnimationSet>();
            public List<AnimationSet> AnimationSets { get { return mAnimationSets; } }

            public Content() { }
            public Content(ICollection<AnimationSet> list)
            {
                mAnimationSets.Clear();
                mAnimationSets.AddRange(list);
            }
        }

        public void Load()
        {
            string fileName = Root.Instance.BasePath + "\\AnimationSet.xml";

            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(Content));
                StreamReader sr = new StreamReader(fileName);
                Content data = (Content)ser.Deserialize(sr);
                sr.Close();

                mAnimationSets.Clear();
                foreach (AnimationSet animationSet in data.AnimationSets)
                    mAnimationSets[animationSet.Name] = animationSet;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Save()
        {
            string fileName = Root.Instance.BasePath + "\\AnimationSet.xml";

            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(Content));
                StreamWriter sw = new StreamWriter(fileName);
                Content data = new Content(mAnimationSets.Values);
                ser.Serialize(sw, data);
                sw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
