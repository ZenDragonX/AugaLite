﻿/*
using AugaUnity;
using HarmonyLib;

namespace AugaLite;

[HarmonyPatch]
public static class Barber_Setup
{
    [HarmonyPatch(typeof(PlayerCustomizaton), nameof(PlayerCustomizaton.OnEnable))]
    public static class PlayerCustomization_Awake_Patch
    {
        public static bool Prefix(PlayerCustomizaton __instance)
        {
            return !SetupHelper.DirectObjectReplace(__instance.transform, AugaLite.Assets.AugaBarber, "BarberGui");
        }
    }

    [HarmonyPatch(typeof(Game), nameof(Game._RequestRespawn))]
    public static class GameRequestRespawn_Awake_Patch
    {
        public static void Postfix(PlayerCustomizaton __instance)
        {
            AugaBarberController.Instance.ResetLocalPlayer();
        }
    }

}

*/