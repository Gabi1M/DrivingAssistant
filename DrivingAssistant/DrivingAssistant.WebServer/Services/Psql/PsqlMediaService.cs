using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Tools;
using DrivingAssistant.WebServer.Services.Generic;
using DrivingAssistant.WebServer.Tools;
using Npgsql;

namespace DrivingAssistant.WebServer.Services.Psql
{
    public class PsqlMediaService : IMediaService
    {
        private readonly NpgsqlConnection _connection;

        //============================================================
        public PsqlMediaService(string connectionString)
        {
            _connection = new NpgsqlConnection(connectionString);
        }

        //============================================================
        public async Task<ICollection<Media>> GetAsync()
        {
            await _connection.OpenAsync();
            await using var command = new NpgsqlCommand(Constants.PsqlDatabaseConstants.GetMediaCommand, _connection);
            var result = await command.ExecuteReaderAsync();
            var media = new List<Media>();
            while (await result.ReadAsync())
            {
                media.Add(new Media
                {
                    Id = Convert.ToInt64(result["id"]),
                    ProcessedId = Convert.ToInt64(result["processed_id"]),
                    SessionId = Convert.ToInt64(result["session_id"]),
                    UserId = Convert.ToInt64(result["user_id"]),
                    Type = (MediaType) Enum.Parse(typeof(MediaType), result["type"].ToString()!),
                    Filepath = result["filepath"].ToString(),
                    Source = result["source"].ToString(),
                    Description = result["description"].ToString(),
                    DateAdded = Convert.ToDateTime(result["date_added"])
                });
            }

            await _connection.CloseAsync();
            return media;
        }

        //============================================================
        public async Task<long> SetAsync(Media media)
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
        public async Task DeleteAsync(Media media)
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
        public async void Dispose()
        {
            await _connection.DisposeAsync();
        }
    }
}
