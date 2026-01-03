namespace NightmareMode.Items.Interfaces;

internal interface INight : ITimeEvent
{
    int Night { get; }
    void InitNight();
}
