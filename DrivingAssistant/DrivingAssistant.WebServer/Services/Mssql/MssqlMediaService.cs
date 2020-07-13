using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Dataset.DrivingAssistantTableAdapters;
using DrivingAssistant.WebServer.Services.Generic;

namespace DrivingAssistant.WebServer.Services.Mssql
{
    public class MssqlMediaService : MediaService
    {
        private readonly Dataset.DrivingAssistant _dataset = new Dataset.DrivingAssistant();
        private readonly string _connectionString;

        //============================================================
        public MssqlMediaService(string connectionString)
        {
            _connectionString = connectionString;
        }

        //============================================================
        public override async Task<ICollection<Media>> GetAsync()
        {
            return await Task.Run(() =>
            {
                using var tableAdapter = new Get_MediaTableAdapter()
                {
                    Connection = new SqlConnection(_connectionString)
                };
                tableAdapter.Fill(_dataset.Get_Media);
                using DataTable dataTable = _dataset.Get_Media;
                var result = from DataRow row in dataTable.AsEnumerable()
                    select new Media(row["Type"].ToString(), row["Filepath"].ToString(), row["Source"].ToString(),
                        row["Description"].ToString(), Convert.ToDateTime(row["DateAdded"]), Convert.ToInt64(row["Id"]),
                        Convert.ToInt64(row["ProcessedId"]), Convert.ToInt64(row["SessionId"]),
                        Convert.ToInt64(row["UserId"]));
                return result.ToList();
            });
        }

        //============================================================
        public override async Task<Media> GetByIdAsync(long id)
        {
            return await Task.Run(() =>
            {
                using var tableAdapter = new Get_Media_By_IdTableAdapter()
                {
                    Connection = new SqlConnection(_connectionString)
                };
                tableAdapter.Fill(_dataset.Get_Media_By_Id, id);
                using DataTable dataTable = _dataset.Get_Media_By_Id;
                var result = from DataRow row in dataTable.AsEnumerable()
                    select new Media(row["Type"].ToString(), row["Filepath"].ToString(), row["Source"].ToString(),
                        row["Description"].ToString(), Convert.ToDateTime(row["DateAdded"]), Convert.ToInt64(row["Id"]),
                        Convert.ToInt64(row["ProcessedId"]), Convert.ToInt64(row["SessionId"]),
                        Convert.ToInt64(row["UserId"]));
                return result.First();
            });
        }

        //============================================================
        public override async Task<long> SetAsync(Media media)
        {
            return await Task.Run(() =>
            {
                using var tableAdapter = new Set_MediaTableAdapter()
                {
                    Connection = new SqlConnection(_connectionString)
                };
                long? idOut = 0;
                tableAdapter.Fill(_dataset.Set_Media, null, media.ProcessedId, media.SessionId, media.UserId,
                    media.Type.ToString(),
                    media.Filepath, media.Source, media.Description, media.DateAdded, ref idOut);
                return idOut.Value;
            });
        }

        //============================================================
        public override async Task UpdateAsync(Media media)
        {
            await Task.Run(() =>
            {
                using var tableAdapter = new Set_MediaTableAdapter()
                {
                    Connection = new SqlConnection(_connectionString)
                };
                long? idOut = 0;
                tableAdapter.Fill(_dataset.Set_Media, media.Id, media.ProcessedId, media.SessionId, media.UserId,
                    media.Type.ToString(),
                    media.Filepath, media.Source, media.Description, media.DateAdded, ref idOut);
            });
        }

        //============================================================
        public override async Task DeleteAsync(Media media)
        {
            await Task.Run(() =>
            {
                using var tableAdapter = new Delete_MediaTableAdapter()
                {
                    Connection = new SqlConnection(_connectionString)
                };
                tableAdapter.Fill(_dataset.Delete_Media, media.Id);
            });
        }

        //============================================================
        public override void Dispose()
        {
            _dataset.Dispose();
        }
    }
}
