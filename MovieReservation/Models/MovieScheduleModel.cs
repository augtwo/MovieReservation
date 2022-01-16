using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieReservation.Models
{
    public class MovieScheduleModel
    {
        public int Id { get; set; }
        public DateTime ScheduleDate { get; set; }
        public DateTime ScheduleTime { get; set; }
        public int MovieId { get; set; }
        public TimeSpan Time { get; set; }
    }
}