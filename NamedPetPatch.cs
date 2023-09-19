using HarmonyLib;
using UnityEngine;

namespace NamedPetTags
{
    [HarmonyPatch]
    internal class NamedPetPatch
    {
        [HarmonyPatch(typeof(Tameable), "GetHoverText")]
        [HarmonyPostfix]
        public static void Tameable_GetHoverText(Tameable __instance, ref string __result)
        {
            string petName = __instance.GetText();

            if (petName.Contains("<pet>")) __result = __result.Insert(__result.IndexOf(" )"), ", Pet");
            if (petName.Contains("<spay>")) __result = __result.Insert(__result.IndexOf(" )"), ", Spayed");
        }

        [HarmonyPatch(typeof(Tameable), "SetName")]
        [HarmonyPrefix]
        public static bool Tameable_SetName(Tameable __instance, Character ___m_character)
        {
            if (!___m_character.IsTamed()) return false;
            TextInput.instance.RequestText(__instance, "$hud_rename", 100);
            return false;
        }

        [HarmonyPatch(typeof(Character), "ApplyDamage")]
        [HarmonyPostfix]
        public static void Character_ApplyDamage(Character __instance, HitData hit, ref bool showDamageText, ref bool triggerEffects, ref HitData.DamageModifier mod, Tameable ___m_tameable)
        {
            if (!__instance.IsTamed() || ___m_tameable == false) return;
            string petName = ___m_tameable.GetText();

            if (petName.Contains("<pet>"))
            {
                __instance.SetHealth(__instance.GetMaxHealth());
            }
        }

        [HarmonyPatch(typeof(Procreation), "ReadyForProcreation")]
        [HarmonyPostfix]
        public static void Procreation_ReadyForProcreation(Tameable ___m_tameable, ref bool __result)
        {
            string petName = ___m_tameable.GetText();

            if (petName.Contains("<spay>"))
            {
                __result = false;
            }
        }

        [HarmonyPatch(typeof(Procreation), "MakePregnant")]
        [HarmonyPostfix]
        public static void Procreation_MakePregnant(Tameable ___m_tameable, ZNetView ___m_nview)
        {
            string petName = ___m_tameable.GetText();

            if (petName.Contains("<spay>"))
            {
                ___m_nview.GetZDO().Set(ZDOVars.s_pregnant, 0L);
            }
        }

        [HarmonyPatch(typeof(Procreation), "IsDue")]
        [HarmonyPostfix]
        public static void Procreation_IsDue(Tameable ___m_tameable, ZNetView ___m_nview, ref bool __result)
        {
            string petName = ___m_tameable.GetText();

            if (petName.Contains("<spay>"))
            {
                ___m_nview.GetZDO().Set(ZDOVars.s_pregnant, 0L);
                __result = false;
            }
        }
    }
}
