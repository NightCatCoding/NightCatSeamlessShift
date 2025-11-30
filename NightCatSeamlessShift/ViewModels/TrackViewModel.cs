using System;
using System.Windows.Input;
using NightCatSeamlessShift.Audio;

namespace NightCatSeamlessShift.ViewModels;

public class TrackViewModel : ViewModelBase
{
    private readonly AudioInstance _instance;
    private readonly Action<string> _onRemove;
    private readonly Action<string> _onFocus;

    public string Id => _instance.Data.Id;
    public string Name => _instance.Data.Name;

    public double Volume
    {
        get => _instance.State.Volume;
        set
        {
            _instance.SetVolume(value);
            OnPropertyChanged();
        }
    }

    public bool IsPlaying => _instance.State.IsPlaying;
    public string PlayPauseText => IsPlaying ? "⏸" : "▶";
    public double CurrentTime => _instance.State.CurrentTime;
    public double Duration => _instance.State.Duration;

    public string TimeDisplay => $"{FormatTime(CurrentTime)} / {FormatTime(Duration)}";

    public double Progress
    {
        get => Duration > 0 ? CurrentTime / Duration * 100 : 0;
        set
        {
            if (Duration > 0)
            {
                var time = value / 100.0 * Duration;
                _instance.Seek(time);
            }
        }
    }

    public string Notes
    {
        get => _instance.Data.Notes;
        set
        {
            _instance.Data.Notes = value;
            OnPropertyChanged();
        }
    }

    public bool LoopActive
    {
        get => _instance.State.Loop.Active;
        set
        {
            _instance.State.Loop.Active = value;
            OnPropertyChanged();
        }
    }

    public double LoopStart
    {
        get => _instance.State.Loop.Start;
        set
        {
            _instance.State.Loop.Start = value;
            OnPropertyChanged();
        }
    }

    public double LoopEnd
    {
        get => _instance.State.Loop.End;
        set
        {
            _instance.State.Loop.End = value;
            OnPropertyChanged();
        }
    }

    public ICommand PlayPauseCommand { get; }
    public ICommand StopCommand { get; }
    public ICommand RemoveCommand { get; }
    public ICommand FocusCommand { get; }
    public ICommand SetLoopStartCommand { get; }
    public ICommand SetLoopEndCommand { get; }

    public TrackViewModel(AudioInstance instance, Action<string> onRemove, Action<string> onFocus)
    {
        _instance = instance;
        _onRemove = onRemove;
        _onFocus = onFocus;

        PlayPauseCommand = new RelayCommand(() =>
        {
            if (IsPlaying)
                _instance.Pause();
            else
                _instance.Play();
            OnPropertyChanged(nameof(IsPlaying));
            OnPropertyChanged(nameof(PlayPauseText));
        });

        StopCommand = new RelayCommand(() =>
        {
            _instance.Stop();
            OnPropertyChanged(nameof(IsPlaying));
            OnPropertyChanged(nameof(PlayPauseText));
            OnPropertyChanged(nameof(CurrentTime));
            OnPropertyChanged(nameof(Progress));
        });

        RemoveCommand = new RelayCommand(() => _onRemove(Id));
        FocusCommand = new RelayCommand(() => _onFocus(Id));

        SetLoopStartCommand = new RelayCommand(() => { LoopStart = CurrentTime; });

        SetLoopEndCommand = new RelayCommand(() => { LoopEnd = CurrentTime; });
    }

    public void UpdateState()
    {
        OnPropertyChanged(nameof(Volume));
        OnPropertyChanged(nameof(IsPlaying));
        OnPropertyChanged(nameof(PlayPauseText));
        OnPropertyChanged(nameof(CurrentTime));
        OnPropertyChanged(nameof(Duration));
        OnPropertyChanged(nameof(TimeDisplay));
        OnPropertyChanged(nameof(Progress));
    }

    private static string FormatTime(double seconds)
    {
        var ts = TimeSpan.FromSeconds(seconds);
        return ts.ToString(@"mm\:ss");
    }
}