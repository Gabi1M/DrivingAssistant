using DrivingAssistant.Core.Models.ImageProcessing;

namespace DrivingAssistant.WebServer.Processing
{
    public interface IProcessingAlgorithm
    {
        //======================================================//
        public string ProcessVideo(string filename, int framesToSkip, out VideoReport report);
    }
}
