using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    public class RigidbodyGrass : MonoBehaviour, IMovableBlock, IBlockInfo
    {
        public void PlayCollisionSE()
        {
        }

        public void PlayPushSE()
        {
        }

        public void Push(Vector3 move)
        {
        }

        public void PushDone()
        {
        }

        public bool StartPush(Vector3 direction)
        {
            return true;
        }
    }
}
