using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11;

public class HutCardFactory
{
    public static long CardIdCounter = 1;

    private static readonly Dictionary<CardSubType, Range> TrainingCardDbIdRanges = new();

    static HutCardFactory()
    {
        TrainingCardDbIdRanges.Add(CardSubType.CARDHOUSE_CARD_TYPE_TRAINING_GK_ATTRIBUTE_HIGH, new Range(5003001, 5003005));
        TrainingCardDbIdRanges.Add(CardSubType.CARDHOUSE_CARD_TYPE_TRAINING_GK_ATTRIBUTE_LOW, new Range(5003006, 5003010));
        TrainingCardDbIdRanges.Add(CardSubType.CARDHOUSE_CARD_TYPE_TRAINING_GK_ATTRIBUTE_QUICKNESS, new Range(5003011, 5003015));
        TrainingCardDbIdRanges.Add(CardSubType.CARDHOUSE_CARD_TYPE_TRAINING_GK_ATTRIBUTE_POSITIONING, new Range(5003016, 5003020));
        TrainingCardDbIdRanges.Add(CardSubType.CARDHOUSE_CARD_TYPE_TRAINING_GK_ATTRIBUTE_REBOUNDCONTROL, new Range(5003021, 5003025));
        TrainingCardDbIdRanges.Add(CardSubType.CARDHOUSE_CARD_TYPE_TRAINING_GK_ALL, new Range(5003026, 5003028));
        TrainingCardDbIdRanges.Add(CardSubType.CARDHOUSE_CARD_TYPE_TRAINING_PLAYER_ATTRIBUTE_SKATING, new Range(5003029, 5003033));
        TrainingCardDbIdRanges.Add(CardSubType.CARDHOUSE_CARD_TYPE_TRAINING_PLAYER_ATTRIBUTE_SHOOTING, new Range(5003034, 5003038));
        TrainingCardDbIdRanges.Add(CardSubType.CARDHOUSE_CARD_TYPE_TRAINING_PLAYER_ATTRIBUTE_HANDS, new Range(5003039, 5003043));
        TrainingCardDbIdRanges.Add(CardSubType.CARDHOUSE_CARD_TYPE_TRAINING_PLAYER_ATTRIBUTE_CHECKING, new Range(5003044, 5003048));
        TrainingCardDbIdRanges.Add(CardSubType.CARDHOUSE_CARD_TYPE_TRAINING_PLAYER_ATTRIBUTE_DEFENSE, new Range(5003049, 5003053));
        TrainingCardDbIdRanges.Add(CardSubType.CARDHOUSE_CARD_TYPE_TRAINING_PLAYER_ALL, new Range(5003054, 5003056));
    }

    public static CardData CreateRandomHeadCoachCard(long owner)
    {
        return CreateNonPlayerCard(owner, (uint)new Random().Next(2000000, 2000025), CardSubType.CARDHOUSE_CARD_TYPE_STAFF_HEADCOACH);
    }



    public static CardData CreateRandomContractCard(long owner)
    {
        return CreateNonPlayerCard(owner, (uint)new Random().Next(5001001, 5001011), CardSubType.CARDHOUSE_CARD_TYPE_CONTRACT_PLAYER);
    }

    public static CardData CreateRandomTrainingCard(long owner)
    {
        var random = new Random().Next(TrainingCardDbIdRanges.Count);
        var cardType = TrainingCardDbIdRanges.ElementAt(random).Key;
        return CreateNonPlayerCard(owner, (uint)new Random().Next(TrainingCardDbIdRanges[cardType].Start.Value, TrainingCardDbIdRanges[cardType].End.Value), cardType);
    }

    public static CardData CreateRandomLogoCard(long owner)
    {
        return CreateNonPlayerCard(owner, (uint)new Random().Next(6000000, 6000211), CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_BADGE);
    }

    public static CardData CreateRandomStadiumCard(long owner)
    {
        return CreateNonPlayerCard(owner, (uint)new Random().Next(6200000, 6200005), CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_STADIUM,104);
    }

    public static CardData CreateRandomJerseyCard(long owner, bool isHome, bool isRare)
    {
        if (isRare) return CreateNonPlayerCard(owner, (uint)new Random().Next(6500001 - 1, 6500196 - 1), CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_KIT);
        if (isHome) return CreateNonPlayerCard(owner, (uint)new Random().Next(6300001 - 1, 6300212 - 1), CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_KIT,101);
        if (!isHome) return CreateNonPlayerCard(owner, (uint)new Random().Next(6400001 - 1, 6400212 - 1), CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_KIT,102);
        return CreateNonPlayerCard(owner, (uint)new Random().Next(6300001 - 1, 6300212 - 1), CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_KIT);
    }

    public static CardData CreateNonPlayerCard(long owner, uint dbId, CardSubType cardSubType, byte cardStateId = 1)
    {
        long cardId = CardIdCounter++;
        CardData cardData = new CardData()
        {
            mAttributes = null,
            mCardStateId = cardStateId,
            mCardId = cardId,
            mCardDbId = dbId,
            mFormationId = 0,
            mFREE = 0,
            mCareerRemaining = 0,
            mInjuryGames = 0,
            mInjuryType = 0,
            mMaxTrainingCardsCanApply = 0,
            mNumberOfOwners = 0,
            mPreferredPositionId = (byte)cardSubType,
            mDiscardPrice = 0,
            mRareFlag = 0,
            mRating = 0,
            mSalaryCap = 0,
            mListStats = null,
            mCardSubTypeId = cardSubType,
            mDateIssued = 0,
            mTeamId = 0,
            mListTrainingCards = null,
            mUsesRemaining = 0
        };
        return CreateCard(owner, cardId, cardData);
    }
    
    public static async Task<CardData> CreateRandomPlayerCard(long owner, CardSubType position)
    {
        if (position > CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_GK) throw new Exception("Position must be 0-4");
        List<uint> dbIds = await Program.Database.GetListDbIds(position);
        uint cardDbId = dbIds[new Random().Next(dbIds.Count)];
        return await CreatePlayerCard(owner, cardDbId);
    }
    
    public static async Task <CardData> CreatePlayerCard(long owner, uint dbId)
    {
        var cardData = Program.Database.GetCardDataByDbId(dbId);
        CreateCard(owner, cardData.Result.Value.mCardId, cardData.Result.Value);
        return (CardData)cardData.Result;
    }

    public static CardData CreateCard(long ownerUserId, long cardId, CardData cardData)
    {
        if (!HutManager.UserInventories.ContainsKey(ownerUserId))
        {
            HutManager.UserInventories[ownerUserId] = new Dictionary<long, CardData>();
        }

        HutManager.UserInventories[ownerUserId][cardId] = cardData;
        return cardData;
    }
}