using Npgsql;
using System.Data;
using System.Threading.Tasks;
using Zamboni11.Components.NHL11.Requests;

namespace Zamboni11;

public class HutTournamentManager
{

    public static async Task SaveTournament(TournamentSaveDataRequest request, long userId)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();
        
        await using var transaction = await conn.BeginTransactionAsync();

        try
        {
            const string insertTournamentSql = @"
                INSERT INTO hut_tournaments (tournament_type, tournament_data)
                VALUES (@type, @data)
                RETURNING tournament_id;";

            await using var tournamentCmd = new NpgsqlCommand(insertTournamentSql, conn, transaction);
            tournamentCmd.Parameters.AddWithValue("type", (int)request.mTournamentType);
            tournamentCmd.Parameters.AddWithValue("data", request.mData);

            var tournamentId = (int)(await tournamentCmd.ExecuteScalarAsync() ?? 0);

            const string insertAssocSql = @"
                INSERT INTO hut_tournament_associations (user_id, tournament_id)
                VALUES (@userId, @tournamentId);";

            await using var assocCmd = new NpgsqlCommand(insertAssocSql, conn, transaction);
            assocCmd.Parameters.AddWithValue("userId", userId);
            assocCmd.Parameters.AddWithValue("tournamentId", tournamentId);

            await assocCmd.ExecuteNonQueryAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public static async Task<byte[]> LoadTournament(TournamentLoadDataRequest request, long userId)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        const string loadSql = @"
            SELECT t.tournament_data 
            FROM hut_tournaments t
            INNER JOIN hut_tournament_associations a ON t.tournament_id = a.tournament_id
            WHERE a.user_id = @userId AND t.tournament_type = @type
            ORDER BY t.tournament_id DESC
            LIMIT 1;";

        await using var cmd = new NpgsqlCommand(loadSql, conn);
        cmd.Parameters.AddWithValue("userId", userId);
        cmd.Parameters.AddWithValue("type", (int)request.mTournamentType);

        await using var reader = await cmd.ExecuteReaderAsync();
        
        if (await reader.ReadAsync())
        {
            return reader.GetFieldValue<byte[]>(0);
        }

        return [];
    }
}