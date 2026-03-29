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

        const string upsertSql = @"
            INSERT INTO hut_tournaments (user_id, tournament_type, tournament_data)
            VALUES (@user_id, @tournament_type, @tournament_data)
            ON CONFLICT (user_id, tournament_type) 
            DO UPDATE SET 
            tournament_data = EXCLUDED.tournament_data;"; 

        await using var cmd = new NpgsqlCommand(upsertSql, conn);
        cmd.Parameters.AddWithValue("user_id", userId);
        cmd.Parameters.AddWithValue("tournament_type", (int)request.mTournamentType);
        cmd.Parameters.AddWithValue("tournament_data", request.mData);

        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task<byte[]> LoadTournamentData(TournamentLoadDataRequest request, long userId)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        const string sql = "SELECT tournament_data FROM hut_tournaments WHERE user_id = @uid AND tournament_type = @tt;";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("uid", userId);
        cmd.Parameters.AddWithValue("tt", (int)request.mTournamentType);

        await using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return reader.GetFieldValue<byte[]>(0);
        }

        return [];
    }
}