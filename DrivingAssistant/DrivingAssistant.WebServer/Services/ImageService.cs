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
        }

        //============================================================
        public async Task<ICollection<Image>> GetAsync()
        {
            await _connection.OpenAsync();
            var command = new NpgsqlCommand(Constants.DatabaseConstants.GetImagesCommand, _connection);
            var result = await command.ExecuteReaderAsync();
            var images = new List<Image>();
            while (await result.ReadAsync())
            {
                images.Add(new Image(result["filepath"].ToString(),
                    Convert.ToInt32(result["width"]),
                    Convert.ToInt32(result["height"]),
                    result["format"].ToString(),
                    result["source"].ToString(),
                    Convert.ToDateTime(result["datetime"].ToString()),
                    Convert.ToInt64(result["id"])));
            }

            await _connection.CloseAsync();
            return images;
        }

        //============================================================
        public async Task<long> SetAsync(Image image)
        {
            await _connection.OpenAsync();
            var command = new NpgsqlCommand(Constants.DatabaseConstants.AddImageCommand, _connection);
            command.Parameters.AddWithValue("filepath", image.Filepath);
            command.Parameters.AddWithValue("width", image.Width);
            command.Parameters.AddWithValue("height", image.Height);
            command.Parameters.AddWithValue("format", image.Format);
            command.Parameters.AddWithValue("source", image.Source);
            command.Parameters.AddWithValue("datetime", DateTime.Now);
            var result = Convert.ToInt64(await command.ExecuteScalarAsync());
            await _connection.CloseAsync();
            return result;
        }

        //============================================================
        public async Task DeleteAsync(long id)
        {
            await _connection.OpenAsync();
            var command = new NpgsqlCommand(Constants.DatabaseConstants.DeleteImageCommand, _connection);
            command.Parameters.AddWithValue("id", id);
            await command.ExecuteNonQueryAsync();
            await _connection.CloseAsync();
        }

        //============================================================
        public async void Dispose()
        {
            await _connection.DisposeAsync();
        }
    }
}
