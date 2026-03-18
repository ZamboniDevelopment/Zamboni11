using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11;

public class HutHelper
{
    public static CardData ReadCardData(NpgsqlDataReader reader)
    {
        var cardData = new CardData
        {
            mAttributes = reader.GetFieldValue<byte[]>(reader.GetOrdinal("attributes")).ToList(),
            mCardStateId = (CardState)(byte)reader.GetInt16(reader.GetOrdinal("state_id")),
            mCardId = reader.GetInt64(reader.GetOrdinal("card_id")),
            mCardDbId = (uint)reader.GetInt32(reader.GetOrdinal("db_id")),
            mFormationId = (byte)reader.GetInt16(reader.GetOrdinal("formation_id")),
            mFREE = (byte)reader.GetInt16(reader.GetOrdinal("free")),
            mCareerRemaining = (byte)reader.GetInt16(reader.GetOrdinal("career_remaining")),
            mInjuryGames = (byte)reader.GetInt16(reader.GetOrdinal("injury_games")),
            mInjuryType = (byte)reader.GetInt16(reader.GetOrdinal("injury_type")),
            mMaxTrainingCardsCanApply = (byte)reader.GetInt16(reader.GetOrdinal("morale")),
            // mNumberOfOwners = (byte)reader.GetInt16(reader.GetOrdinal("free")), ///TODO
            mPreferredPositionId = (byte)reader.GetInt16(reader.GetOrdinal("preferred_position_id")),
            mDiscardPrice = (byte)reader.GetInt16(reader.GetOrdinal("discard_price")),
            mRareFlag = (byte)reader.GetInt16(reader.GetOrdinal("rare_flag")),
            mRating = (byte)reader.GetInt16(reader.GetOrdinal("rating")),
            mSalaryCap = reader.GetInt16(reader.GetOrdinal("salary_cap")),
            mListStats = reader.GetFieldValue<int[]>(reader.GetOrdinal("list_stats")).ToList(),
            mCardSubTypeId = (CardSubType)reader.GetInt16(reader.GetOrdinal("sub_type")),
            mDateIssued = (uint)reader.GetInt64(reader.GetOrdinal("date_issued")),
            mTeamId = (uint)reader.GetInt32(reader.GetOrdinal("team_id")),
            mListTrainingCards = reader.GetFieldValue<int[]>(reader.GetOrdinal("list_training_cards")).ToList(),
            mUsesRemaining = (byte)reader.GetInt16(reader.GetOrdinal("uses_remaining"))
        };
        return cardData;
    }

    public static async Task<ISTradeInfo> ReadTrade(NpgsqlDataReader reader, long readerUserId)
    {
        YourBid yourBid = await HutTradeManager.DetermineMyBidState(reader.GetInt64(reader.GetOrdinal("trade_id")), readerUserId);
        CardData cardData = (await HutManager.GetCard(reader.GetInt64(reader.GetOrdinal("card_id")))).Card;
        return new ISTradeInfo
        {
            mBlazeUserId = reader.GetInt64(reader.GetOrdinal("user_id")),
            // mCardData = HutManager.GetCard(reader.GetInt64(reader.GetOrdinal("card_id"))),
            mCardData = cardData,

            mTradeId = reader.GetInt64(reader.GetOrdinal("trade_id")),
            mUserId = reader.GetInt64(reader.GetOrdinal("user_id")),
            mYourBidState = yourBid,
            mCardId = reader.GetInt64(reader.GetOrdinal("card_id")),
            mStartingPrice = reader.GetInt32(reader.GetOrdinal("starting_price")),
            mCardDbId = cardData.mCardDbId,
            mSellerEstDate = 0,
            mHighestBid = reader.GetInt32(reader.GetOrdinal("highest_bid")),
            // mInbox = 0,
            // mIsWatched = 0,
            // mOfferPendingCount = 0,
            mBuyOutPrice = reader.GetInt32(reader.GetOrdinal("buy_out_price")),
            mSellerName = reader.GetString(reader.GetOrdinal("seller_name")),
            mTradeState = (TradeState)reader.GetInt32(reader.GetOrdinal("trade_state")),
            mSecondsLeft = reader.GetInt32(reader.GetOrdinal("expire_time")),
            // mGlow = 0,
        };
    }
}