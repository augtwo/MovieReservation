using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieReservation.Models
{
    public class MovieReservationModel
    {
        public int Id { get; set; }
        public string MovieName { get; set; }
        public string ReservationDate { get; set; }
        public string CustomerName { get; set; }
        public int SeatId { get; set; }
        public string SeatNumber { get; set; }
        public float TotalAmount { get; set; }
        public string ReservationStatusName { get; set; }
        public DateTime MovieSchedule { get; set; }
    }
}