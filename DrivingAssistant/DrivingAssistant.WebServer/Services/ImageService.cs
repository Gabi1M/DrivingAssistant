using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Tools;
using Npgsql;

namespace DrivingAssistant.WebServer.Services
{
    public class ImageService : IDisposable
    {
        private readonly NpgsqlConnection _connection;

        //============================================================
        public ImageService(string connectionString)
        {
            _connection = new NpgsqlConnection(connectionString);
            _connection.Open();
        }

        //============================================================
        public async Task<ICollection<Image>> GetAsync()
        {
            await using var command = new NpgsqlCommand(Constants.DatabaseConstants.GetImagesCommand, _connection);
            var result = await command.ExecuteReaderAsync();
            var images = new List<Image>();
            while (await result.ReadAsync())
            {
                images.Add(new Image(result["filepath"].ToString(),
                    Convert.ToInt32(result["width"]),
                    Convert.ToInt32(result["height"]),
                    result["format"].ToString(),
                    result["source"].ToString(),
                    Convert.ToDateTime(result["datetime"]),
                    Convert.ToInt64(result["id"])));
            }

            return images;
        }

        //============================================================
        public async Task<long> SetAsync(Image image)
        {
            await using var command = new NpgsqlCommand(Constants.DatabaseConstants.AddImageCommand, _connection);
            command.Parameters.AddWithValue("filepath", image.Filepath);
            command.Parameters.AddWithValue("width", image.Width);
            command.Parameters.AddWithValue("height", image.Height);
            command.Parameters.AddWithValue("format", image.Format);
            command.Parameters.AddWithValue("source", image.Source);
            command.Parameters.AddWithValue("datetime", DateTime.Now);
            var result = Convert.ToInt64(await command.ExecuteScalarAsync());
            return result;
        }

        //============================================================
        public async Task DeleteAsync(long id)
        {
            await using var command = new NpgsqlCommand(Constants.DatabaseConstants.DeleteImageCommand, _connection);
            command.Parameters.AddWithValue("id", id);
            await command.ExecuteNonQueryAsync();
        }

        //============================================================
        public async void Dispose()
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }
    }
}
