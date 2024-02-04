using HarmonyLib;
using UnityEngine;

namespace AugaLite;

[HarmonyPatch(typeof(FejdStartup))]
public static class FejdStartup_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(FejdStartup.SetupGui))]
    private static void SetupGui(FejdStartup __instance)
    {
        // disable the modded text on title screen. - JKA
        __instance.m_moddedText.SetActive(false);
    }
}