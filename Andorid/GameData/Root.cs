using System;
using System.Collections.Generic;
using System.Text;

namespace GameData
{
    public class Root : Singleton<Root>
    {
        string mBasePath = ".";

        public string BasePath { get { return mBasePath; } set { mBasePath = value; } }

        public void Init(string basePath)
        {
            mBasePath = basePath;

            ImagesetManager.Instance.Load();
            AnimationSetManager.Instance.Load();
        }

        public void Shutdown()
        {
        }
    }
}
