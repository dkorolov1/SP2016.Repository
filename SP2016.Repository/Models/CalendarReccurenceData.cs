using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace SP2016.Repository.Models
{
    [Serializable()]
    [XmlRoot("recurrence")]
    public class CalendarReccurenceData
    {
        [XmlElement("rule")]
        public CalendarRule Rule { get; set; }
    }

    [Serializable()]
    [XmlRoot("rule")]
    public class CalendarRule
    {
        [XmlElement("firstDayOfWeek")]
        public CalendarDayOfWeek FirstDayOfWeek { get; set; }

        [XmlElement("repeat")]
        public CalendarRepeat CalendarRepeat { get; set; }

        [XmlElement("repeatForever")]
        public bool RepeatForever { get; set; }
    }

    public enum CalendarDayOfWeek
    {
        [XmlEnum("mo")]
        Monday = 0,
        [XmlEnum("tu")]
        Tuesday = 1,
        [XmlEnum("we")]
        Wednesday = 2,
        [XmlEnum("th")]
        Thursday = 3,
        [XmlEnum("fr")]
        Friday = 4,
        [XmlEnum("sa")]
        Saturday = 5,
        [XmlEnum("su")]
        Sunday = 6
    }

    public class CalendarRepeat
    {
        [XmlElement(Type = typeof(CalendarYearRepeatSettings), ElementName = "yearly")]
        [XmlElement(Type = typeof(CalendarMounthRepeatSettings), ElementName = "monthly")]
        [XmlElement(Type = typeof(CalendarWeekRepeatSettings), ElementName = "weekly")]
        public CalendarBaseRepeatSettings RepeatSettings { get; set; }
    }

    [XmlInclude(typeof(CalendarYearRepeatSettings))]
    [XmlInclude(typeof(CalendarMounthRepeatSettings))]
    [XmlInclude(typeof(CalendarWeekRepeatSettings))]
    public abstract class CalendarBaseRepeatSettings
    {

    }

    public class CalendarYearRepeatSettings : CalendarBaseRepeatSettings
    {
        [XmlAttribute("yearFrequency")]
        public int Frequency { get; set; }

        [XmlAttribute("month")]
        public int Mounth { get; set; }

        [XmlAttribute("day")]
        public int Day { get; set; }
    }

    public class CalendarMounthRepeatSettings : CalendarBaseRepeatSettings
    {
        [XmlAttribute("monthFrequency")]
        public int Frequency { get; set; }

        [XmlAttribute("day")]
        public int Day { get; set; }
    }

    public class CalendarWeekRepeatSettings : CalendarBaseRepeatSettings
    {
        [XmlAttribute("weekFrequency")]
        public int Frequency { get; set; }

        [XmlArrayItem(Type = typeof(CalendarDayOfWeek))]
        public HashSet<CalendarDayOfWeek> DaysOfWeek { get; set; }
    }
}
