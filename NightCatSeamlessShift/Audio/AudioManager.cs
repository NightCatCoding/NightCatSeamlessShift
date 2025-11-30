using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NightCatSeamlessShift.Models;

namespace NightCatSeamlessShift.Audio;

public class AudioManager : IDisposable
{
    private readonly object _lock = new();
    private readonly Dictionary<string, AudioInstance> _instances = new();
    private readonly MixingSampleProvider _mixer;
    private readonly WaveOutEvent _output;
    private Timer? _updateTimer;
    private double _masterVolume = 1.0;
    private bool _disposed;

    public event Action? StateChanged;
    public double MasterVolume => _masterVolume;

    public AudioManager()
    {
        _mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2))
        {
            ReadFully = true
        };

        _output = new WaveOutEvent { Volume = 1.0f };
        _output.Init(_mixer);
        _output.Play();

        _updateTimer = new Timer(50);
        _updateTimer.Elapsed += (s, e) =>
        {
            lock (_lock)
            {
                if (_disposed) return;
                foreach (var instance in _instances.Values)
                {
                    instance.UpdateCurrentTime();
                }
            }

            StateChanged?.Invoke();
        };
        _updateTimer.Start();
    }

    public AudioInstance AddTrack(TrackData data)
    {
        lock (_lock)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(AudioManager));

            var instance = new AudioInstance(data);
            _instances[data.Id] = instance;

            if (instance.SampleProvider != null)
                _mixer.AddMixerInput(instance.SampleProvider);

            StateChanged?.Invoke();
            return instance;
        }
    }

    public void RemoveTrack(string id)
    {
        lock (_lock)
        {
            if (_disposed || !_instances.TryGetValue(id, out var instance)) return;

            if (instance.SampleProvider != null)
                _mixer.RemoveMixerInput(instance.SampleProvider);

            instance.Dispose();
            _instances.Remove(id);
            StateChanged?.Invoke();
        }
    }

    public AudioInstance? GetTrack(string id)
    {
        lock (_lock)
        {
            return _instances.TryGetValue(id, out var instance) ? instance : null;
        }
    }

    public IEnumerable<AudioInstance> GetAllTracks()
    {
        lock (_lock)
        {
            return _instances.Values.ToList();
        }
    }

    public void PlayAll()
    {
        lock (_lock)
        {
            if (_disposed) return;
            foreach (var instance in _instances.Values)
            {
                instance.Play();
            }

            StateChanged?.Invoke();
        }
    }

    public void PauseAll()
    {
        lock (_lock)
        {
            if (_disposed) return;
            foreach (var instance in _instances.Values)
            {
                instance.Pause();
            }

            StateChanged?.Invoke();
        }
    }

    public void StopAll()
    {
        lock (_lock)
        {
            if (_disposed) return;
            foreach (var instance in _instances.Values)
            {
                instance.Stop();
            }

            StateChanged?.Invoke();
        }
    }

    public void SetMasterVolume(double volume)
    {
        lock (_lock)
        {
            if (_disposed) return;
            _masterVolume = Math.Clamp(volume, 0, 1);
            _output.Volume = (float)_masterVolume;
            StateChanged?.Invoke();
        }
    }

    public void FocusTrack(string id, double fadeDuration = 2.0)
    {
        lock (_lock)
        {
            if (_disposed) return;

            foreach (var instance in _instances.Values)
            {
                if (instance.Data.Id == id)
                {
                    instance.FadeTo(1.0, fadeDuration);
                    if (!instance.State.IsPlaying)
                        instance.Play();
                }
                else
                {
                    instance.FadeTo(0.0, fadeDuration);
                }
            }

            StateChanged?.Invoke();
        }
    }

    public void FadeAllTo(double targetVolume, double duration)
    {
        lock (_lock)
        {
            if (_disposed) return;
            foreach (var instance in _instances.Values)
            {
                instance.FadeTo(targetVolume, duration);
            }

            StateChanged?.Invoke();
        }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            if (_disposed) return;
            _disposed = true;

            _updateTimer?.Stop();
            _updateTimer?.Dispose();

            foreach (var instance in _instances.Values)
            {
                instance.Dispose();
            }

            _instances.Clear();

            _output?.Stop();
            _output?.Dispose();
        }
    }
}