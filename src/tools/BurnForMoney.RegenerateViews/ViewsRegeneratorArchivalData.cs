﻿using System.Collections.Generic;
using BurnForMoney.Functions.Shared.Helpers;
using BurnForMoney.Infrastructure.Persistence.Repositories.Dto;

namespace BurnForMoney.RegenerateViews
{
    public class ViewsRegeneratorArchivalData
    {
        public class Data
        {
            public string Date { get; set; }
            public AthleteMonthlyResult Results { get; set; }
        }

        public static readonly List<Data> ArchivalData = new List<Data>
        {
            new Data
            {
                Date = "2016/11",
                Results = new AthleteMonthlyResult
                {
                    Points = 1812,
                    Distance = UnitsConverter.ConvertKilometersToMeters(1018),
                    Time = UnitsConverter.ConvertHoursToMinutes(32)
                }
            },
            new Data
            {
                Date = "2016/12",
                Results = new AthleteMonthlyResult
                {
                    Points = 2027,
                    Distance = UnitsConverter.ConvertKilometersToMeters(1002),
                    Time = UnitsConverter.ConvertHoursToMinutes(71)
                }
            },
            new Data
            {
                Date = "2017/1",
                Results = new AthleteMonthlyResult
                {
                    Points = 2270,
                    Distance = UnitsConverter.ConvertKilometersToMeters(1001),
                    Time = UnitsConverter.ConvertHoursToMinutes(131)
                }
            },
            new Data
            {
                Date = "2017/2",
                Results = new AthleteMonthlyResult
                {
                    Points = 3333,
                    Distance = UnitsConverter.ConvertKilometersToMeters(1175),
                    Time = UnitsConverter.ConvertHoursToMinutes(250)
                }
            },
            new Data
            {
                Date = "2017/3",
                Results = new AthleteMonthlyResult
                {
                    Points = 4547,
                    Distance = UnitsConverter.ConvertKilometersToMeters(2272),
                    Time = UnitsConverter.ConvertHoursToMinutes(189)
                }
            },
            new Data
            {
                Date = "2017/4",
                Results = new AthleteMonthlyResult
                {
                    Points = 5164,
                    Distance = UnitsConverter.ConvertKilometersToMeters(2993),
                    Time = UnitsConverter.ConvertHoursToMinutes(254)
                }
            },
            new Data
            {
                Date = "2017/5",
                Results = new AthleteMonthlyResult
                {
                    Points = 8239,
                    Distance = UnitsConverter.ConvertKilometersToMeters(5682),
                    Time = UnitsConverter.ConvertHoursToMinutes(179)
                }
            },
            new Data
            {
                Date = "2017/6",
                Results = new AthleteMonthlyResult
                {
                    Points = 9267,
                    Distance = UnitsConverter.ConvertKilometersToMeters(6557),
                    Time = UnitsConverter.ConvertHoursToMinutes(205)
                }
            },
            new Data
            {
                Date = "2017/7",
                Results = new AthleteMonthlyResult
                {
                    Points = 9801,
                    Distance = UnitsConverter.ConvertKilometersToMeters(6862),
                    Time = UnitsConverter.ConvertHoursToMinutes(237)
                }
            },
            new Data
            {
                Date = "2017/8",
                Results = new AthleteMonthlyResult
                {
                    Points = 7405,
                    Distance = UnitsConverter.ConvertKilometersToMeters(4589),
                    Time = UnitsConverter.ConvertHoursToMinutes(254)
                }
            },
            new Data
            {
                Date = "2017/9",
                Results = new AthleteMonthlyResult
                {
                    Points = 3681,
                    Distance = UnitsConverter.ConvertKilometersToMeters(1985),
                    Time = UnitsConverter.ConvertHoursToMinutes(225)
                }
            },
            new Data
            {
                Date = "2017/10",
                Results = new AthleteMonthlyResult
                {
                    Points = 3776,
                    Distance = UnitsConverter.ConvertKilometersToMeters(1806),
                    Time = UnitsConverter.ConvertHoursToMinutes(207)
                }
            },
            new Data
            {
                Date = "2017/11",
                Results = new AthleteMonthlyResult
                {
                    Points = 2526,
                    Distance = UnitsConverter.ConvertKilometersToMeters(1177),
                    Time = UnitsConverter.ConvertHoursToMinutes(156)
                }
            },
            new Data
            {
                Date = "2017/12",
                Results = new AthleteMonthlyResult
                {
                    Points = 2553,
                    Distance = UnitsConverter.ConvertKilometersToMeters(1389),
                    Time = UnitsConverter.ConvertHoursToMinutes(109)
                }
            },
            new Data
            {
                Date = "2018/1",
                Results = new AthleteMonthlyResult
                {
                    Points = 3157,
                    Distance = UnitsConverter.ConvertKilometersToMeters(1182),
                    Time = UnitsConverter.ConvertHoursToMinutes(250)
                }
            },
            new Data
            {
                Date = "2018/2",
                Results = new AthleteMonthlyResult
                {
                    Points = 3267,
                    Distance = UnitsConverter.ConvertKilometersToMeters(901),
                    Time = UnitsConverter.ConvertHoursToMinutes(251)
                }
            },
            new Data
            {
                Date = "2018/3",
                Results = new AthleteMonthlyResult
                {
                    Points = 4171,
                    Distance = UnitsConverter.ConvertKilometersToMeters(2469),
                    Time = UnitsConverter.ConvertHoursToMinutes(309)
                }
            },
            new Data
            {
                Date = "2018/4",
                Results = new AthleteMonthlyResult
                {
                    Points = 4506,
                    Distance = UnitsConverter.ConvertKilometersToMeters(2272),
                    Time = UnitsConverter.ConvertHoursToMinutes(205)
                }
            },
            new Data
            {
                Date = "2018/5",
                Results = new AthleteMonthlyResult
                {
                    Points = 5952,
                    Distance = UnitsConverter.ConvertKilometersToMeters(3359),
                    Time = UnitsConverter.ConvertHoursToMinutes(107)
                }
            },
            new Data
            {
                Date = "2018/6",
                Results = new AthleteMonthlyResult
                {
                    Points = 4884,
                    Distance = UnitsConverter.ConvertKilometersToMeters(2507),
                    Time = UnitsConverter.ConvertHoursToMinutes(287)
                }
            },
            new Data
            {
                Date = "2018/7",
                Results = new AthleteMonthlyResult
                {
                    Points = 4579,
                    Distance = UnitsConverter.ConvertKilometersToMeters(2272),
                    Time = UnitsConverter.ConvertHoursToMinutes(189)
                }
            },
            new Data
            {
                Date = "2018/8",
                Results = new AthleteMonthlyResult
                {
                    Points = 6255,
                    Distance = UnitsConverter.ConvertKilometersToMeters(3359),
                    Time = UnitsConverter.ConvertHoursToMinutes(107)
                }
            },
            new Data
            {
                Date = "2018/9",
                Results = new AthleteMonthlyResult
                {
                    Points = 4626,
                    Distance = UnitsConverter.ConvertKilometersToMeters(3451),
                    Time = UnitsConverter.ConvertHoursToMinutes(79)
                }
            },
            new Data
            {
                Date = "2018/10",
                Results = new AthleteMonthlyResult
                {
                    Points = 4191,
                    Distance = UnitsConverter.ConvertKilometersToMeters(2474),
                    Time = UnitsConverter.ConvertHoursToMinutes(181)
                }
            },
            new Data
            {
                Date = "2018/11",
                Results = new AthleteMonthlyResult
                {
                    Points = 2020,
                    Distance = 1662676,
                    Time = 19046
                }
            }
        };
    }
}