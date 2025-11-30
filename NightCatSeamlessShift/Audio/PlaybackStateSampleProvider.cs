using NAudio.Wave;

namespace NightCatSeamlessShift.Audio;

public class PlaybackStateSampleProvider : ISampleProvider
{
    private readonly ISampleProvider _source;
    private bool _isPlaying;

    public WaveFormat WaveFormat => _source.WaveFormat;

    public bool IsPlaying
    {
        get => _isPlaying;
        set => _isPlaying = value;
    }

    public PlaybackStateSampleProvider(ISampleProvider source)
    {
        _source = source;
    }

    public int Read(float[] buffer, int offset, int count)
    {
        if (!_isPlaying)
        {
            for (int i = 0; i < count; i++)
            {
                buffer[offset + i] = 0;
            }

            return count;
        }

        return _source.Read(buffer, offset, count);
    }
}