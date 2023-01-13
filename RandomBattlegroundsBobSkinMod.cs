using MelonLoader;
using HarmonyLib;
using System.Linq;
using System;

namespace RandomBattlegroundsBobSkin
{
    public class RandomBattlegroundsBobSkinMod : MelonMod
    {
        public static MelonLogger.Instance SharedLogger;

        public static string currentBobSkin = null;

        public override void OnInitializeMelon()
        {
            RandomBattlegroundsBobSkinMod.SharedLogger = LoggerInstance;
            var harmony = this.HarmonyInstance;
            harmony.PatchAll(typeof(EntityPatcher));
            harmony.PatchAll(typeof(GameMgrPatcher));
        }
    }


    public static class GameMgrPatcher
    {
        [HarmonyPatch(typeof(GameMgr), "OnGameSetup")]
        [HarmonyPostfix]
        public static void OnGameSetupPostfix()
        {
            if (GameMgr.Get().IsBattlegrounds())
            {
                RandomBattlegroundsBobSkinMod.currentBobSkin = Utils.GetRandomBobSkin();
            }
        }
    }

    // Implementation insired by https://github.com/Pik-4/HsMod/blob/master/HsMod/Patcher.cs#L1955
    public static class EntityPatcher
    {
        [HarmonyPatch(typeof(Entity), "LoadCard", new Type[] { typeof(string), typeof(Entity.LoadCardData) })]
        [HarmonyPrefix]
        public static bool Prefix(Entity __instance, ref string cardId)
        {
            if (cardId != null && Utils.IsHero(cardId, out Assets.CardHero.HeroType heroType))
            {
                if (heroType == Assets.CardHero.HeroType.BATTLEGROUNDS_GUIDE)
                {
                    cardId = RandomBattlegroundsBobSkinMod.currentBobSkin;
                    RandomBattlegroundsBobSkinMod.SharedLogger.Msg($"Assigned new skin {cardId} to Bob");
                }
            }
            return true;
        }
    }
}
