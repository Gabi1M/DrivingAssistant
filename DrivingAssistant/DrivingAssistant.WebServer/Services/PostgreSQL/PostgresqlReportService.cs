using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DrivingAssistant.Core.Models.Reports;
using DrivingAssistant.WebServer.Services.Generic;
using DrivingAssistant.WebServer.Tools;
using Npgsql;

namespace DrivingAssistant.WebServer.Services.PostgreSQL
{
    public class PostgresqlReportService : IReportService
    {

        //============================================================
        public async Task<IEnumerable<LaneDepartureWarningReport>> GetAsync()
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.GetAllReports, connection);
                var result = await command.ExecuteReaderAsync();
                var reports = new List<LaneDepartureWarningReport>();
                while (await result.ReadAsync())
                {
                    reports.Add(new LaneDepartureWarningReport
                    {
                        Id = Convert.ToInt64(result["id"]),
                        VideoId = Convert.ToInt64(result["video_id"]),
                        ProcessedFrames = Convert.ToInt64(result["processed_frames"]),
                        SuccessFrames = Convert.ToInt64(result["success_frames"]),
                        FailFrames = Convert.ToInt64(result["fail_frames"]),
                        SuccessRate = Convert.ToDouble(result["success_rate"]),
                        LeftSidePercent = Convert.ToDouble(result["left_side_percent"]),
                        RightSidePercent = Convert.ToDouble(result["right_side_percent"]),
                        LeftSideLineLength = Convert.ToDouble(result["left_side_line_length"]),
                        RightSideLineLength = Convert.ToDouble(result["right_side_line_length"]),
                        SpanLineAngle = Convert.ToDouble(result["span_line_angle"]),
                        SpanLineLength = Convert.ToDouble(result["span_line_length"]),
                        LeftSideLineNumber = Convert.ToInt32(result["left_side_line_number"]),
                        RightSideLineNumber = Convert.ToInt32(result["right_side_line_number"])
                    });
                }

