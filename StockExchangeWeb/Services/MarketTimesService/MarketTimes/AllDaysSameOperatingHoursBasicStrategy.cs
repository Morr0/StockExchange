using System;
using System.Collections.Generic;

namespace StockExchangeWeb.Services.MarketTimesService.MarketTimes
{
    public class AllDaysSameOperatingHoursBasicStrategy : TimesStrategy
    {
        private Dictionary<DayOfWeek, bool> _daysOpen = new Dictionary<DayOfWeek, bool>(7);

        // UTC zone
        private TimeSpan _openTime;
        private TimeSpan _closeTime;

        public AllDaysSameOperatingHoursBasicStrategy(bool openMonday, bool openTuesday, bool openWednesday, bool openThursday, 
            bool openFriday, bool openSaturday, bool openSunday, TimeSpan openTime, TimeSpan closeTime)
        {
            AssignOpenDays(openMonday, openTuesday, openWednesday, openThursday, openFriday, openSaturday, openSunday);

            _openTime = openTime;
            _closeTime = closeTime;
        }

        private void AssignOpenDays(in bool openMonday, in bool openTuesday, in bool openWednesday, in bool openThursday, 
            in bool openFriday, in bool openSaturday, in bool openSunday)
        {
            _daysOpen.Add(DayOfWeek.Monday, openMonday);
            _daysOpen.Add(DayOfWeek.Tuesday, openTuesday);
            _daysOpen.Add(DayOfWeek.Wednesday, openWednesday);
            _daysOpen.Add(DayOfWeek.Thursday, openThursday);
            _daysOpen.Add(DayOfWeek.Friday, openFriday);
            _daysOpen.Add(DayOfWeek.Saturday, openSaturday);
            _daysOpen.Add(DayOfWeek.Sunday, openSunday);
        }

        public override bool OpenNow()
        {
            DateTime currentTime = DateTime.UtcNow;

            if (!_daysOpen[currentTime.DayOfWeek])
                return false;

            return currentTime.TimeOfDay >= _openTime && currentTime.TimeOfDay <= _closeTime;
        }
    }
}