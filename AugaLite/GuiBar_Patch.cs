﻿using HarmonyLib;
using UnityEngine.UI;

namespace AugaLite
{
    [HarmonyPatch(typeof(GuiBar), nameof(GuiBar.SetBar))]
    public static class GuiBar_Patch
    {
        public static bool Prefix(GuiBar __instance, float i)
        {
            if (__instance != null && __instance.m_barImage != null && __instance.m_barImage.type == Image.Type.Filled)
            {
                __instance.m_barImage.fillAmount = i;
                return false;
            }

            return true;
        }
    }
}
