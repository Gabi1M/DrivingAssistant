using System;
using Newtonsoft.Json;

namespace DrivingAssistant.Core.Models.ImageProcessing
{
    public class Parameters
    {
        [JsonProperty("CannyThreshold")]
        public double CannyThreshold { get; set; }

        [JsonProperty("CannyThresholdLinking")]
        public double CannyThresholdLinking { get; set; }

        [JsonProperty("HoughLinesRhoResolution")]
        public double HoughLinesRhoResolution { get; set; }

        [JsonProperty("HoughLinesThetaResolution")]
        public double HoughLinesThetaResolution { get; set; }

        [JsonProperty("HoughLinesMinimumLineWidth")]
        public double HoughLinesMinimumLineWidth { get; set; }

        [JsonProperty("HoughLinesGapBetweenLines")]
        public double HoughLinesGapBetweenLines { get; set; }

        [JsonProperty("HoughLinesThreshold")]
        public int HoughLinesThreshold { get; set; }

        [JsonProperty("DilateIterations")]
        public int DilateIterations { get; set; }

        //======================================================//
        public static Parameters Default()
        {
            return new Parameters
            {
                CannyThreshold = 100,
                CannyThresholdLinking = 150,
                HoughLinesRhoResolution = 1,
                HoughLinesThetaResolution = Math.PI / 100,
                HoughLinesMinimumLineWidth = 5,
                HoughLinesGapBetweenLines = 5,
                HoughLinesThreshold = 10
            };
        }
    }
}
