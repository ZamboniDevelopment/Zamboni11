using System.Collections.Generic;
using BlazeCommon;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11;

public class HutManager
{
    //TODO: OF COURSE SAVE TO DISK
    public static List<HutPlayerInstance> HutPlayerInstances = new();
    public static Dictionary<long, CardData> CardData = new();

    static HutManager()
    {
        CardData.Add(0,new CardData
        {
            mAttributes = null,
            mCardStateId = 0,
            mCardId = 0,
            mCardDbId = 0,
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
            mCardTypeId = CardType.CARDHOUSE_CARD_TYPE_PLAYER_C,
            mDateIssued = 0,
            mTeamId = 0,
            mListTrainingCards = null,
            mUsesRemaining = 0
        });
    }
    
    public static HutPlayerInstance? GetHutPlayerInstance(ServerPlayer serverPlayer)
    {
        return HutPlayerInstances.Find(instance => instance.ServerPlayer.Equals(serverPlayer));
    }
    
    public static HutPlayerInstance? GetHutPlayerInstance(BlazeServerConnection blazeServerConnection)
    {
        return HutPlayerInstances.Find(instance => instance.ServerPlayer.Equals(ServerManager.GetServerPlayer(blazeServerConnection)));
    }
    
    public static void AddHutPlayerInstance(HutPlayerInstance hutPlayerInstance)
    {
        HutPlayerInstances.Add(hutPlayerInstance);
    }

    public static CardData GetCard(long cardId)
    {
        return CardData[cardId];
    }
}