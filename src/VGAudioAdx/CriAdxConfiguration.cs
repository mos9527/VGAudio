﻿using VGAudio.Containers.Adx;

// ReSharper disable once CheckNamespace
namespace VGAudio.Codecs
{
    public class CriAdxConfiguration
    {
        public int SampleRate { get; set; } = 48000;
        public int HighpassFrequency { get; set; } = 500;
        public int FrameSize { get; set; } = 18;
        public int Version { get; set; } = 4;
        public short History { get; set; }
        public int Padding { get; set; }
        public AdxType Type { get; set; } = AdxType.Linear;
        public int Filter { get; set; }
    }
}