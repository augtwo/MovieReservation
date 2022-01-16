using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieReservation.Models
{
    public class SeatAvailabilityPerSchedModel
    {
        public int Id { get; set; }
        public int SeatId { get; set; }
        public int MovieScheduleId { get; set; }
        public bool IsSeatAvailable { get; set; }
        public string SeatNumber { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public List<int> SelectedSeats { get; set; }
    }
}