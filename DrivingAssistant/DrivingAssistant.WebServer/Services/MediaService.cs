using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Tools;
using Npgsql;

namespace DrivingAssistant.WebServer.Services
{
    public class MediaService : GenericService<Media>
    {
        private readonly NpgsqlConnection _connection;

        //============================================================
        public MediaService(string connectionString)
        {
            _connection = new NpgsqlConnection(connectionString);
        }

        //============================================================
        public override async Task<ICollection<Media>> GetAsync()
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.DatabaseConstants.GetMediaCommand, _connection);
            var result = await command.ExecuteReaderAsync();
            var media = new List<Media>();
            while (await result.ReadAsync())
            {
                media.Add(new Media(result["type"].ToString(), result["filepath"].ToString(),
                    result["source"].ToString(), Convert.ToDateTime(result["datetime"]), Convert.ToInt64(result["id"]),
                    Convert.ToInt64(result["processed_id"]), Convert.ToInt64(result["session_id"])));
            }

            await _connection.CloseAsync();
            return media;
        }

        //============================================================
        public override async Task<long> SetAsync(Media media)
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.DatabaseConstants.AddMediaCommand, _connection);
            command.Parameters.AddWithValue("type", media.Type.ToString());
            command.Parameters.AddWithValue("processed_id", media.ProcessedId);
            command.Parameters.AddWithValue("session_id", media.SessionId);
            command.Parameters.AddWithValue("filepath", media.Filepath);
            command.Parameters.AddWithValue("source", media.Source);
            command.Parameters.AddWithValue("datetime", media.DateAdded);
            var result = Convert.ToInt64(await command.ExecuteScalarAsync());
            await _connection.CloseAsync();
            return result;
        }

        //============================================================
        public override async Task UpdateAsync(Media media)
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.DatabaseConstants.UpdateMediaCommand, _connection);
            command.Parameters.AddWithValue("id", media.Id);
            command.Parameters.AddWithValue("type", media.Type.ToString());
            command.Parameters.AddWithValue("processed_id", media.ProcessedId);
            command.Parameters.AddWithValue("session_id", media.SessionId);
            command.Parameters.AddWithValue("filepath", media.Filepath);
            command.Parameters.AddWithValue("source", media.Source);
            command.Parameters.AddWithValue("datetime", media.DateAdded);
            await command.ExecuteNonQueryAsync();
            await _connection.CloseAsync();
        }

        //============================================================
        public override async Task DeleteAsync(long id)
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.DatabaseConstants.DeleteMediaCommand, _connection);
            command.Parameters.AddWithValue("id", id);
            await command.ExecuteNonQueryAsync();
            await _connection.CloseAsync();
        }

        //============================================================
        public override async void Dispose()
        {
            await _connection.DisposeAsync();
        }
    }
}
