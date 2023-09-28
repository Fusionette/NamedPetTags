using BepInEx;
using HarmonyLib;

namespace NamedPetTags
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class NamedPetTags : BaseUnityPlugin
    {
        private const string modGUID = "Fusionette.NamedPetTags";
        private const string modName = "Named Pet Tags";
        private const string modVersion = "0.9.5";
        private readonly Harmony harmony = new Harmony(modGUID);

        void Awake()
        {
            harmony.PatchAll();
        }

        void OnDestroy()
        {
            harmony.UnpatchSelf();
        }
    }
}
