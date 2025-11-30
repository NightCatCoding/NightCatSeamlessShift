using System;

namespace NightCatSeamlessShift.Models;

public class TrackData
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}