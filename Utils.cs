using Assets;
using System.Collections.Generic;
using System.Linq;

namespace RandomBattlegroundsBobSkin
{
    public class Utils
    {
        public static Dictionary<int, CardHero.HeroType> CacheHeroes = new Dictionary<int, CardHero.HeroType>();
        public static List<string> CacheBattlegroundsHeroes = new List<string>();

        public static class CacheInfo
        {
            public static void UpdateBattlegroundsSkin()
            {
                CacheBattlegroundsHeroes.Clear();
                HashSet<Hearthstone.BattlegroundsGuideSkinId> ownedBgSkins = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsGuideSkins>().OwnedBattlegroundsGuideSkins;
                var allBobSkins = CollectionManager.Get().GetAllBattlegroundsGuideCardIds();
                foreach (var cardId in allBobSkins)
                {
                    var dbId = GameUtils.TranslateCardIdToDbId(cardId);
                    var skinId = new Hearthstone.BattlegroundsGuideSkinId();
                    CollectionManager.Get().GetBattlegroundsGuideSkinIdForCardId(dbId, out skinId);
                    if (!ownedBgSkins.Contains(skinId))
                    {
                        continue;
                    }
                    CacheBattlegroundsHeroes.Add(cardId);
                }

            }

            public static void UpdateHeroes()
            {
                CacheHeroes.Clear();
                foreach (var record in GameDbf.CardHero.GetRecords())
                {
                    if (record != null)
                    {
                        CacheHeroes.Add(record.CardId, record.HeroType);
                    }
                }
            }
        }

        public static string GetRandomBobSkin()
        {
            if (CacheBattlegroundsHeroes.Count == 0)
            {
                CacheInfo.UpdateBattlegroundsSkin();
            }
            var skin = CacheBattlegroundsHeroes[UnityEngine.Random.Range(0, CacheBattlegroundsHeroes.Count)];
            var allSkinsInfo = CacheBattlegroundsHeroes.Select(id => $"(id={id})");
            RandomBattlegroundsBobSkinMod.SharedLogger.Msg($"Loaded new skin for Bob {skin}. All options were {string.Join(", ", allSkinsInfo)}");
            return skin;
        }

        public static bool IsHero(string cardID, out CardHero.HeroType heroType)
        {
            if (CacheHeroes.Count == 0)
            {
                CacheInfo.UpdateHeroes();
            }
            int dbid = GameUtils.TranslateCardIdToDbId(cardID);
            if (CacheHeroes.ContainsKey(dbid))
            {
                heroType = CacheHeroes[dbid];
                return true;
            }
            else
            {
                heroType = CardHero.HeroType.UNKNOWN;
                return false;
            }
        }
    }
}
