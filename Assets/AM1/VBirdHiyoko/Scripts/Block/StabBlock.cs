using AM1.VBirdHiyoko;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    public class StabBlock : IMovableBlock
    {
        public bool StartPush(Vector3 direction)
        {
            Debug.Log($"Stab StartPush({direction})");
            return true;
        }
        public void Push(Vector3 move)
        {
            Debug.Log($"Push({move.x}, {move.z})");
        }

        public void PushDone()
        {
            Debug.Log($"PushDone");
        }

        public void PlayPushSE()
        {
            Debug.Log($"PlayPushSE");
        }

        public void PlayCollisionSE()
        {
            Debug.Log($"PlayCollisionSE");
        }
    }
}