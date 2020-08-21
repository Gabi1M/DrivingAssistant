using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Dataset.DrivingAssistantTableAdapters;
using DrivingAssistant.WebServer.Services.Generic;
using DrivingAssistant.WebServer.Tools;

namespace DrivingAssistant.WebServer.Services.Mssql
{
    public class MssqlThumbnailService : IThumbnailService
    {
        private readonly Dataset.DrivingAssistant _dataset = new Dataset.DrivingAssistant();
        private readonly ThumbnailTableAdapter _tableAdapter = new ThumbnailTableAdapter();

        //============================================================
        public MssqlThumbnailService()
        {
            _tableAdapter.Connection = new SqlConnection(Constants.ServerConstants.GetMssqlConnectionString());
        }

        //============================================================
        public async Task<IEnumerable<Thumbnail>> GetAsync()
        {
            return await Task.Run(() =>
            {
                _tableAdapter.Fill(_dataset.Thumbnail);
                return _dataset.Thumbnail.AsEnumerable().Select(row => new Thumbnail
                {
                    Id = row.Id,
                    VideoId = row.VideoId,
                    Filepath = row.Filepath
                });
            });
        }

        //============================================================
        public async Task<Thumbnail> GetById(long id)
        {
            return await Task.Run(() =>
            {
                using var tableAdapter = new Get_Thumbnail_By_IdTableAdapter();
                tableAdapter.Fill(_dataset.Get_Thumbnail_By_Id, id);
                return _dataset.Get_Thumbnail_By_Id.AsEnumerable().Select(row => new Thumbnail
                {
                    Id = row.Id,
                    VideoId = row.VideoId,
                    Filepath = row.Filepath
                }).First();
            });
        }

        //============================================================
        public async Task<Thumbnail> GetByVideo(long videoId)
        {
            return await Task.Run(() =>
            {
                using var tableAdapter = new Get_Thumbnail_By_VideoTableAdapter();
                tableAdapter.Fill(_dataset.Get_Thumbnail_By_Video, videoId);
                return _dataset.Get_Thumbnail_By_Video.AsEnumerable().Select(row => new Thumbnail
                {
                    Id = row.Id,
                    VideoId = row.VideoId,
                    Filepath = row.Filepath
                }).First();
            });
        }

        //============================================================
        public async Task<long> SetAsync(Thumbnail thumbnail)
        {
            return await Task.Run(() =>
            {
                long? idOut = 0;
                _tableAdapter.Insert(thumbnail.Id, thumbnail.VideoId, thumbnail.Filepath, ref idOut);
                return idOut ?? -1;
            });
        }

        //============================================================
        public async Task DeleteAsync(Thumbnail thumbnail)
        {
            await Task.Run(() =>
            {
                _tableAdapter.Delete(thumbnail.Id);
            });
        }

        //============================================================
        public void Dispose()
        {
            _tableAdapter.Dispose();
            _dataset.Dispose();
        }
    }
}
