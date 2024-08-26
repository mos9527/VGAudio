using System;
using System.IO;
using VGAudio.Codecs.CriHca;
using VGAudio.Containers.Hca;
using VGAudio.Containers.Wave;
using VGAudio.Formats.Pcm16;

namespace VGAudio
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            const int ChannelCount = 1;
            const int SampleRate = 44100;
            const int SampleBitRate = 128000;
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: VGAudio <sekai_XX_XXXXXX.hca> <output.wav>"); 
                return 1;
            }
            var path = args[0]; var opath = args[1];
            var stream = File.OpenRead(path);
            HcaStructure data = new HcaStructure();
            data.Hca.ChannelCount = ChannelCount;
            data.Hca.SampleRate = SampleRate;
            data.Hca.SampleCount = data.Hca.SampleRate;
            data.Hca.TrackCount = 1;
            CriHcaEncoder.CalculateBandCounts(data.Hca, SampleBitRate, data.Hca.SampleRate / 2);
            data.Hca.FrameCount = (int)(stream.Length / data.Hca.FrameSize);
            Console.WriteLine("Frame Size: " + data.Hca.FrameSize);
            Console.WriteLine("Frame Count: " + data.Hca.FrameCount);
            HcaReader.ReadHcaData(new BinaryReader(stream), data);
            {
                var pcm = CriHcaDecoder.Decode(data.Hca, data.AudioData);
                Pcm16FormatBuilder builder = new Pcm16FormatBuilder(pcm, data.Hca.SampleRate);
                var fmt = builder.Build();
                WaveWriter writer = new WaveWriter();
                var ostream = File.OpenWrite(opath);
                writer.WriteToStream(fmt, ostream);
            }
            return 0;           
        }

    }
}