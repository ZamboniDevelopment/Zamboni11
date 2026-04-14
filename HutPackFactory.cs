using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11;

public class HutPackFactory
{
    public static async Task<List<CardData>> CreatePack(long userId, PackType packType)
    {
        var cardDataList = new List<CardData>();
        await HutManager.IncrementVersionInfo(userId, HutManager.VersionType.Unassigned);

        switch (packType)
        {
            case PackType.CARDHOUSE_CARD_PACK_TYPE_STARTER:
            {
                cardDataList.Add(await HutCardFactory.CreateRandomJerseyCard(userId, true, false));
                cardDataList.Add(await HutCardFactory.CreateRandomJerseyCard(userId, false, false));

                cardDataList.Add(await HutCardFactory.CreateRandomLogoCard(userId));

                cardDataList.Add(await HutCardFactory.CreateRandomStadiumCard(userId));

                cardDataList.Add(await HutCardFactory.CreateRandomTrainingCard(userId));

                cardDataList.Add(await HutCardFactory.CreateRandomContractCard(userId));

                var starterOverallRange = new Range(0, 85);

                cardDataList.Add(await HutCardFactory.RollPlayerCard(userId, cardDataList, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_GK, starterOverallRange, 0, true));
                cardDataList.Add(await HutCardFactory.RollPlayerCard(userId, cardDataList, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_GK, starterOverallRange, 0, true));
                cardDataList.Add(await HutCardFactory.RollPlayerCard(userId, cardDataList, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_D, starterOverallRange, 0, true));
                cardDataList.Add(await HutCardFactory.RollPlayerCard(userId, cardDataList, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_D, starterOverallRange, 0, true));
                cardDataList.Add(await HutCardFactory.RollPlayerCard(userId, cardDataList, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_D, starterOverallRange, 0, true));
                cardDataList.Add(await HutCardFactory.RollPlayerCard(userId, cardDataList, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_D, starterOverallRange, 0, true));
                cardDataList.Add(await HutCardFactory.RollPlayerCard(userId, cardDataList, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_D, starterOverallRange, 0, true));
                cardDataList.Add(await HutCardFactory.RollPlayerCard(userId, cardDataList, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_D, starterOverallRange, 0, true));
                cardDataList.Add(await HutCardFactory.RollPlayerCard(userId, cardDataList, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_D, starterOverallRange, 0, true));
                cardDataList.Add(await HutCardFactory.RollPlayerCard(userId, cardDataList, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_LW, starterOverallRange, 0, true));
                cardDataList.Add(await HutCardFactory.RollPlayerCard(userId, cardDataList, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_LW, starterOverallRange, 0, true));
                cardDataList.Add(await HutCardFactory.RollPlayerCard(userId, cardDataList, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_LW, starterOverallRange, 0, true));
                cardDataList.Add(await HutCardFactory.RollPlayerCard(userId, cardDataList, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_LW, starterOverallRange, 0, true));
                cardDataList.Add(await HutCardFactory.RollPlayerCard(userId, cardDataList, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_RW, starterOverallRange, 0, true));
                cardDataList.Add(await HutCardFactory.RollPlayerCard(userId, cardDataList, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_RW, starterOverallRange, 0, true));
                cardDataList.Add(await HutCardFactory.RollPlayerCard(userId, cardDataList, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_RW, starterOverallRange, 0, true));
                cardDataList.Add(await HutCardFactory.RollPlayerCard(userId, cardDataList, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_RW, starterOverallRange, 0, true));
                cardDataList.Add(await HutCardFactory.RollPlayerCard(userId, cardDataList, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_C, starterOverallRange, 0, true));
                cardDataList.Add(await HutCardFactory.RollPlayerCard(userId, cardDataList, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_C, starterOverallRange, 0, true));
                cardDataList.Add(await HutCardFactory.RollPlayerCard(userId, cardDataList, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_C, starterOverallRange, 0, true));
                cardDataList.Add(await HutCardFactory.RollPlayerCard(userId, cardDataList, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_C, starterOverallRange, 0, true));
                cardDataList.Add(await HutCardFactory.RollPlayerCard(userId, cardDataList, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_C, starterOverallRange, 60, true));
                cardDataList.Add(await HutCardFactory.RollPlayerCard(userId, cardDataList, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_C, starterOverallRange, 80, true));

                return cardDataList;
            }
            case PackType.CARDHOUSE_CARD_PACK_TYPE_PEEWEE:
            {
                cardDataList.Add(await HutCardFactory.CreatePlayerCard(userId, 3673));
                cardDataList.Add(await HutCardFactory.CreateNonPlayerCard(userId, 6300004, CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_KIT));
                cardDataList.Add(await HutCardFactory.CreateRandomJerseyCard(userId, false, false));
                cardDataList.Add(await HutCardFactory.CreateNonPlayerCard(userId, 5003004, CardSubType.CARDHOUSE_CARD_TYPE_TRAINING_GK_ATTRIBUTE_HIGH));
                cardDataList.Add(await HutCardFactory.CreateRandomLogoCard(userId));
                cardDataList.Add(await HutCardFactory.CreateRandomContractCard(userId));
                cardDataList.Add(await HutCardFactory.CreateRandomStadiumCard(userId));
                cardDataList.Add(await HutCardFactory.CreateRandomHeadCoachCard(userId));

                return cardDataList;
            }
            default: throw new NotImplementedException();
        }
    }
}