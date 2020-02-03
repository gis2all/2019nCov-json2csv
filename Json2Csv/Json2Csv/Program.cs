using System;

namespace Json2Csv
{
    class Program
    {
        static void Main(string[] args)
        {
            // Download lastest history json data form http://ncov.nosensor.com:8080/api/ (https://github.com/wuhan2020/map-viz)
            // I didn’t get json from the web page in real time since there were too many people requesting it and the website was unstable
            // You can replace this json file with the lastest json file
            var historyData = Utils.GetProjectPath() + @"data\history_data_02_01.json";
            var outTxtDir = AppDomain.CurrentDomain.BaseDirectory;

            GetEpidmicResult(EpidemicType.Province, historyData, outTxtDir);
            GetEpidmicResult(EpidemicType.City, historyData, outTxtDir);
            Console.ReadKey();
        }

        private static void GetEpidmicResult(EpidemicType epidemicType, string historyDataPath, string outDir)
        {
            var epidemicList = Utils.GetEpidemicData(epidemicType, historyDataPath);
            var newEpidemicList = Utils.OrberByConfirmedCount(epidemicList);
            Utils.GenerateTxtFile(newEpidemicList, epidemicType, outDir);
            Console.WriteLine("Generate txt file successfully!");
        }
    }
}
