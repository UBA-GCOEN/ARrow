using System.Collections.Generic;
using System.Xml.Serialization;
using System;

namespace Gpm.Manager.Constant
{
    using Util;

    [XmlRoot("noticeInfo")]
    public class NoticeInfo
    {
        public class Day
        {
            [XmlElement("start")]
            public string start;
            [XmlElement("end")]
            public string end;
        }

        public class TimeInfo
        {
            [XmlElement("startTime")]
            public string startTime;
            [XmlElement("endTime")]
            public string endTime;
            [XmlElement("day")]
            public Day day;
        }

        [XmlRoot("noticeList")]
        public class NoticeList
        {
            [XmlRoot("notice")]
            public class Notice
            {
                [XmlElement("text")]
                public string text;
                [XmlElement("url")]
                public string url;
                [XmlElement("timeInfo")]
                public TimeInfo timeInfo;

                public bool IsActiveTime()
                {
                    DateTime dateTimeStart = ManagerTime.ParseTimeInfo(timeInfo.startTime);
                    DateTime dateTimeEnd = ManagerTime.ParseTimeInfo(timeInfo.endTime);

                    DateTime now = ManagerTime.Now;
                    long ticks = now.Ticks;
                    int utcNowHour = now.Hour % 24;

                    if (ticks >= dateTimeStart.Ticks
                        && ticks <= dateTimeEnd.Ticks)
                    {
                        if (utcNowHour >= int.Parse(timeInfo.day.start)
                            && utcNowHour <= int.Parse(timeInfo.day.end))
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }

            [XmlElement("notice")]
            public List<Notice> list;
        }

        public NoticeList noticeList;
    }
}