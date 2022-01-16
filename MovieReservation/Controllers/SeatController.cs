using Microsoft.Ajax.Utilities;
using MovieReservation.DataModel.dbmx;
using MovieReservation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieReservation.Controllers
{
    public class SeatController : Controller
    {
        private readonly MovieSeatReservationDBEntities _database = new MovieSeatReservationDBEntities();
       
        public ActionResult SeatIndex(int id)
        {
            var seatsPerSched = _database.SeatAvailabilityPerScheds.Where(item => item.MovieScheduleId == id).ToList();
            var movieSchedId = _database.SeatAvailabilityPerScheds.Where(item => item.MovieScheduleId == id).FirstOrDefault();

            List <SeatAvailabilityPerSchedModel> seats = new List<SeatAvailabilityPerSchedModel>();

            foreach (var seat in seatsPerSched)
            {
                SeatAvailabilityPerSchedModel model = new SeatAvailabilityPerSchedModel();
                model.Id = seat.Id;
                model.SeatId = (int)seat.SeatId;
                model.MovieScheduleId = (int)seat.MovieScheduleId;
                model.IsSeatAvailable = (bool)seat.IsSeatAvailable;
                model.SeatNumber = seat.Seat.SeatNumber;
                model.Row = (int)seat.Seat.SeatRow;
                model.Column = (int)seat.Seat.SeatColumn;
                seats.Add(model);
            }

            var x = seats.GroupBy(item => item.Column);

            ViewBag.Seats = x;
            ViewBag.MovieSchedId = movieSchedId.MovieScheduleId;
            ViewBag.MovieSched = movieSchedId.MovieSchedule.Schedule;

            //Get Movie
            var movie = _database.MovieSchedules.Where(item => item.Id == id).Select(m => m.Movie).FirstOrDefault();
            ViewBag.MovieDetail = movie;

            return View();
        }


        public JsonResult CreateReservation(int mId, string cname, List<string> seats, float amount, int mSchedId, string mSched)
        { 
            foreach (var chosenSeat in seats)
            {
                var seatId = _database.Seats.Where(item => item.SeatNumber == chosenSeat).Select(item => item.Id).FirstOrDefault();

                MovieReserve reserve = new MovieReserve();
                reserve.MovieId = mId;
                reserve.SeatId = seatId;
                reserve.CustomerName = cname;
                reserve.ReservationDate = DateTime.Now;
                reserve.TotalAmount = amount;
                reserve.ReservationStatusId = 1;
                reserve.MovieSchedule = Convert.ToDateTime(mSched);
                _database.MovieReserves.Add(reserve);
                
                var seatAvailability = _database.SeatAvailabilityPerScheds.FirstOrDefault(seat => seat.Seat.SeatNumber == chosenSeat && seat.MovieScheduleId == mSchedId);

                SeatAvailabilityPerSched availability = _database.SeatAvailabilityPerScheds.Find(seatAvailability.Id);
                availability.IsSeatAvailable = false;

                _database.SaveChanges();
            }

            return Json(new { result = "Redirect", url = Url.Action("Index", "Home") });
        }


    }
}