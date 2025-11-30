namespace NightCatSeamlessShift.Models;

public class AudioState
{
    public double Volume { get; set; } = 1.0;
    public bool IsPlaying { get; set; }
    public bool IsFading { get; set; }
    public double Duration { get; set; }
    public double CurrentTime { get; set; }
    public LoopConfig Loop { get; set; } = new();
}