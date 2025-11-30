# NightCat Seamless Shift

[English](#english) | [中文](#中文)

---

## English

### What is this?

A simple multi-track audio player that solves a specific problem: **seamlessly switching between different versions of
the same music during live events or performances**.

### Why I made this

During events or performances, you often need to switch between energetic and mellow versions of music based on the
atmosphere. It works especially well when both tracks have the same melody and rhythm but different instruments.

Traditional players can only play one track at a time. To switch, you have to:

1. Stop the current track
2. Select another file
3. Manually seek to the exact same position
4. Or spend time editing audio files beforehand

This tool eliminates those problems by allowing you to:

- Load multiple tracks at once
- Play them simultaneously
- Seamlessly fade between tracks without losing position
- Loop sections perfectly without gaps

### Core Features

- **Multi-track simultaneous playback** - Load multiple versions of the same piece
- **Seamless switching** - Fade between tracks instantly while maintaining position
- **Perfect loops** - Set loop points for continuous playback
- **Individual controls** - Volume, play/pause for each track
- **Focus mode** - Quickly switch between tracks with auto-fade
- **Supported formats**: MP3, WAV, FLAC, OGG, M4A, AAC

### Use Cases

- Live performances with dynamic atmosphere changes
- DJ sets with multiple stems
- Practice sessions with different instrument tracks
- Audio production and comparison
- Game streaming with adaptive music

### Technical Details

- Built with Avalonia UI (cross-platform)
- Audio engine: NAudio
- Platform: .NET 9.0
- Clean MVVM architecture

### How to Use

1. **Add tracks** - Click "Add Track" to load audio files (works best with synchronized versions)
2. **Set volumes** - Adjust each track's volume independently
3. **Enable loops** - Set start/end points for seamless looping
4. **Switch smoothly** - Use "Focus" button or fade controls to transition between tracks
5. **Control playback** - Use global or individual play/pause/stop controls

### Building

```bash
git clone https://github.com/NightCatCoding/NightCatSeamlessShift.git
cd NightCatSeamlessShift
dotnet restore
dotnet build
dotnet run --project NightCatSeamlessShift
```

Requires .NET 9.0 SDK

## My ko-fi donation page: [https://ko-fi.com/nightcatcoding](https://ko-fi.com/nightcatcoding)

---

## 中文

### 這是什麼？

一個簡單的多軌音頻播放器，解決一個具體問題：**在現場活動或表演時無縫切換同一音樂的不同版本**。

### 為什麼做這個

在活動或表演時，常常需要根據當下氣氛在熱烈版本和抒情版本之間切換。當兩個曲目的旋律和節奏相同但樂器不同時，效果特別好。

傳統播放器一次只能播一首。要切換時必須：

1. 停止當前曲目
2. 選擇另一個文件
3. 手動定位到相同的秒數
4. 或者事先花時間剪輯音頻

這個工具解決了這些問題，讓你可以：

- 一次載入多個軌道
- 同時播放
- 無縫淡入淡出切換，不會失去位置
- 完美循環播放，沒有間隙

### 核心功能

- **多軌同步播放** - 載入同一曲目的多個版本
- **無縫切換** - 即時在軌道間淡入淡出，保持播放位置
- **完美循環** - 設定循環點持續播放
- **獨立控制** - 每個軌道的音量、播放/暫停
- **焦點模式** - 快速切換軌道並自動淡入淡出
- **支援格式**：MP3、WAV、FLAC、OGG、M4A、AAC

### 使用場景

- 需要動態調整氣氛的現場表演
- DJ 混音與多音軌控制
- 使用不同樂器軌道的練習
- 音頻製作與比較
- 遊戲直播的動態音樂

### 技術細節

- 使用 Avalonia UI 構建（跨平台）
- 音頻引擎：NAudio
- 平台：.NET 9.0
- 簡潔的 MVVM 架構

### 使用方法

1. **添加軌道** - 點擊「Add Track」載入音頻文件（同步版本效果最佳）
2. **設定音量** - 獨立調整每個軌道的音量
3. **啟用循環** - 設定起止點實現無縫循環
4. **流暢切換** - 使用「Focus」按鈕或淡入淡出控制在軌道間轉換
5. **控制播放** - 使用全局或單獨的播放/暫停/停止控制

### 構建

```bash
git clone https://github.com/NightCatCoding/NightCatSeamlessShift.git
cd NightCatSeamlessShift
dotnet restore
dotnet build
dotnet run --project NightCatSeamlessShift
```

需要 .NET 9.0 SDK

## 我的 ko-fi 捐助頁面: [https://ko-fi.com/nightcatcoding](https://ko-fi.com/nightcatcoding)