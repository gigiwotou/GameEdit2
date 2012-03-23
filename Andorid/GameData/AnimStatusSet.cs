using System;
using System.Collections.Generic;
using System.Text;

namespace GameData
{
    public class AnimStatusSet
    {
        List<AnimStatus> mAnimStatusList = new List<AnimStatus>();

        public void Update(float deltaTime)
        {
            foreach (AnimStatus animStatus in mAnimStatusList)
                animStatus.Update(deltaTime);
        }
    }
}
