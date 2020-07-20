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
    public class MssqlUserSettingsService : UserSettingsService
    {
        private readonly Dataset.DrivingAssistant _dataset = new Dataset.DrivingAssistant();
        private readonly string _connectionString;

        //============================================================
        public MssqlUserSettingsService(string connectionString)
        {
            _connectionString = connectionString;
        }

        //============================================================
        public override async Task<ICollection<UserSettings>> GetAsync()
        {
            return await Task.Run(() =>
            {
                using var tableAdapter = new Get_UserSettingsTableAdapter
                {
                    Connection = new SqlConnection(_connectionString)
                };
                tableAdapter.Fill(_dataset.Get_UserSettings);
                using var dataTable = _dataset.Get_UserSettings as DataTable;
                var result = from DataRow row in dataTable.AsEnumerable()
                    select new UserSettings
                    {
                        Id = Convert.ToInt64(row["Id"]),
                        UserId = Convert.ToInt64(row["UserId"]),
                        ImageProcessorParameters = new ImageProcessorParameters
                        {
                            CannyThreshold = Convert.ToDouble(row["CannyThreshold"]),
                            CannyThresholdLinking = Convert.ToDouble(row["CannyThresholdLinking"]),
                            HoughLinesRhoResolution = Convert.ToDouble(row["HoughLinesRhoResolution"]),
                            HoughLinesThetaResolution = Convert.ToDouble(row["HoughLinesThetaResolution"]),
                            HoughLinesMinimumLineWidth = Convert.ToDouble(row["HoughLinesMinimumLineWidth"]),
                            HoughLinesGapBetweenLines = Convert.ToDouble(row["HoughLinesGapBetweenLines"]),
                            HoughLinesThreshold = Convert.ToInt32(row["HoughLinesThreshold"]),
                            DilateIterations = Convert.ToInt32(row["DilateIterations"])
                        }
                    };
                return result.ToList();
            });
        }

        //============================================================
        public override async Task<UserSettings> GetByIdAsync(long id)
        {
            return await Task.Run(() =>
            {
                using var tableAdapter = new Get_UserSettings_By_IdTableAdapter
                {
                    Connection = new SqlConnection(_connectionString)
                };
                tableAdapter.Fill(_dataset.Get_UserSettings_By_Id, id);
                using var dataTable = _dataset.Get_UserSettings_By_Id as DataTable;
                var result = from DataRow row in dataTable.AsEnumerable()
                    select new UserSettings
                    {
                        Id = Convert.ToInt64(row["Id"]),
                        UserId = Convert.ToInt64(row["UserId"]),
                        ImageProcessorParameters = new ImageProcessorParameters
                        {
                            CannyThreshold = Convert.ToDouble(row["CannyThreshold"]),
                            CannyThresholdLinking = Convert.ToDouble(row["CannyThresholdLinking"]),
                            HoughLinesRhoResolution = Convert.ToDouble(row["HoughLinesRhoResolution"]),
                            HoughLinesThetaResolution = Convert.ToDouble(row["HoughLinesThetaResolution"]),
                            HoughLinesMinimumLineWidth = Convert.ToDouble(row["HoughLinesMinimumLineWidth"]),
                            HoughLinesGapBetweenLines = Convert.ToDouble(row["HoughLinesGapBetweenLines"]),
                            HoughLinesThreshold = Convert.ToInt32(row["HoughLinesThreshold"]),
                            DilateIterations = Convert.ToInt32(row["DilateIterations"])
                        }
                    };
                return result.First();
            });
        }

        //============================================================
        public override async Task<long> SetAsync(UserSettings userSettings)
        {
            return await Task.Run(() =>
            {
                using var tableAdapter = new Set_UserSettingsTableAdapter
                {
                    Connection = new SqlConnection(_connectionString)
                };
                long? IdOut = 0;
                tableAdapter.Fill(_dataset.Set_UserSettings, null, userSettings.UserId,
                    userSettings.ImageProcessorParameters.CannyThreshold,
                    userSettings.ImageProcessorParameters.CannyThresholdLinking,
                    userSettings.ImageProcessorParameters.HoughLinesRhoResolution,
                    userSettings.ImageProcessorParameters.HoughLinesThetaResolution,
                    userSettings.ImageProcessorParameters.HoughLinesMinimumLineWidth,
                    userSettings.ImageProcessorParameters.HoughLinesGapBetweenLines,
                    userSettings.ImageProcessorParameters.HoughLinesThreshold,
                    userSettings.ImageProcessorParameters.DilateIterations, ref IdOut);
                return IdOut.Value;
            });
        }

        //============================================================
        public override async Task UpdateAsync(UserSettings userSettings)
        {
            await Task.Run(() =>
            {
                using var tableAdapter = new Set_UserSettingsTableAdapter
                {
                    Connection = new SqlConnection(_connectionString)
                };
                long? IdOut = 0;
                tableAdapter.Fill(_dataset.Set_UserSettings, userSettings.Id, userSettings.UserId,
                    userSettings.ImageProcessorParameters.CannyThreshold,
                    userSettings.ImageProcessorParameters.CannyThresholdLinking,
                    userSettings.ImageProcessorParameters.HoughLinesRhoResolution,
                    userSettings.ImageProcessorParameters.HoughLinesThetaResolution,
                    userSettings.ImageProcessorParameters.HoughLinesMinimumLineWidth,
                    userSettings.ImageProcessorParameters.HoughLinesGapBetweenLines,
                    userSettings.ImageProcessorParameters.HoughLinesThreshold,
                    userSettings.ImageProcessorParameters.DilateIterations, ref IdOut);
            });
        }

        //============================================================
        public override async Task DeleteAsync(UserSettings userSettings)
        {
            await Task.Run(() =>
            {
                using var tableAdapter = new Delete_UserSettingsTableAdapter
                {
                    Connection = new SqlConnection(_connectionString)
                };
                tableAdapter.Fill(_dataset.Delete_UserSettings, userSettings.Id);
            });
        }

        //============================================================
        public override void Dispose()
        {
            _dataset.Dispose();
        }
    }
}
