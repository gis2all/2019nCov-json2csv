using System;
using System.Collections.Generic;
using System.Text;

namespace Json2Csv
{
    public interface Epidemic
    {
        int ID { get; set; }
        DateTime DateTime { get; set; }
        int ConfirmedCount { get; set; }
        int CuredCount { get; set; }
        int DeadCount { get; set; }
    }

    public class ProvinceEpidemic : Epidemic
    {
        public string ProvinceName { get; set; }
        public int ID { get; set; }
        public DateTime DateTime { get; set; }
        public int ConfirmedCount { get; set; }
        public int CuredCount { get; set; }
        public int DeadCount { get; set; }
    }

    public class CityEpidemic : Epidemic
    {
        public string ProvinceName { get; set; }
        public string CityName { get; set; }
        public string FullCityName { get; set; }
        public int ID { get; set; }
        public DateTime DateTime { get; set; }
        public int ConfirmedCount { get; set; }
        public int CuredCount { get; set; }
        public int DeadCount { get; set; }
    }
}
