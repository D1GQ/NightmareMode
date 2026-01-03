using NightmareMode.Items.Interfaces;
using NightmareMode.Modules;

namespace NightmareMode.Managers;

internal class ModManager : MonoSingleton<ModManager>
{
    internal bool IsModded => NightmarePlugin.ModEnabled;
    internal ITimeEvent? CurrentNight => NightManager.Current;
}
