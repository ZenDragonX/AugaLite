/*
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace AugaLite
{
    [HarmonyPatch(typeof(ZNet), nameof(ZNet.Awake))]
    public static class ZNet_Awake_Patch
    {
        [UsedImplicitly]
        public static void Postfix(ZNet __instance)
        {
            var parent = __instance.m_passwordDialog.parent;
            Object.Destroy(__instance.m_passwordDialog.gameObject);
            var newPasswordDialog = Object.Instantiate(AugaLite.Assets.PasswordDialog, parent, false);
            newPasswordDialog.gameObject.SetActive(false);
            __instance.m_passwordDialog = newPasswordDialog.GetComponent<RectTransform>();

            Object.Destroy(__instance.m_connectingDialog.gameObject);
            var newConnectingDialog = Object.Instantiate(AugaLite.Assets.ConnectingDialog, parent, false);
            newConnectingDialog.gameObject.SetActive(false);
            __instance.m_connectingDialog = newConnectingDialog.GetComponent<RectTransform>();
        }
    }
}

*/