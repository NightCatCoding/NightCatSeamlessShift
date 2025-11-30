using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using NightCatSeamlessShift.Audio;
using NightCatSeamlessShift.Models;

namespace NightCatSeamlessShift.ViewModels;

public class MainViewModel : ViewModelBase, IDisposable
{
    private readonly AudioManager _audioManager;
    private double _masterVolume = 1.0;
    private double _fadeDuration = 2.0;

    public ObservableCollection<TrackViewModel> Tracks { get; } = new();

    public double MasterVolume
    {
        get => _masterVolume;
        set
        {
            if (SetProperty(ref _masterVolume, value))
            {
                _audioManager.SetMasterVolume(value);
            }
        }
    }

    public double FadeDuration
    {
        get => _fadeDuration;
        set => SetProperty(ref _fadeDuration, value);
    }

    public ICommand AddTrackCommand { get; }
    public ICommand PlayAllCommand { get; }
    public ICommand PauseAllCommand { get; }
    public ICommand StopAllCommand { get; }
    public ICommand FadeInAllCommand { get; }
    public ICommand FadeOutAllCommand { get; }
    public ICommand ClearAllCommand { get; }

    public MainViewModel()
    {
        _audioManager = new AudioManager();
        _audioManager.StateChanged += OnAudioStateChanged;

        AddTrackCommand = new RelayCommand(async () => await AddTrackAsync());
        PlayAllCommand = new RelayCommand(() => _audioManager.PlayAll());
        PauseAllCommand = new RelayCommand(() => _audioManager.PauseAll());
        StopAllCommand = new RelayCommand(() => _audioManager.StopAll());
        FadeInAllCommand = new RelayCommand(() => _audioManager.FadeAllTo(1.0, FadeDuration));
        FadeOutAllCommand = new RelayCommand(() => _audioManager.FadeAllTo(0.0, FadeDuration));
        ClearAllCommand = new RelayCommand(ClearAll, () => Tracks.Count > 0);
    }

    private async Task AddTrackAsync()
    {
        var storageProvider = App.StorageProvider;
        if (storageProvider == null) return;

        var options = new FilePickerOpenOptions
        {
            Title = "Select Audio File",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Audio Files")
                {
                    Patterns = new[] { "*.mp3", "*.wav", "*.flac", "*.ogg", "*.m4a", "*.aac" }
                }
            }
        };

        var files = await storageProvider.OpenFilePickerAsync(options);
        if (files.Count > 0)
        {
            var file = files[0];
            var path = file.Path.LocalPath;
            var name = file.Name;

            var data = new TrackData
            {
                Name = name,
                Url = path
            };

            var instance = _audioManager.AddTrack(data);
            var vm = new TrackViewModel(instance, RemoveTrack, FocusTrack);
            Tracks.Add(vm);
        }
    }

    private void RemoveTrack(string id)
    {
        _audioManager.RemoveTrack(id);
        var vm = Tracks.FirstOrDefault(t => t.Id == id);
        if (vm != null)
        {
            Tracks.Remove(vm);
        }
    }

    private void FocusTrack(string id)
    {
        _audioManager.FocusTrack(id, FadeDuration);
    }

    private void ClearAll()
    {
        var trackIds = Tracks.Select(t => t.Id).ToList();
        foreach (var id in trackIds)
        {
            _audioManager.RemoveTrack(id);
        }

        Tracks.Clear();
    }

    private void OnAudioStateChanged()
    {
        Dispatcher.UIThread.Post(() =>
        {
            foreach (var track in Tracks)
            {
                track.UpdateState();
            }
        });
    }

    public void Dispose()
    {
        _audioManager?.Dispose();
    }
}