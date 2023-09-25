using HarmonyLib;
using System.Globalization;
using System.Reflection;
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
            if (petName.Contains("<bed>")) __result = __result.Insert(__result.IndexOf(" )"), ", Bed");
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
        public static void Character_ApplyDamage(Character __instance, HitData hit, bool showDamageText, bool triggerEffects, HitData.DamageModifier mod)
        {
            Tameable tameable = __instance.GetComponent<Tameable>();
            if (!__instance.IsTamed() || tameable == null) return;
            string petName = tameable.GetText();

            if (__instance.GetHealth() < 1f && petName.Contains("<bed>"))
            {
                __instance.SetHealth(__instance.GetMaxHealth());
                Player.m_localPlayer.Message(MessageHud.MessageType.Center, tameable.GetHoverName() + " has been wounded!", 0, null);

                if (Game.instance.GetPlayerProfile().HaveCustomSpawnPoint())
                    __instance.transform.position = Game.instance.GetPlayerProfile().GetCustomSpawnPoint();
                else
                    __instance.transform.position = Game.instance.GetPlayerProfile().GetHomePoint();
            }

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

        [HarmonyPatch(typeof(LevelEffects), "Start")]
        [HarmonyPostfix]
        public static void LevelEffects_Start(LevelEffects __instance, Character ___m_character)
        {
            if (!___m_character.IsTamed()) return;
            Tameable tameable = ___m_character.GetComponent<Tameable>();
            ApplyPetTags(tameable, __instance);
        }

        [HarmonyPatch(typeof(Tameable), "RPC_SetName")]
        [HarmonyPostfix]
        public static void Tameable_RPC_SetName(Tameable __instance, long sender, string name, string authorId, Character ___m_character, ZNetView ___m_nview)
        {
            FieldInfo fi = typeof(Character).GetField("m_visual", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            GameObject visual = fi.GetValue(___m_character) as GameObject;
            LevelEffects levelEffects = visual.GetComponent<LevelEffects>();
            ApplyPetTags(__instance, levelEffects);
        }

        private static bool TryParseAttrib(string petName, string attrib, out int value)
        {
            int i;
            value = 0;

            i = petName.IndexOf("<" + attrib + "=");
            if (i < 0) return false;

            string str = petName.Substring(i + attrib.Length + 2);

            i = str.IndexOf(">");
            if (i < 0) return false;
            
            str = str.Substring(0, i);

            bool result = int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
            return result;
        }

        private static void ApplyPetTags(Tameable pet, LevelEffects levelEffects)
        {
            int i;
            Material[] materials = levelEffects.m_mainRender.sharedMaterials;
            materials[0] = new Material(materials[0]);

            string petName = pet.GetText();

            if (TryParseAttrib(petName, "h", out i)) materials[0].SetFloat("_Hue", i / 360f);
            if (TryParseAttrib(petName, "s", out i)) materials[0].SetFloat("_Saturation", i / 100f);
            if (TryParseAttrib(petName, "v", out i)) materials[0].SetFloat("_Value", i / 100f);
            if (TryParseAttrib(petName, "follow", out i)) pet.m_commandable = (i != 0);

            levelEffects.m_mainRender.sharedMaterials = materials;
        }
    }
}
