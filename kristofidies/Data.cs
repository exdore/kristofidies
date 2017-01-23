using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace kristofidies
{
    public class Data
    {
        public List<City> Cities { get; set; }
        public List<List<int>>  DistanciesList;
        public int CitiesCount;

        public static Data ReadData()
        {
            var streamReader = new StreamReader(@"data.txt", System.Text.Encoding.GetEncoding(1251));
            var sourceData = new List<City>();
            var line = streamReader.ReadLine();
            if (line != null)
            {
                do
                {
                    var sourceList = line.Split(' ').ToList();
                    sourceData.Add(new City
                    {
                        X = Convert.ToDouble(sourceList[1]),
                        Y = Convert.ToDouble(sourceList[2]),
                        Number = Convert.ToInt32(sourceList[0])
                    });
                } while ((line = streamReader.ReadLine()) != null);
                streamReader.Close();
                return new Data()
                {
                    Cities = sourceData,
                    CitiesCount = sourceData.Count
                };
            }
            return null;
        }

        public void CalculateDistanciesList()
        {
            var distancies = new List<List<int>>();
            foreach (var city1 in Cities)
            {
                distancies.Add(new List<int>());
                foreach (var city2 in Cities)
                {
                    distancies.Last().Add(Convert.ToInt32(Math.Floor(Math.Sqrt(Math.Pow(city1.X - city2.X, 2) + Math.Pow(city1.Y - city2.Y, 2)))));
                }
            }
            DistanciesList = distancies;
        }
    }
}
