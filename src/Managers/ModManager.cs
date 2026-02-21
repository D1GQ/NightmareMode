using NightmareMode.Interfaces;
using NightmareMode.Monos;

namespace NightmareMode.Managers;

internal class ModManager : MonoSingleton<ModManager>
{
    internal bool IsModded => NightmarePlugin.ModEnabled;
    internal ITimeEvent? CurrentNight => NightManager.Current;
}
