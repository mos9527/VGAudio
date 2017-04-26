﻿using System;
using VGAudio.Codecs;

namespace VGAudio.Formats.GcAdpcm
{
    public class GcAdpcmChannel
    {
        private readonly int _sampleCount;

        public byte[] Adpcm { get; }
        private short[] Pcm { get; }
        public int SampleCount => AlignmentNeeded ? Alignment.SampleCountAligned : _sampleCount;

        public short Gain { get; }
        public short[] Coefs { get; }
        public short PredScale => Adpcm[0];
        public short Hist1 { get; }
        public short Hist2 { get; }

        public short LoopPredScale => LoopContext?.PredScale ?? 0;
        public short LoopHist1 => LoopContext?.Hist1 ?? 0;
        public short LoopHist2 => LoopContext?.Hist2 ?? 0;

        private GcAdpcmSeekTable SeekTable { get; }
        private GcAdpcmLoopContext LoopContext { get; }
        private GcAdpcmAlignment Alignment { get; }
        private int AlignmentMultiple => Alignment?.AlignmentMultiple ?? 0;
        private bool AlignmentNeeded => Alignment?.AlignmentNeeded ?? false;

        public GcAdpcmChannel(byte[] adpcm, short[] coefs, int sampleCount)
        {
            Adpcm = adpcm;
            Coefs = coefs;
            _sampleCount = sampleCount;
        }

        internal GcAdpcmChannel(GcAdpcmChannelBuilder b)
        {
            if (b.Adpcm.Length < GcAdpcmHelpers.SampleCountToByteCount(b.SampleCount))
            {
                throw new ArgumentException("Audio array length is too short for the specified number of samples.");
            }

            _sampleCount = b.SampleCount;
            Adpcm = b.Adpcm;

            Coefs = b.Coefs;
            Gain = b.Gain;
            Hist1 = b.Hist1;
            Hist2 = b.Hist2;

            Alignment = b.GetAlignment();
            LoopContext = b.GetLoopContext();
            SeekTable = b.GetSeekTable();

            Pcm = b.AlignedPcm;
        }

        public short[] GetPcmAudio() => Pcm ?? GcAdpcmDecoder.Decode(GetAudioData(), Coefs, SampleCount, Hist1, Hist2);
        public short[] GetSeekTable() => SeekTable?.SeekTable ?? new short[0];
        public byte[] GetAudioData() => AlignmentNeeded ? Alignment.AdpcmAligned : Adpcm;

        public GcAdpcmChannelBuilder GetCloneBuilder()
        {
            var builder = new GcAdpcmChannelBuilder(Adpcm, Coefs, SampleCount)
            {
                Pcm = Pcm,
                Gain = Gain,
                Hist1 = Hist1,
                Hist2 = Hist2,
                LoopAlignmentMultiple = AlignmentMultiple
            };
            builder.WithPrevious(SeekTable, LoopContext, Alignment);

            if (SeekTable != null)
            {
                builder.WithSeekTable(SeekTable.SeekTable, SeekTable.SamplesPerEntry, SeekTable.IsSelfCalculated);
            }

            if (LoopContext != null)
            {
                builder.WithLoopContext(LoopContext.LoopStart, PredScale, Hist1, Hist2);
            }

            if (Alignment != null)
            {
                builder.WithLoop(true, Alignment.LoopStart, Alignment.LoopEnd);
            }

            return builder;
        }
    }
}
