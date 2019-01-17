using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Leap.Unity
{
    [Serializable]
    public class FrameContainer

    {

        public List<Frame> playbackFrames { get; set; }
        public List<long> playbackOffsets { get; set; }
        public List<long> playbackTimestamps { get; set; }
        public List<long> playbackInterpolationTimes { get; set; }

    }

}