                return reports;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        //============================================================
        public async Task<LaneDepartureWarningReport> GetById(long id)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.GetReportById, connection);
                command.Parameters.AddWithValue("id", id);
                var result = await command.ExecuteReaderAsync();
                var reports = new List<LaneDepartureWarningReport>();
                while (await result.ReadAsync())
                {
                    reports.Add(new LaneDepartureWarningReport
                    {
                        Id = Convert.ToInt64(result["id"]),
                        VideoId = Convert.ToInt64(result["video_id"]),
                        ProcessedFrames = Convert.ToInt64(result["processed_frames"]),
                        SuccessFrames = Convert.ToInt64(result["success_frames"]),
                        FailFrames = Convert.ToInt64(result["fail_frames"]),
                        SuccessRate = Convert.ToDouble(result["success_rate"]),
                        LeftSidePercent = Convert.ToDouble(result["left_side_percent"]),
                        RightSidePercent = Convert.ToDouble(result["right_side_percent"]),
                        LeftSideLineLength = Convert.ToDouble(result["left_side_line_length"]),
                        RightSideLineLength = Convert.ToDouble(result["right_side_line_length"]),
                        SpanLineAngle = Convert.ToDouble(result["span_line_angle"]),
                        SpanLineLength = Convert.ToDouble(result["span_line_length"]),
                        LeftSideLineNumber = Convert.ToInt32(result["left_side_line_number"]),
                        RightSideLineNumber = Convert.ToInt32(result["right_side_line_number"])
                    });
                }

                return reports.Single();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        //============================================================
        public async Task<LaneDepartureWarningReport> GetByVideo(long videoId)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.GetReportByVideo, connection);
                command.Parameters.AddWithValue("video_id", videoId);
                var result = await command.ExecuteReaderAsync();
                var reports = new List<LaneDepartureWarningReport>();
                while (await result.ReadAsync())
                {
                    reports.Add(new LaneDepartureWarningReport
                    {
                        Id = Convert.ToInt64(result["id"]),
                        VideoId = Convert.ToInt64(result["video_id"]),
                        ProcessedFrames = Convert.ToInt64(result["processed_frames"]),
                        SuccessFrames = Convert.ToInt64(result["success_frames"]),
                        FailFrames = Convert.ToInt64(result["fail_frames"]),
                        SuccessRate = Convert.ToDouble(result["success_rate"]),
                        LeftSidePercent = Convert.ToDouble(result["left_side_percent"]),
                        RightSidePercent = Convert.ToDouble(result["right_side_percent"]),
                        LeftSideLineLength = Convert.ToDouble(result["left_side_line_length"]),
                        RightSideLineLength = Convert.ToDouble(result["right_side_line_length"]),
                        SpanLineAngle = Convert.ToDouble(result["span_line_angle"]),
                        SpanLineLength = Convert.ToDouble(result["span_line_length"]),
                        LeftSideLineNumber = Convert.ToInt32(result["left_side_line_number"]),
                        RightSideLineNumber = Convert.ToInt32(result["right_side_line_number"])
                    });
                }

                return reports.Single();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        //============================================================
        public async Task<IEnumerable<LaneDepartureWarningReport>> GetBySession(long sessionId)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.GetReportBySession, connection);
                command.Parameters.AddWithValue("session_id", sessionId);
                var result = await command.ExecuteReaderAsync();
                var reports = new List<LaneDepartureWarningReport>();
                while (await result.ReadAsync())
                {
                    reports.Add(new LaneDepartureWarningReport
                    {
                        Id = Convert.ToInt64(result["id"]),
                        VideoId = Convert.ToInt64(result["video_id"]),
                        ProcessedFrames = Convert.ToInt64(result["processed_frames"]),
                        SuccessFrames = Convert.ToInt64(result["success_frames"]),
                        FailFrames = Convert.ToInt64(result["fail_frames"]),
                        SuccessRate = Convert.ToDouble(result["success_rate"]),
                        LeftSidePercent = Convert.ToDouble(result["left_side_percent"]),
                        RightSidePercent = Convert.ToDouble(result["right_side_percent"]),
                        LeftSideLineLength = Convert.ToDouble(result["left_side_line_length"]),
                        RightSideLineLength = Convert.ToDouble(result["right_side_line_length"]),
                        SpanLineAngle = Convert.ToDouble(result["span_line_angle"]),
                        SpanLineLength = Convert.ToDouble(result["span_line_length"]),
                        LeftSideLineNumber = Convert.ToInt32(result["left_side_line_number"]),
                        RightSideLineNumber = Convert.ToInt32(result["right_side_line_number"])
                    });
                }

                return reports;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        //============================================================
        public async Task<IEnumerable<LaneDepartureWarningReport>> GetByUser(long userId)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.GetReportByUser, connection);
                command.Parameters.AddWithValue("user_id", userId);
                var result = await command.ExecuteReaderAsync();
                var reports = new List<LaneDepartureWarningReport>();
                while (await result.ReadAsync())
                {
                    reports.Add(new LaneDepartureWarningReport
                    {
                        Id = Convert.ToInt64(result["id"]),
                        VideoId = Convert.ToInt64(result["video_id"]),
                        ProcessedFrames = Convert.ToInt64(result["processed_frames"]),
                        SuccessFrames = Convert.ToInt64(result["success_frames"]),
                        FailFrames = Convert.ToInt64(result["fail_frames"]),
                        SuccessRate = Convert.ToDouble(result["success_rate"]),
                        LeftSidePercent = Convert.ToDouble(result["left_side_percent"]),
                        RightSidePercent = Convert.ToDouble(result["right_side_percent"]),
                        LeftSideLineLength = Convert.ToDouble(result["left_side_line_length"]),
                        RightSideLineLength = Convert.ToDouble(result["right_side_line_length"]),
                        SpanLineAngle = Convert.ToDouble(result["span_line_angle"]),
                        SpanLineLength = Convert.ToDouble(result["span_line_length"]),
                        LeftSideLineNumber = Convert.ToInt32(result["left_side_line_number"]),
                        RightSideLineNumber = Convert.ToInt32(result["right_side_line_number"])
                    });
                }

                return reports;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        //============================================================
        public async Task<long> SetAsync(LaneDepartureWarningReport laneDepartureWarningReport)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                if (laneDepartureWarningReport.Id == -1)
                {
                    await using var command = new NpgsqlCommand(PostgreSQLCommands.SetReport, connection);
                    command.Parameters.AddWithValue("video_id", laneDepartureWarningReport.VideoId);
                    command.Parameters.AddWithValue("processed_frames", laneDepartureWarningReport.ProcessedFrames);
                    command.Parameters.AddWithValue("success_frames", laneDepartureWarningReport.SuccessFrames);
                    command.Parameters.AddWithValue("fail_frames", laneDepartureWarningReport.FailFrames);
                    command.Parameters.AddWithValue("success_rate", laneDepartureWarningReport.SuccessRate);
                    command.Parameters.AddWithValue("left_side_percent", laneDepartureWarningReport.LeftSidePercent);
                    command.Parameters.AddWithValue("right_side_percent", laneDepartureWarningReport.RightSidePercent);
                    command.Parameters.AddWithValue("left_side_line_length", laneDepartureWarningReport.LeftSideLineLength);
                    command.Parameters.AddWithValue("right_side_line_length", laneDepartureWarningReport.RightSideLineLength);
                    command.Parameters.AddWithValue("span_line_angle", laneDepartureWarningReport.SpanLineAngle);
                    command.Parameters.AddWithValue("span_line_length", laneDepartureWarningReport.SpanLineLength);
                    command.Parameters.AddWithValue("left_side_line_number", laneDepartureWarningReport.LeftSideLineNumber);
                    command.Parameters.AddWithValue("right_side_line_number", laneDepartureWarningReport.RightSideLineNumber);
                    var result = Convert.ToInt64(await command.ExecuteScalarAsync());
                    return result;
                }
                else
                {
                    await using var command = new NpgsqlCommand(PostgreSQLCommands.UpdateReport, connection);
                    command.Parameters.AddWithValue("id", laneDepartureWarningReport.Id);
                    command.Parameters.AddWithValue("video_id", laneDepartureWarningReport.VideoId);
                    command.Parameters.AddWithValue("processed_frames", laneDepartureWarningReport.ProcessedFrames);
                    command.Parameters.AddWithValue("success_frames", laneDepartureWarningReport.SuccessFrames);
                    command.Parameters.AddWithValue("fail_frames", laneDepartureWarningReport.FailFrames);
                    command.Parameters.AddWithValue("success_rate", laneDepartureWarningReport.SuccessRate);
                    command.Parameters.AddWithValue("left_side_percent", laneDepartureWarningReport.LeftSidePercent);
                    command.Parameters.AddWithValue("right_side_percent", laneDepartureWarningReport.RightSidePercent);
                    command.Parameters.AddWithValue("left_side_line_length", laneDepartureWarningReport.LeftSideLineLength);
                    command.Parameters.AddWithValue("right_side_line_length", laneDepartureWarningReport.RightSideLineLength);
                    command.Parameters.AddWithValue("span_line_angle", laneDepartureWarningReport.SpanLineAngle);
                    command.Parameters.AddWithValue("span_line_length", laneDepartureWarningReport.SpanLineLength);
                    command.Parameters.AddWithValue("left_side_line_number", laneDepartureWarningReport.LeftSideLineNumber);
                    command.Parameters.AddWithValue("right_side_line_number", laneDepartureWarningReport.RightSideLineNumber);
                    await command.ExecuteNonQueryAsync();
                    return laneDepartureWarningReport.Id;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        //============================================================
        public async Task DeleteAsync(LaneDepartureWarningReport laneDepartureWarningReport)
        {
            await using var connection = new NpgsqlConnection(Constants.ServerConstants.GetPsqlConnectionString());
            try
            {
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand(PostgreSQLCommands.DeleteReport, connection);
                command.Parameters.AddWithValue("id", laneDepartureWarningReport.Id);
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        //============================================================
        public void Dispose()
        {
            
        }
    }
}
