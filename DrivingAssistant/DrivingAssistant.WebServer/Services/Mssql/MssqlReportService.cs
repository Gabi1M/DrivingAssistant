using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Dataset.DrivingAssistantTableAdapters;
using DrivingAssistant.WebServer.Services.Generic;
using DrivingAssistant.WebServer.Tools;

namespace DrivingAssistant.WebServer.Services.Mssql
{
    public class MssqlReportService : IReportService
    {
        private readonly Dataset.DrivingAssistant _dataset = new Dataset.DrivingAssistant();
        private readonly ReportTableAdapter _tableAdapter = new ReportTableAdapter();

        //============================================================
        public MssqlReportService()
        {
            _tableAdapter.Connection = new SqlConnection(Constants.ServerConstants.GetMssqlConnectionString());
        }

        //============================================================
        public async Task<IEnumerable<Report>> GetAsync()
        {
            return await Task.Run(() =>
            {
                _tableAdapter.Fill(_dataset.Report);
                return _dataset.Report.AsEnumerable()!.Select(row => new Report
                {
                    Id = row.Id,
                    VideoId = row.VideoId,
                    ProcessedFrames = row.ProcessedFrames,
                    SuccessFrames = row.SuccessFrames,
                    FailFrames = row.FailFrames,
                    SuccessRate = row.SuccessRate,
                    LeftSidePercent = row.LeftSidePercent,
                    RightSidePercent = row.RightSidePercent,
                    LeftSideLineLength = row.LeftSideLineLength,
                    RightSideLineLength = row.RightSideLineLength,
                    SpanLineAngle = row.SpanLineAngle,
                    SpanLineLength = row.SpanLineLength,
                    LeftSideLineNumber = row.LeftSideLineNumber,
                    RightSideLineNumber = row.RightSideLineNumber
                });
            });
        }

        //============================================================
        public async Task<Report> GetById(long id)
        {
            return await Task.Run(() =>
            {
                using var tableAdapter = new Get_Reports_By_IdTableAdapter();
                tableAdapter.Fill(_dataset.Get_Reports_By_Id, id);
                return _dataset.Get_Reports_By_Id.AsEnumerable()!.Select(row => new Report
                {
                    Id = row.Id,
                    VideoId = row.VideoId,
                    ProcessedFrames = row.ProcessedFrames,
                    SuccessFrames = row.SuccessFrames,
                    FailFrames = row.FailFrames,
                    SuccessRate = row.SuccessRate,
                    LeftSidePercent = row.LeftSidePercent,
                    RightSidePercent = row.RightSidePercent,
                    LeftSideLineLength = row.LeftSideLineLength,
                    RightSideLineLength = row.RightSideLineLength,
                    SpanLineAngle = row.SpanLineAngle,
                    SpanLineLength = row.SpanLineLength,
                    LeftSideLineNumber = row.LeftSideLineNumber,
                    RightSideLineNumber = row.RightSideLineNumber
                }).First();
            });
        }

        //============================================================
        public async Task<Report> GetByVideo(long videoId)
        {
            return await Task.Run(() =>
            {
                using var tableAdapter = new Get_Reports_By_VideoTableAdapter();
                tableAdapter.Fill(_dataset.Get_Reports_By_Video, videoId);
                return _dataset.Get_Reports_By_Video.AsEnumerable()!.Select(row => new Report
                {
                    Id = row.Id,
                    VideoId = row.VideoId,
                    ProcessedFrames = row.ProcessedFrames,
                    SuccessFrames = row.SuccessFrames,
                    FailFrames = row.FailFrames,
                    SuccessRate = row.SuccessRate,
                    LeftSidePercent = row.LeftSidePercent,
                    RightSidePercent = row.RightSidePercent,
                    LeftSideLineLength = row.LeftSideLineLength,
                    RightSideLineLength = row.RightSideLineLength,
                    SpanLineAngle = row.SpanLineAngle,
                    SpanLineLength = row.SpanLineLength,
                    LeftSideLineNumber = row.LeftSideLineNumber,
                    RightSideLineNumber = row.RightSideLineNumber
                }).First();
            });
        }

        //============================================================
        public async Task<IEnumerable<Report>> GetBySession(long sessionId)
        {
            return await Task.Run(() =>
            {
                using var tableAdapter = new Get_Reports_By_SessionTableAdapter();
                tableAdapter.Fill(_dataset.Get_Reports_By_Session, sessionId);
                return _dataset.Get_Reports_By_Session.AsEnumerable()!.Select(row => new Report
                {
                    Id = row.Id,
                    VideoId = row.VideoId,
                    ProcessedFrames = row.ProcessedFrames,
                    SuccessFrames = row.SuccessFrames,
                    FailFrames = row.FailFrames,
                    SuccessRate = row.SuccessRate,
                    LeftSidePercent = row.LeftSidePercent,
                    RightSidePercent = row.RightSidePercent,
                    LeftSideLineLength = row.LeftSideLineLength,
                    RightSideLineLength = row.RightSideLineLength,
                    SpanLineAngle = row.SpanLineAngle,
                    SpanLineLength = row.SpanLineLength,
                    LeftSideLineNumber = row.LeftSideLineNumber,
                    RightSideLineNumber = row.RightSideLineNumber
                });
            });
        }

        //============================================================
        public async Task<IEnumerable<Report>> GetByUser(long userId)
        {
            return await Task.Run(() =>
            {
                using var tableAdapter = new Get_Reports_By_UserTableAdapter();
                tableAdapter.Fill(_dataset.Get_Reports_By_User, userId);
                return _dataset.Get_Reports_By_User.AsEnumerable()!.Select(row => new Report
                {
                    Id = row.Id,
                    VideoId = row.VideoId,
                    ProcessedFrames = row.ProcessedFrames,
                    SuccessFrames = row.SuccessFrames,
                    FailFrames = row.FailFrames,
                    SuccessRate = row.SuccessRate,
                    LeftSidePercent = row.LeftSidePercent,
                    RightSidePercent = row.RightSidePercent,
                    LeftSideLineLength = row.LeftSideLineLength,
                    RightSideLineLength = row.RightSideLineLength,
                    SpanLineAngle = row.SpanLineAngle,
                    SpanLineLength = row.SpanLineLength,
                    LeftSideLineNumber = row.LeftSideLineNumber,
                    RightSideLineNumber = row.RightSideLineNumber
                });
            });
        }

        //============================================================
        public async Task<long> SetAsync(Report report)
        {
            return await Task.Run(() =>
            {
                long? idOut = 0;
                _tableAdapter.Insert(report.Id, report.VideoId, report.ProcessedFrames, report.SuccessFrames, report.FailFrames, report.SuccessRate, report.LeftSidePercent, report.RightSidePercent,
                    report.LeftSideLineLength, report.RightSideLineLength, report.SpanLineAngle, report.SpanLineLength,
                    report.LeftSideLineNumber, report.RightSideLineNumber, ref idOut);
                return idOut ?? -1;
            });
        }

        //============================================================
        public async Task DeleteAsync(Report data)
        {
            await Task.Run(() =>
            {
                _tableAdapter.Delete(data.Id);
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
