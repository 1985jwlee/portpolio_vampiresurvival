using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public interface ISeekTargetComponent
    {
        
    }

    [Serializable]
    public struct SeekingTargetComponent : ISeekTargetComponent
    {
        public bool startSeek;
        public float seekingTime;
        public uint seekTargetEGID;
    }

    [Serializable]
    public struct SeekingPositionComponent : ISeekTargetComponent
    {
        public bool startSeek;
        public float seekingTime;
        public Vector3 seekTargetPosition;
    }
}
