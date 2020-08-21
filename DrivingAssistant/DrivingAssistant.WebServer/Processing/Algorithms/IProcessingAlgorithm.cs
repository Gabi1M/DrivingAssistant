using DrivingAssistant.Core.Models.Reports;

namespace DrivingAssistant.WebServer.Processing.Algorithms
{
    public interface IProcessingAlgorithm
    {
        //======================================================//
        public string ProcessVideo(string filename, int framesToSkip, out VideoReport report);
    }
}
