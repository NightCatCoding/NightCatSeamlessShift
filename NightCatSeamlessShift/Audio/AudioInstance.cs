using System;
using System.IO;
using System.Timers;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NightCatSeamlessShift.Models;

namespace NightCatSeamlessShift.Audio;

public class AudioInstance : IDisposable
{
    private readonly object _lock = new();
    private AudioFileReader? _reader;
    private VolumeSampleProvider? _volumeProvider;
    private PlaybackStateSampleProvider? _playbackStateProvider;
    private readonly AudioState _state;
    private Timer? _fadeTimer;
    private Timer? _loopCheckTimer;
    private double _fadeStartVolume;
    private double _fadeTargetVolume;
    private double _fadeDuration;
    private double _fadeElapsed;
    private bool _disposed;

    public TrackData Data { get; }
    public AudioState State => _state;
    public ISampleProvider? SampleProvider => _playbackStateProvider;

    public AudioInstance(TrackData data)
    {
        Data = data;
        _state = new AudioState();

        if (!string.IsNullOrEmpty(data.Url) && File.Exists(data.Url))
        {
            try
            {
                _reader = new AudioFileReader(data.Url);
                _volumeProvider = new VolumeSampleProvider(_reader) { Volume = 0f };
                _playbackStateProvider = new PlaybackStateSampleProvider(_volumeProvider) { IsPlaying = false };
                _state.Duration = _reader.TotalTime.TotalSeconds;
                _state.Loop.End = _state.Duration;
            }
            catch
            {
                _reader?.Dispose();
                _reader = null;
                _volumeProvider = null;
                _playbackStateProvider = null;
            }
        }
    }

    public void Play()
    {
        lock (_lock)
        {
            if (_disposed || _reader == null || _playbackStateProvider == null) return;

            if (_state.CurrentTime >= _state.Duration - 0.1)
            {
                _reader.CurrentTime = TimeSpan.Zero;
                _state.CurrentTime = 0;
            }

            if (!_state.IsPlaying)
            {
                _state.IsPlaying = true;
                _playbackStateProvider.IsPlaying = true;
                EnsureLoopTimer();
            }
        }
    }

    public void Pause()
    {
        lock (_lock)
        {
            if (_disposed || _playbackStateProvider == null) return;
            if (_state.IsPlaying)
            {
                _state.IsPlaying = false;
                _playbackStateProvider.IsPlaying = false;
                StopLoopTimer();
            }
        }
    }

    public void Stop()
    {
        lock (_lock)
        {
            if (_disposed || _reader == null || _playbackStateProvider == null) return;
            _state.IsPlaying = false;
            _playbackStateProvider.IsPlaying = false;
            StopLoopTimer();
            _reader.CurrentTime = TimeSpan.Zero;
            _state.CurrentTime = 0;
        }
    }

    public void SetVolume(double volume)
    {
        lock (_lock)
        {
            if (_disposed) return;
            _state.Volume = Math.Clamp(volume, 0, 1);
            if (_volumeProvider != null)
                _volumeProvider.Volume = (float)_state.Volume;
        }
    }

    public void FadeTo(double targetVolume, double duration)
    {
        lock (_lock)
        {
            if (_disposed) return;

            _fadeTimer?.Stop();
            _fadeTimer?.Dispose();

            _state.IsFading = true;
            _fadeStartVolume = _state.Volume;
            _fadeTargetVolume = Math.Clamp(targetVolume, 0, 1);
            _fadeDuration = Math.Max(duration, 0.01);
            _fadeElapsed = 0;

            _fadeTimer = new Timer(16);
            _fadeTimer.Elapsed += OnFadeTimerElapsed;
            _fadeTimer.Start();
        }
    }

    private void OnFadeTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        lock (_lock)
        {
            if (_disposed) return;

            _fadeElapsed += 0.016;
            var progress = Math.Min(_fadeElapsed / _fadeDuration, 1.0);
            var newVolume = _fadeStartVolume + (_fadeTargetVolume - _fadeStartVolume) * progress;

            _state.Volume = newVolume;
            if (_volumeProvider != null)
                _volumeProvider.Volume = (float)newVolume;

            if (progress >= 1.0)
            {
                _fadeTimer?.Stop();
                _state.IsFading = false;
            }
        }
    }

    public void Seek(double seconds)
    {
        lock (_lock)
        {
            if (_disposed || _reader == null) return;
            var time = TimeSpan.FromSeconds(Math.Clamp(seconds, 0, _state.Duration));
            _reader.CurrentTime = time;
            _state.CurrentTime = seconds;
        }
    }

    public void UpdateCurrentTime()
    {
        lock (_lock)
        {
            if (_disposed || _reader == null) return;

            if (_state.IsPlaying)
            {
                _state.CurrentTime = _reader.CurrentTime.TotalSeconds;

                if (_state.CurrentTime >= _state.Duration - 0.1 && !_state.Loop.Active)
                {
                    _state.IsPlaying = false;
                    if (_playbackStateProvider != null)
                        _playbackStateProvider.IsPlaying = false;
                    StopLoopTimer();
                }
            }
        }
    }

    private void CheckLoop(object? sender, ElapsedEventArgs e)
    {
        lock (_lock)
        {
            if (_disposed || _reader == null || !_state.IsPlaying) return;

            _state.CurrentTime = _reader.CurrentTime.TotalSeconds;

            if (_state.Loop.Active)
            {
                if (_state.Loop.End > _state.Loop.Start && _state.Loop.Start >= 0)
                {
                    if (_state.CurrentTime >= _state.Loop.End)
                    {
                        _reader.CurrentTime = TimeSpan.FromSeconds(_state.Loop.Start);
                        _state.CurrentTime = _state.Loop.Start;
                    }
                }
                else
                {
                    if (_state.CurrentTime >= _state.Duration - 0.1)
                    {
                        _reader.CurrentTime = TimeSpan.Zero;
                        _state.CurrentTime = 0;
                    }
                }
            }
            else if (_state.CurrentTime >= _state.Duration - 0.1)
            {
                _state.IsPlaying = false;
                if (_playbackStateProvider != null)
                    _playbackStateProvider.IsPlaying = false;
                StopLoopTimer();
            }
        }
    }

    private void EnsureLoopTimer()
    {
        if (_loopCheckTimer == null || !_loopCheckTimer.Enabled)
        {
            if (_loopCheckTimer != null)
            {
                _loopCheckTimer.Stop();
                _loopCheckTimer.Dispose();
            }

            _loopCheckTimer = new Timer(16);
            _loopCheckTimer.Elapsed += CheckLoop;
            _loopCheckTimer.Start();
        }
    }

    private void StopLoopTimer()
    {
        if (_loopCheckTimer != null && _loopCheckTimer.Enabled)
        {
            _loopCheckTimer.Stop();
        }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            if (_disposed) return;
            _disposed = true;

            _fadeTimer?.Stop();
            _fadeTimer?.Dispose();
            _loopCheckTimer?.Stop();
            _loopCheckTimer?.Dispose();
            _reader?.Dispose();
        }
    }
}