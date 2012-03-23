using System;
using System.Collections.Generic;
using System.Text;

namespace GameData
{
    public class Singleton<T>
    {
        private static readonly T instance = Activator.CreateInstance<T>();

        protected Singleton() { }

        public static T Instance { get { return instance; } }
    }
}
