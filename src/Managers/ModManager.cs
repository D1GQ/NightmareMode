using NightmareMode.Helpers;
using NightmareMode.Interfaces;
using NightmareMode.Monos;
using TMPro;

namespace NightmareMode.Managers;

internal class ModManager : MonoSingleton<ModManager>
{
    internal bool IsModded => NightmarePlugin.ModEnabled;
    internal ITimeEvent? CurrentNight => NightManager.Current;

    internal readonly TMP_FontAsset? NotoSans = Utils.LoadFontFromResources("NightmareMode.Resources.Fonts.NotoSans.ttf");
    internal readonly TMP_FontAsset? NotoSans_JP = Utils.LoadFontFromResources("NightmareMode.Resources.Fonts.NotoSansJP.ttf");
    internal readonly TMP_FontAsset? NotoSans_KR = Utils.LoadFontFromResources("NightmareMode.Resources.Fonts.NotoSansKR.ttf");
    internal readonly TMP_FontAsset? NotoSans_SC = Utils.LoadFontFromResources("NightmareMode.Resources.Fonts.NotoSansSC.ttf");
}
