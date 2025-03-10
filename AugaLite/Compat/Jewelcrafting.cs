﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using AugaLite.Utilities;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace AugaLite.Compat;

public static class Jewelcrafting
{
    public static Assembly ModAssembly;
    public static Type Synergy;
    public static Type DisplaySynergyView;
    public static Type AddSynergyIcon;
    public static Type SocketsBackground;
    public static Type FusionBoxSetup;
    public static Type AddSealButton;
    public static Type GemCursor;
    public static Type CacheVanillaCursor;
    public static Type GemStones;
    public static Type OpenFakeSocketsContainer;
    public static Type CloseFakeSocketsContainer;
    
    [HarmonyBefore(new []{"org.bepinex.plugins.jewelcrafting"})]
    public static void Hud_Awake_Prefix(Hud __instance)
    {
        var hotkeyBar = __instance.Replace("hudroot/HotKeyBar", AugaLite.Assets.Hud, "hudroot/HotKeyBar");
    }

    public static Vector2 ChangeSealPosition(Vector2 anchoredPosition)
    {
        return new Vector2(41f,340f);
    }

    public static Vector2 ChangeTakeAllPosition(Vector2 anchoredPosition)
    {
        return anchoredPosition;
    }

    public static IEnumerable<CodeInstruction> FusionBoxSetup_AddSealButton_Postfix_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var th = new TranspilerHelpers();
        var instrs = instructions.ToList();
        var skipRows = false;

