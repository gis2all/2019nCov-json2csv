using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Json2Csv
{
    public static class Utils
    {
        public static List<Epidemic> GetEpidemicData(EpidemicType epidemicType, string jsonFile)
        {
            var jsonContent = File.ReadAllText(jsonFile);
            var jObject = JObject.Parse(jsonContent);

            if (epidemicType == EpidemicType.Province)
            {
                return GetTotalProvinceEpidemic(jObject);
            }
            if (epidemicType == EpidemicType.City)
            {
                return GetTotalCityEpidemic(jObject);
            }
            return null;
        }

        private static List<Epidemic> GetTotalProvinceEpidemic(JToken provinceJToken)
        {
            var provinceObj = provinceJToken["province"];
            var dailyProvinceList = provinceObj.ToList();
            var proEpidemicList = new List<Epidemic>();
            for (int i = 0; i < dailyProvinceList.Count; i++)
            {
                var dailyProvinceInfo = dailyProvinceList[i];
                var provinceDetail = dailyProvinceInfo["ProvinceDetail"].ToList();
                for (int j = 0; j < provinceDetail.Count; j++)
                {
                    // Daily province epidemic data
                    var proEpidemic = new ProvinceEpidemic
                    {
                        DateTime = DateTime.Parse(dailyProvinceInfo["Time"].ToString()),
                        ProvinceName = provinceDetail[j]["Province"].ToString(),
                        ConfirmedCount = (int)provinceDetail[j]["Confirmed"],
                        DeadCount = (int)provinceDetail[j]["Dead"],
                        CuredCount = (int)provinceDetail[j]["Cured"]
                    };
                    // Historical province epidemic data
                    proEpidemicList.Add(proEpidemic);
                }
            }
            return proEpidemicList;
        }

        private static List<Epidemic> GetTotalCityEpidemic(JToken cityJToken)
        {
            var cityObj = cityJToken["city"];
            var dailyCityList = cityObj.ToList();
            var cityEpidemicList = new List<Epidemic>();
            for (int i = 0; i < dailyCityList.Count; i++)
            {
                var dailyCityInfo = dailyCityList[i];
                var cityDetail = dailyCityInfo["CityDetail"].ToList();
                for (int j = 0; j < cityDetail.Count; j++)
                {
                    // Daily city epidemic data
                    var cityEpidemic = new CityEpidemic
                    {
                        DateTime = DateTime.Parse(dailyCityInfo["Time"].ToString()),
                        ProvinceName = cityDetail[j]["Province"].ToString(),
                        CityName = cityDetail[j]["City"].ToString(),
                        ConfirmedCount = (int)cityDetail[j]["Confirmed"],
                        DeadCount = (int)cityDetail[j]["Dead"],
                        CuredCount = (int)cityDetail[j]["Cured"]
                    };
                    // Historical city epidemic data
                    cityEpidemicList.Add(cityEpidemic);
                }
            }
            return cityEpidemicList;
        }

        public static List<Epidemic> OrberByConfirmedCount(List<Epidemic> epidemicList)
        {
            // Order by ConfirmedCount and from more to less
            var newList = epidemicList.OrderByDescending(k => k.ConfirmedCount).ToList();
            // Add ID property to count
            for (int i = 0; i < newList.Count; i++)
            {
                newList[i].ID = i + 1;
            }
            return newList;
        }

        public static void GenerateTxtFile(List<Epidemic> epidemicList, EpidemicType epidemicType, string directory)
        {
            if (epidemicList is null || epidemicList.Count == 0)
            {
                Console.Write("No Data! \r\n");
                return;
            }
            // Total days        
            var startDate = DateTime.Parse("2020-01-15");
            var endDate = GetLastDate(epidemicList);
            var days = endDate - startDate;
            // Generate statistics data for each day
            for (int i = 0; i <= days.Days; i++)
            {
                // Date increment
                var date = startDate + TimeSpan.FromDays(i);
                Console.WriteLine(date.ToString("yyyy-MM-dd"));
                // Statistics data list for the day
                var dailyEpidemicList = epidemicList.Where(p => p.DateTime.Month == date.Month && p.DateTime.Day == date.Day).ToList();
                // Order by ConfirmedCount
                var dailyEpidemicList_2 = Utils.OrberByConfirmedCount(dailyEpidemicList);
                if (epidemicType == EpidemicType.Province)
                {
                    // Create province result file
                    var dir = CreateDirForResult(EpidemicType.Province, directory);
                    var path = dir + $"Epidemic_Province_{date.ToString("yyyy_MM_dd")}.txt";
                    // Write title
                    File.WriteAllText(path, "ID,ProvinceName,ConfirmedCount,DeadCount,CuredCount \r\n");
                    Console.WriteLine("ID,ProvinceName,ConfirmedCount,DeadCount,CuredCount \r\n");
                    // Write data
                    for (int k = 0; k < dailyEpidemicList_2.Count; k++)
                    {
                        var province = dailyEpidemicList_2[k] as ProvinceEpidemic;
                        File.AppendAllText(path, $"{province.ID},{province.ProvinceName},{province.ConfirmedCount}," +
                            $"{province.DeadCount},{province.CuredCount} \r\n");
                        Console.WriteLine($"{province.ID},{province.ProvinceName},{province.ConfirmedCount}," +
                            $"{province.DeadCount},{province.CuredCount} \r\n");
                    }
                }

                if (epidemicType == EpidemicType.City)
                {
                    // Create city result file
                    var dir = CreateDirForResult(EpidemicType.City, directory);
                    var path = dir + $"Epidemic_City_{date.ToString("yyyy_MM_dd")}.txt";
                    // Write title
                    File.WriteAllText(path, "ID,ProvinceName,CityName,FullCityName,ConfirmedCount,DeadCount,CuredCount \r\n");
                    Console.WriteLine("ID,ProvinceName,CityName,FullCityName,ConfirmedCount,DeadCount,CuredCount \r\n");
                    // Write data
                    for (int k = 0; k < dailyEpidemicList_2.Count; k++)
                    {
                        var city = dailyEpidemicList_2[k] as CityEpidemic;
                        File.AppendAllText(path, $"{city.ID},{city.ProvinceName},{city.CityName},{city.ProvinceName + city.CityName},{city.ConfirmedCount}," +
                            $"{city.DeadCount} {city.CuredCount} \r\n");
                        Console.WriteLine($"{city.ID},{city.ProvinceName},{city.CityName},{city.ProvinceName + city.CityName},{city.ConfirmedCount}," +
                            $"{city.DeadCount},{city.CuredCount} \r\n");
                    }
                }
            }
        }

        private static DateTime GetLastDate(List<Epidemic> epidemicList)
        {
            return epidemicList.FirstOrDefault().DateTime;
        }

        private static string CreateDirForResult(EpidemicType epidemicType, string dir)
        {
            if (!string.Equals(dir.LastOrDefault(), Path.DirectorySeparatorChar))
            {
                dir += Path.DirectorySeparatorChar;
            }
            switch (epidemicType)
            {
                case EpidemicType.Province:
                    dir += ("Province" + Path.DirectorySeparatorChar);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    return dir;
                case EpidemicType.City:
                    dir += ("City" + Path.DirectorySeparatorChar);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    return dir;
                default:
                    return null;
            }
        }

        public static string GetProjectPath()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            return baseDir.Split("bin").FirstOrDefault();
        }
    }
}
