namespace NightmareMode.Items.Interfaces;

public interface ITimeEvent
{
    int Hours { get; }
    void OnHour(int hour);
    void OnHalfHour(int hour);
    void OnWin();
}
