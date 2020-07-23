using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Models.ImageProcessing;
using DrivingAssistant.WebServer.Dataset.DrivingAssistantTableAdapters;
using DrivingAssistant.WebServer.Services.Generic;

namespace DrivingAssistant.WebServer.Services.Mssql
{
    public class MssqlUserSettingsService : IUserSettingsService
    {
        private readonly Dataset.DrivingAssistant _dataset = new Dataset.DrivingAssistant();
        private readonly UserSettingsTableAdapter _tableAdapter = new UserSettingsTableAdapter();

        //============================================================
        public MssqlUserSettingsService(string connectionString)
        {
            _tableAdapter.Connection = new SqlConnection(connectionString);
        }

        //============================================================
        public async Task<ICollection<UserSettings>> GetAsync()
        {
            return await Task.Run(() =>
            {
                _tableAdapter.Fill(_dataset.UserSettings);
                return _dataset.UserSettings.AsEnumerable().Select(row => new UserSettings
                {
                    Id = row.Id,
                    UserId = row.UserID,
                    Parameters = new Parameters
                    {
                        CannyThreshold = row.CannyThreshold,
                        CannyThresholdLinking = row.CannyThresholdLinking,
                        HoughLinesRhoResolution = row.HoughLinesRhoResolution,
                        HoughLinesThetaResolution = row.HoughLinesThetaResolution,
                        HoughLinesMinimumLineWidth = row.HoughLinesMinimumLineWidth,
                        HoughLinesGapBetweenLines = row.HoughLinesGapBetweenLines,
                        HoughLinesThreshold = row.HoughLinesThreshold,
                        DilateIterations = row.DilateIterations
                    }
                }).ToList();
            });
        }

        //============================================================
        public async Task<long> SetAsync(UserSettings userSettings)
        {
            return await Task.Run(() =>
            {
                long? idOut = 0;
                _tableAdapter.Insert(userSettings.Id, userSettings.UserId, userSettings.Parameters.CannyThreshold,
                    userSettings.Parameters.CannyThresholdLinking, userSettings.Parameters.HoughLinesRhoResolution,
                    userSettings.Parameters.HoughLinesThetaResolution,
                    userSettings.Parameters.HoughLinesMinimumLineWidth,
                    userSettings.Parameters.HoughLinesGapBetweenLines, userSettings.Parameters.HoughLinesThreshold,
                    userSettings.Parameters.DilateIterations, ref idOut);
                return idOut ?? -1;
            });
        }

        //============================================================
        public async Task DeleteAsync(UserSettings userSettings)
        {
            await Task.Run(() =>
            {
                _tableAdapter.Delete(userSettings.Id);
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
