/*
using HarmonyLib;

namespace AugaLite
{
    [HarmonyPatch]
    public static class TextInput_Setup
    {
        [HarmonyPatch(typeof(TextInput), nameof(TextInput.Awake))]
        public static class TextInput_Awake_Patch
        {
            public static bool Prefix(TextInput __instance)
            {
                return !SetupHelper.DirectObjectReplace(__instance.transform, AugaLite.Assets.TextInput, "TextInput");
            }
        }
    }
}
*/
