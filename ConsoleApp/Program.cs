using Npgsql;
using NpgsqlTypes;
using System.Security.Cryptography;
using System.Text;

namespace Battleship;

class Program
{
    static async Task Main()
    {
        // change this
        var connectionString = "Host=localhost;Username=username;Password=password;Database=battleshipdb";
        await using var dataSource = NpgsqlDataSource.Create(connectionString);

        await CreateTables(dataSource);
        await GenerateTables(dataSource);
        await PrintTables(dataSource);
    }

    static async Task PrintTables(NpgsqlDataSource dataSource)
    {
        await using var playersCommand = dataSource.CreateCommand("SELECT id, username, created_at FROM players");
        await using var playersReader = await playersCommand.ExecuteReaderAsync();

        Console.WriteLine("[Players]");

        while (await playersReader.ReadAsync())
        {
            for (int i = 0; i < playersReader.FieldCount; i++)
            {
                Console.Write(playersReader[i]?.ToString()?.PadRight(10));
            }
            Console.WriteLine();
        }

        Console.WriteLine();

        await using var roomsCommand = dataSource.CreateCommand("SELECT * FROM rooms");
        await using var roomsReader = await roomsCommand.ExecuteReaderAsync();

        Console.WriteLine("[Rooms]");

        while (await roomsReader.ReadAsync())
        {
            for (int i = 0; i < roomsReader.FieldCount; ++i)
            {
                Console.Write(roomsReader[i]?.ToString()?.PadRight(8) + "  ");
            }
            Console.WriteLine();
        }
    }

    static async Task CreateTables(NpgsqlDataSource dataSource)
    {
        await using var command = dataSource.CreateCommand(@"
        create table players
        (
            id serial primary key,
            username varchar(20) not null unique,
            password varchar not null,
            created_at timestamp not null default current_timestamp
        );

        create table rooms
        (
            game_code varchar(6) primary key,
            is_public boolean not null default TRUE,
            player1_id integer references players(id) not null,
            player2_id integer references players(id) check (player2_id != player1_id),
            winner_id integer references players(id),
            game_started_at timestamp not null,
            game_ended_at timestamp check (game_ended_at > game_started_at),
            ships_position json
        );

        create table history
        (
            id serial primary key,
            game_code varchar(6) not null,
            x integer not null,
            y integer not null
        )
        ");
        await command.ExecuteNonQueryAsync();
    }

    static async Task GenerateTables(NpgsqlDataSource dataSource)
    {
        for (int i = 1; i <= 10; ++i)
        {
            await using var command = dataSource.CreateCommand(@"
                INSERT INTO players (username, password)
                VALUES (@username, @password);
            ");

            command.Parameters.AddRange(new NpgsqlParameter[]
            { 
                new NpgsqlParameter("@username", NpgsqlDbType.Varchar) { Value = $"Player{i}" },
                new NpgsqlParameter("@password", NpgsqlDbType.Varchar) { Value = HashPassword(GenerateString(8)) }
            });

            await command.ExecuteNonQueryAsync();
        }

        Random random = new Random();
        int player1Id = 1;
        int player2Id = 2;

        for (int i = 1; i <= 5; ++i)
        {
            await using var command = dataSource.CreateCommand(@"
                INSERT INTO rooms (game_code, is_public, player1_id, player2_id, winner_id, game_started_at, game_ended_at)
                VALUES (@game_code, @is_public, @player1_id, @player2_id, @winner_id, @game_started_at, @game_ended_at);
            ");

            string gameCode = GenerateString(6);
            bool isPublic = random.Next(0, 2) == 1 ? true : false;
            int winnerId = random.Next(0, 2) == 1 ? player1Id : player2Id;

            int hours = random.Next(-24, -1);
            int minutes = random.Next(-60, 60);
            DateTime gameStartedAt = DateTime.Now.AddHours(hours).AddMinutes(minutes);
            DateTime gameEndedAt = DateTime.Now.AddHours(hours).AddMinutes(minutes + random.Next(5, 25));

            command.Parameters.AddRange(new NpgsqlParameter[]
            {
                new NpgsqlParameter("@game_code", NpgsqlDbType.Varchar) { Value = gameCode },
                new NpgsqlParameter("@is_public", NpgsqlDbType.Boolean) { Value = isPublic },
                new NpgsqlParameter("@player1_id", NpgsqlDbType.Integer) { Value = player1Id },
                new NpgsqlParameter("@player2_id", NpgsqlDbType.Integer) { Value = player2Id },
                new NpgsqlParameter("@winner_id", NpgsqlDbType.Integer) { Value = winnerId },
                new NpgsqlParameter("@game_started_at", NpgsqlDbType.Timestamp) { Value = gameStartedAt },
                new NpgsqlParameter("@game_ended_at", NpgsqlDbType.Timestamp) { Value = gameEndedAt }
            });

            player1Id += 2;
            player2Id += 2;

            await command.ExecuteNonQueryAsync();
        }
    }

    static string GenerateString(int length)
    {
        const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        StringBuilder str = new StringBuilder();
        Random random = new Random();

        for (int i = 0; i < length; i++)
        {
            int index = random.Next(validChars.Length);
            str.Append(validChars[index]);
        }

        return str.ToString();
    }

    static string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = sha256.ComputeHash(bytes);
            StringBuilder hashBuilder = new StringBuilder();

            foreach (byte b in hashBytes)
            {
                hashBuilder.Append(b.ToString("x2"));
            }

            return hashBuilder.ToString();
        }
    }
}
