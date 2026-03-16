using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11;

public class HutPackFactory
{
    public static async Task<List<CardData>> CreatePack(long userId, PackType packType)
    {
        switch (packType)
        {
            case PackType.CARDHOUSE_CARD_PACK_TYPE_STARTER:
            {
                var cardDataList = new List<CardData>();

                cardDataList.Add(await HutCardFactory.CreateRandomJerseyCard(userId, true, false));
                cardDataList.Add(await HutCardFactory.CreateRandomJerseyCard(userId, false, false));
                cardDataList.Add(await HutCardFactory.CreateRandomLogoCard(userId));
                cardDataList.Add(await HutCardFactory.CreateRandomStadiumCard(userId));
                // cardDataList.Add(await HutCardFactory.CreateRandomHeadCoachCard(userId));
                cardDataList.Add(await HutCardFactory.CreateRandomTrainingCard(userId));
                cardDataList.Add(await HutCardFactory.CreateRandomContractCard(userId));
                cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_GK));
                cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_GK));
                cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_D));
                cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_D));
                cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_D));
                cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_D));
                cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_D));
                cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_D));
                cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_D));
                cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_LW));
                cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_LW));
                cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_LW));
                cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_LW));
                cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_RW));
                cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_RW));
                cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_RW));
                cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_RW));
                cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_C));
                cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_C));
                cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_C));
                cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_C));
                cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_C));
                cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_C));
                return cardDataList;
            }
            case PackType.CARDHOUSE_CARD_PACK_TYPE_PEEWEE:
            {
                var cardDataList = new List<CardData>();

                cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_LW));
                cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_RW));
                cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_C));
                return cardDataList;
            }
            default: throw new NotImplementedException();
        }
    }
}