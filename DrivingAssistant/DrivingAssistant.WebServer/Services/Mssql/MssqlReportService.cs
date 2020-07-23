using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models;
using DrivingAssistant.WebServer.Dataset.DrivingAssistantTableAdapters;
using DrivingAssistant.WebServer.Services.Generic;

namespace DrivingAssistant.WebServer.Services.Mssql
{
    public class MssqlReportService : IReportService
    {
        private readonly Dataset.DrivingAssistant _dataset = new Dataset.DrivingAssistant();
        private readonly ReportTableAdapter _tableAdapter = new ReportTableAdapter();

        //============================================================
        public MssqlReportService(string connectionString)
        {
            _tableAdapter.Connection = new SqlConnection(connectionString);
        }

        //============================================================
        public async Task<ICollection<Report>> GetAsync()
        {
            return await Task.Run(() =>
            {
                _tableAdapter.Fill(_dataset.Report);
                return _dataset.Report.AsEnumerable()!.Select(row => new Report
                {
                    Id = row.Id,
                    MediaId = row.MediaId,
                    SessionId = row.SessionId,
                    LeftSidePercent = row.LeftSidePercent,
                    RightSidePercent = row.RightSidePercent,
                    LeftSideLineLength = row.LeftSideLineLength,
                    RightSideLineLength = row.RightSideLineLength,
                    SpanLineAngle = row.SpanLineAngle,
                    SpanLineLength = row.SpanLineLength,
                    LeftSideLineNumber = row.LeftSideLineNumber,
                    RightSideLineNumber = row.RightSideLineNumber
                }).ToList();
            });
        }

        //============================================================
        public async Task<long> SetAsync(Report report)
        {
            return await Task.Run(() =>
            {
                long? idOut = 0;
                _tableAdapter.Insert(report.Id, report.MediaId, report.SessionId, report.LeftSidePercent, report.RightSidePercent,
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
