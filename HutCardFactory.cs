using System;
using System.Collections.Generic;
using System.Linq;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11;

public class HutCardFactory
{
    public static ulong CardIdCounter = 1;

    private static readonly Dictionary<CardType, Range> TrainingCardDbIdRanges = new();

    static HutCardFactory()
    {
        TrainingCardDbIdRanges.Add(CardType.CARDHOUSE_CARD_TYPE_TRAINING_GK_ATTRIBUTE_HIGH, new Range(5003001, 5003005));
        TrainingCardDbIdRanges.Add(CardType.CARDHOUSE_CARD_TYPE_TRAINING_GK_ATTRIBUTE_LOW, new Range(5003006, 5003010));
        TrainingCardDbIdRanges.Add(CardType.CARDHOUSE_CARD_TYPE_TRAINING_GK_ATTRIBUTE_QUICKNESS, new Range(5003011, 5003015));
        TrainingCardDbIdRanges.Add(CardType.CARDHOUSE_CARD_TYPE_TRAINING_GK_ATTRIBUTE_POSITIONING, new Range(5003016, 5003020));
        TrainingCardDbIdRanges.Add(CardType.CARDHOUSE_CARD_TYPE_TRAINING_GK_ATTRIBUTE_REBOUNDCONTROL, new Range(5003021, 5003025));
        TrainingCardDbIdRanges.Add(CardType.CARDHOUSE_CARD_TYPE_TRAINING_GK_ALL, new Range(5003026, 5003028));
        TrainingCardDbIdRanges.Add(CardType.CARDHOUSE_CARD_TYPE_TRAINING_PLAYER_ATTRIBUTE_SKATING, new Range(5003029, 5003033));
        TrainingCardDbIdRanges.Add(CardType.CARDHOUSE_CARD_TYPE_TRAINING_PLAYER_ATTRIBUTE_SHOOTING, new Range(5003034, 5003038));
        TrainingCardDbIdRanges.Add(CardType.CARDHOUSE_CARD_TYPE_TRAINING_PLAYER_ATTRIBUTE_HANDS, new Range(5003039, 5003043));
        TrainingCardDbIdRanges.Add(CardType.CARDHOUSE_CARD_TYPE_TRAINING_PLAYER_ATTRIBUTE_CHECKING, new Range(5003044, 5003048));
        TrainingCardDbIdRanges.Add(CardType.CARDHOUSE_CARD_TYPE_TRAINING_PLAYER_ATTRIBUTE_DEFENSE, new Range(5003049, 5003053));
        TrainingCardDbIdRanges.Add(CardType.CARDHOUSE_CARD_TYPE_TRAINING_PLAYER_ALL, new Range(5003054, 5003056));
    }
    
    public static CardData CreateRandomHeadCoachCard()
    {
        return CreateCard((uint)new Random().Next(2000000, 2000025), CardType.CARDHOUSE_CARD_TYPE_STAFF_HEADCOACH);
    }
    
    public static CardData CreateRandomContractCard()
    {
        return CreateCard((uint)new Random().Next(5001001, 5001011), CardType.CARDHOUSE_CARD_TYPE_CONTRACT_PLAYER);
    }

    public static CardData CreateRandomTrainingCard()
    {
        var random = new Random().Next(TrainingCardDbIdRanges.Count);
        var cardType = TrainingCardDbIdRanges.ElementAt(random).Key;
        return CreateCard((uint)new Random().Next(TrainingCardDbIdRanges[cardType].Start.Value, TrainingCardDbIdRanges[cardType].End.Value), cardType);
    }

    public static CardData CreateRandomLogoCard()
    {
        return CreateCard((uint)new Random().Next(6000000, 6000211), CardType.CARDHOUSE_CARD_TYPE_CUSTOM_BADGE);
    }

    public static CardData CreateRandomStadiumCard()
    {
        return CreateCard((uint)new Random().Next(6200000, 6200005), CardType.CARDHOUSE_CARD_TYPE_CUSTOM_STADIUM);
    }
    
    public static CardData CreateRandomJerseyCard(bool isHome, bool isRare)
    {
        if (isRare) return CreateCard((uint)new Random().Next(6500001 - 1, 6500196 - 1), CardType.CARDHOUSE_CARD_TYPE_CUSTOM_KIT);
        if (isHome) return CreateCard((uint)new Random().Next(6300001 - 1, 6300212 - 1), CardType.CARDHOUSE_CARD_TYPE_CUSTOM_KIT);
        if (!isHome) return CreateCard((uint)new Random().Next(6400001 - 1, 6400212 - 1), CardType.CARDHOUSE_CARD_TYPE_CUSTOM_KIT);
        return CreateCard((uint)new Random().Next(6300001 - 1, 6300212 - 1), CardType.CARDHOUSE_CARD_TYPE_CUSTOM_KIT);
    }

    public static CardData CreateCard(uint dbId, CardType cardType)
    {
        ulong cardId = CardIdCounter++;
        CardData cardData = new CardData()
        {
            mAttributes = null,
            mCardStateId = 1,
            mCardId = (long)cardId,
            mCardDbId = dbId,
            mFormationId = 0,
            mFREE = 0,
            mFitness = 0,
            mInjuryGames = 0,
            mInjuryType = 0,
            mMoral = 0,
            mNumberOfOwners = 0,
            mPreferredPositionId = 0,
            mDiscardPrice = 0,
            mRareFlag = 0,
            mRating = 0,
            mSalaryCap = 0,
            mListStats = null,
            mCardTypeId = cardType,
            mDateIssued = 0,
            mTeamId = 0,
            mListTrainingCards = null,
            mUsesRemaining = 0
        };
        return CreateCard(cardData);
    }

    public static CardData CreateCard(CardData cardData)
    {
        HutManager.CardData.Add(cardData.mCardId, cardData);
        return cardData;
    }
}