using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Tools;
using DrivingAssistant.WebServer.Services.Generic;
using DrivingAssistant.WebServer.Tools;
using Npgsql;

namespace DrivingAssistant.WebServer.Services.Psql
{
    public class PsqlMediaService : MediaService
    {
        private readonly NpgsqlConnection _connection;

        //============================================================
        public PsqlMediaService(string connectionString)
        {
            _connection = new NpgsqlConnection(connectionString);
        }

        //============================================================
        public override async Task<ICollection<Media>> GetAsync()
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.PsqlDatabaseConstants.GetMediaCommand, _connection);
            var result = await command.ExecuteReaderAsync();
            var media = new List<Media>();
            while (await result.ReadAsync())
            {
                media.Add(new Media(result["type"].ToString(), 
                    result["filepath"].ToString(),
                    result["source"].ToString(), 
                    result["description"].ToString(), 
                    Convert.ToDateTime(result["date_added"]), 
                    Convert.ToInt64(result["id"]),
                    Convert.ToInt64(result["processed_id"]), 
                    Convert.ToInt64(result["session_id"]), 
                    Convert.ToInt64(result["user_id"])));
            }

            await _connection.CloseAsync();
            return media;
        }

        //============================================================
        public override async Task<Media> GetByIdAsync(long id)
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.PsqlDatabaseConstants.GetMediaByIdCommand, _connection);
            command.Parameters.AddWithValue("id", id);
            var result = await command.ExecuteReaderAsync();
            await result.ReadAsync();
            var media = new Media(result["type"].ToString(), 
                result["filepath"].ToString(),
                result["source"].ToString(), 
                result["description"].ToString(), 
                Convert.ToDateTime(result["date_added"]), 
                Convert.ToInt64(result["id"]),
                Convert.ToInt64(result["processed_id"]), 
                Convert.ToInt64(result["session_id"]), 
                Convert.ToInt64(result["user_id"]));
            await _connection.CloseAsync();
            return media;
        }

        //============================================================
        public override async Task<long> SetAsync(Media media)
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.PsqlDatabaseConstants.AddMediaCommand, _connection);
            command.Parameters.AddWithValue("type", media.Type.ToString());
            command.Parameters.AddWithValue("processed_id", media.ProcessedId);
            command.Parameters.AddWithValue("session_id", media.SessionId);
            command.Parameters.AddWithValue("user_id", media.UserId);
            command.Parameters.AddWithValue("filepath", media.Filepath);
            command.Parameters.AddWithValue("source", media.Source);
            command.Parameters.AddWithValue("description", media.Description);
            command.Parameters.AddWithValue("date_added", media.DateAdded);
            var result = Convert.ToInt64(await command.ExecuteScalarAsync());
            await _connection.CloseAsync();
            return result;
        }

        //============================================================
        public override async Task UpdateAsync(Media media)
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.PsqlDatabaseConstants.UpdateMediaCommand, _connection);
            command.Parameters.AddWithValue("id", media.Id);
            command.Parameters.AddWithValue("type", media.Type.ToString());
            command.Parameters.AddWithValue("processed_id", media.ProcessedId);
            command.Parameters.AddWithValue("session_id", media.SessionId);
            command.Parameters.AddWithValue("user_id", media.UserId);
            command.Parameters.AddWithValue("filepath", media.Filepath);
            command.Parameters.AddWithValue("source", media.Source);
            command.Parameters.AddWithValue("description", media.Description);
            command.Parameters.AddWithValue("date_added", media.DateAdded);
            await command.ExecuteNonQueryAsync();
            await _connection.CloseAsync();
        }

        //============================================================
        public override async Task DeleteAsync(Media media)
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.PsqlDatabaseConstants.DeleteMediaCommand, _connection);
            command.Parameters.AddWithValue("id", media.Id);
            await command.ExecuteNonQueryAsync();
            await _connection.CloseAsync();
            if (File.Exists(media.Filepath))
            {
                var count = 0;
                while (count < 5)
                {
                    try
                    {
                        ++count;
                        File.Delete(media.Filepath);
                        break;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                    }
                }
            }
        }

        //============================================================
        public override async void Dispose()
        {
            await _connection.DisposeAsync();
        }
    }
}
