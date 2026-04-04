using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11;

public class HutPackFactory
{
    public static async Task<(List<CardData> CardList, List<CardIdPair> CardIdPairs)> CreatePack(long userId, PackType packType)
    {
        var cardDataList = new List<CardData>();
        var cardIdPairs = new List<CardIdPair>();

        switch (packType)
        {
            case PackType.CARDHOUSE_CARD_PACK_TYPE_STARTER:
            {
                cardDataList.Add(await HutCardFactory.CreateRandomJerseyCard(userId, true, false));
                cardDataList.Add(await HutCardFactory.CreateRandomJerseyCard(userId, false, false));
                cardDataList.Add(await HutCardFactory.CreateRandomJerseyCard(userId, false, false));

                cardDataList.Add(await HutCardFactory.CreateRandomJerseyCard(userId, true, false));

                // cardDataList.Add(await HutCardFactory.CreateNonPlayerCard(userId, 6300021, CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_KIT));
                // cardDataList.Add(await HutCardFactory.CreateNonPlayerCard(userId, 6300021, CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_KIT));


                cardDataList.Add(await HutCardFactory.CreateRandomLogoCard(userId));
                cardDataList.Add(await HutCardFactory.CreateRandomLogoCard(userId));
                cardDataList.Add(await HutCardFactory.CreateRandomLogoCard(userId));

                cardDataList.Add(await HutCardFactory.CreateRandomStadiumCard(userId));
                cardDataList.Add(await HutCardFactory.CreateRandomHeadCoachCard(userId));
                cardDataList.Add(await HutCardFactory.CreateRandomTrainingCard(userId));
                cardDataList.Add(await HutCardFactory.CreateRandomContractCard(userId));
                // cardDataList.Add(await HutCardFactory.CreatePlayerCard(userId, 4226));
                // cardDataList.Add(await HutCardFactory.CreatePlayerCard(userId, 4226));
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

                HashSet<CardData> seen = new();

                foreach (var VARIABLE in cardDataList)
                {
                    foreach (var VARIABLEB in seen)
                    {
                        if (VARIABLEB.mCardDbId == VARIABLE.mCardDbId)
                        {
                            cardIdPairs.Add(new CardIdPair
                            {
                                mCardId = VARIABLE.mCardId,
                                mDuplicateCardId = VARIABLEB.mCardId
                            });
                        }
                    }

                    seen.Add(VARIABLE);
                }

                return (cardDataList, cardIdPairs);
            }
            case PackType.CARDHOUSE_CARD_PACK_TYPE_PEEWEE:
            {
                // cardDataList.Add(await HutCardFactory.CreatePlayerCard(userId, 4226));
                cardDataList.Add(await HutCardFactory.CreatePlayerCard(userId, 3673));
                // cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_C));
                // cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_C));
                
                HashSet<CardData> seen = new();

                // foreach (var VARIABLE in cardDataList)
                // {
                //     foreach (var VARIABLEB in seen)
                //     {
                //         if (VARIABLEB.mCardDbId == VARIABLE.mCardDbId)
                //         {
                //             cardIdPairs.Add(new CardIdPair
                //             {
                //                 mCardId = VARIABLE.mCardId,
                //                 mDuplicateCardId = VARIABLEB.mCardId
                //             });
                //         }
                //     }
                //
                //     seen.Add(VARIABLE);
                // }
                
                cardIdPairs.Add(new CardIdPair
                {
                    mCardId = 33,
                    mDuplicateCardId = 20
                });
                
                
                return (cardDataList , cardIdPairs);
            }
            default: throw new NotImplementedException();
        }
    }
}