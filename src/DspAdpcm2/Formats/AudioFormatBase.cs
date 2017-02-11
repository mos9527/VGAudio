﻿using System;

namespace DspAdpcm.Formats
{
    public abstract class AudioFormatBase : IAudioFormat
    {
        public int SampleCount { get; }
        public int SampleRate { get; }
        public int ChannelCount { get; }
        public int LoopStart { get; protected set; }
        public int LoopEnd { get; protected set; }
        public bool Looping { get; protected set; }

        protected AudioFormatBase(int sampleCount, int sampleRate, int channelCount)
        {
            SampleCount = sampleCount;
            SampleRate = sampleRate;
            ChannelCount = channelCount;
        }

        public void SetLoop(int loopStart, int loopEnd)
        {
            if (loopStart < 0 || loopStart > SampleCount)
            {
                throw new ArgumentOutOfRangeException(nameof(loopStart), loopStart, "Loop points must be less than the number of samples and non-negative.");
            }

            if (loopEnd < 0 || loopEnd > SampleCount)
            {
                throw new ArgumentOutOfRangeException(nameof(loopEnd), loopEnd, "Loop points must be less than the number of samples and non-negative.");
            }

            if (loopEnd < loopStart)
            {
                throw new ArgumentOutOfRangeException(nameof(loopEnd), loopEnd, "The loop end must be greater than the loop start");
            }

            Looping = true;
            LoopStart = loopStart;
            LoopEnd = loopEnd;
        }

        public abstract Pcm16Format ToPcm16();
    }
}