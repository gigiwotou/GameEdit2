using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace GameData
{
    public class ImagesetManager : Singleton<ImagesetManager>
    {
        Dictionary<string, Imageset> mImagesets = new Dictionary<string, Imageset>();
        public Dictionary<string, Imageset> Imagesets { get { return mImagesets; } }
        
        public bool Contains(string name)
        {
            return mImagesets.ContainsKey(name);
        }

        public bool Delete(string name)
        {
            return mImagesets.Remove(name);
        }

        public Imageset Get(string name)
        {
            Imageset imageset;
            mImagesets.TryGetValue(name, out imageset);
            return imageset;
        }

        public void Add(Imageset imageset)
        {
            mImagesets[imageset.Name] = imageset;
        }

        public Image GetImage(string path)
        {
            string[] args = path.Split(':');
            if (args.Length != 2)
                return null;

            Imageset imageSet = Get(args[0]);
            if (imageSet == null)
                return null;

            return imageSet.Get(args[1]);
        }
        
        public class Content
        {
            List<Imageset> mImagesets = new List<Imageset>();
            public List<Imageset> Imagesets { get { return mImagesets; } }

            public Content() { }
            public Content(ICollection<Imageset> list)
            {
                mImagesets.Clear();
                mImagesets.AddRange(list);
            }
        }

        public void Load()
        {
            string fileName = Root.Instance.BasePath + "\\Imageset.xml";

            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(Content));
                StreamReader sr = new StreamReader(fileName);
                Content data = (Content)ser.Deserialize(sr);
                sr.Close();

                mImagesets.Clear();
                foreach (Imageset imageset in data.Imagesets)
                    mImagesets[imageset.Name] = imageset;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Save()
        {
            string fileName = Root.Instance.BasePath + "\\Imageset.xml";

            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(Content));
                StreamWriter sw = new StreamWriter(fileName);
                Content data = new Content(mImagesets.Values);
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