        for (var i = 0; i < instrs.Count; ++i)
        {
            if (i > 0 && instrs[i].opcode == OpCodes.Ldstr && instrs[i].operand.Equals("Text"))
            {
                yield return th.LogMessage(new CodeInstruction(OpCodes.Ldstr,"Label"));
            }
            else if (instrs[i].opcode == OpCodes.Ldloca_S)
            {
                skipRows = true;
            } else if (i > 5 && skipRows && 
                       instrs[i].opcode == OpCodes.Ldloc_1 && instrs[i-1].opcode == OpCodes.Call)
            {
                skipRows = false;
                yield return th.LogMessage(instrs[i]);
                yield return th.LogMessage(new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(typeof(Jewelcrafting), nameof(ChangeSealPosition))));
            } else if (skipRows)
            {
            }
            else
            {
                yield return th.LogMessage(instrs[i]);
            }
        }
    }

    public static IEnumerable<CodeInstruction> GemStones_CloseFakeSocketsContainer_Prefix_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var th = new TranspilerHelpers();
        var instrs = instructions.ToList();
        var skipRows = false;

        for (var i = 0; i < instrs.Count; ++i)
        {
            if (instrs[i].opcode == OpCodes.Dup && instrs[i+1].opcode == OpCodes.Callvirt
                                                  && instrs[i+2].opcode == OpCodes.Stloc_0 
                                                  && instrs[i+3].opcode == OpCodes.Ldloca_S)
            {
                skipRows = true;
            } 
            else if (i > 5 && skipRows && 
                       instrs[i].opcode == OpCodes.Callvirt && instrs[i-1].opcode == OpCodes.Ldloc_0
                       && instrs[i-2].opcode == OpCodes.Dup)
            {
                skipRows = false;
            } 
            else if (skipRows)
            {
            }
            else
            {
                yield return th.LogMessage(instrs[i]);
            }
        }
    }
    public static IEnumerable<CodeInstruction> GemStones_OpenFakeSocketsContainer_Open_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var th = new TranspilerHelpers();
        var instrs = instructions.ToList();
        var skipRows = false;

        for (var i = 0; i < instrs.Count; ++i)
        { 
            if (instrs[i].opcode == OpCodes.Callvirt && instrs[i+1].opcode == OpCodes.Stloc_S)
            {
                yield return th.LogMessage(instrs[i]);
                yield return th.LogMessage(instrs[i+1]);
                yield return th.LogMessage(instrs[i+3]);
                yield return th.LogMessage(new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(typeof(Jewelcrafting), nameof(ChangeTakeAllPosition))));
                yield return th.LogMessage(instrs[i+1]);
                skipRows = true;
            } else if (i > 5 && skipRows && 
                       instrs[i].opcode == OpCodes.Callvirt && instrs[i-1].opcode == OpCodes.Ldloc_S
                       && instrs[i-2].opcode == OpCodes.Ldloc_3)
            {
                skipRows = false;
                yield return th.LogMessage(instrs[i-2]);
                yield return th.LogMessage(instrs[i-1]);
                yield return th.LogMessage(instrs[i]);
            } else if (skipRows)
            {
            }
            else
            {
                yield return th.LogMessage(instrs[i]);
            }
        }
    }

    
    private static Texture2D GetAugaCursor()
    {
        return AugaLite.Assets.Cursor;
    }
    public static IEnumerable<CodeInstruction> GemCursor_CacheVanillaCursor_Postfix_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var instrs = instructions.ToList();
        var th = new TranspilerHelpers();

        var skipRows = false;

        for (var i = 0; i < instrs.Count; ++i)
        {
            if (instrs[i].opcode == OpCodes.Ldsflda)
            {
                yield return th.LogMessage(instrs[i]);

                skipRows = !skipRows;

                if (skipRows)
                {
                    yield return th.LogMessage(new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(typeof(Jewelcrafting), nameof(GetAugaCursor))));
                }
            } else if (instrs[i].opcode == OpCodes.Stfld && skipRows)
            {
                yield return th.LogMessage(instrs[i]);
            } 
            else if (skipRows)
            {
            }
            else
            {
                yield return th.LogMessage(instrs[i]);
            }
        }
    }
    
    public static IEnumerable<CodeInstruction> AddSocketAddingTab_InventoryGui_Awake_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var instrs = instructions.ToList();

        var th = new TranspilerHelpers();

        for (var i = 0; i < instrs.Count; ++i)
        {
            if (false)
            {
                //yield return th.LogMessage(new CodeInstruction(OpCodes.Ldstr,"Text"));
            }
            else
            {
                yield return th.LogMessage(instrs[i]);
            }
        }
    }

    private static bool ConfigureForAuga()
    {
        return true;
    }
    
    [HarmonyAfter(new []{"org.bepinex.plugins.jewelcrafting"})]
    public static void IvnentoryGui_Awake_Postfix(InventoryGui __instance)
    {
        //if (ConfigureForAuga())
        //{
        //   return;
        //}
        
        var jewelCraftingSynergy = __instance.m_player.transform.Find("Jewelcrafting Synergy")?.gameObject;
        var TrashDividerLine = __instance.m_player.transform.Find("TrashDivider/DividerLeft/Line")?.gameObject;
        var StandardDividerCenter = __instance.m_player.transform.Find("StandardDivider/Center")?.gameObject;
        var StandardDividerLeft = __instance.m_player.transform.Find("StandardDivider/Left")?.gameObject;
        
        if (jewelCraftingSynergy != null)
        {
            var rect = jewelCraftingSynergy.RectTransform();
            rect.anchoredPosition += new Vector2(80f, +78f);
            var image = jewelCraftingSynergy.GetComponentInChildren<Image>();
            var imageRect = image.RectTransform();
            image.color = Color.white;
            imageRect.sizeDelta = new Vector2(25, 25);
        }

        if (TrashDividerLine != null)
        {
            var rect = TrashDividerLine.RectTransform();
            rect.anchoredPosition = new Vector2(36f, 0f);
            rect.sizeDelta = new Vector2(-80f, 2f);
        }

        if (StandardDividerCenter != null)
        {
            var rect = StandardDividerCenter.RectTransform();
            rect.anchoredPosition = new Vector2(75f, 0f);
            rect.sizeDelta = new Vector2(-80f, 2f);
        }

        if (StandardDividerLeft != null)
        {
            var rect = StandardDividerLeft.RectTransform();
            rect.anchoredPosition = new Vector2(72f, 0f);
        }

        
    }
    
    public static IEnumerable<CodeInstruction> DisplaySynergyView_Awake_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var instrs = instructions.ToList();

        var th = new TranspilerHelpers();

        for (var i = 0; i < instrs.Count; ++i)
        {
            if (i > 0 && instrs[i].opcode == OpCodes.Ldstr && instrs[i].operand.Equals("ac_text"))
            {
                yield return th.LogMessage(new CodeInstruction(OpCodes.Ldstr,"Text"));
            }
            else
            {
                yield return th.LogMessage(instrs[i]);
            }
        }
    }

}