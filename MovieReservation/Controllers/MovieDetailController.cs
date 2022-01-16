using MovieReservation.DataModel.dbmx;
using MovieReservation.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieReservation.Controllers
{
    public class MovieDetailController : Controller
    {
        private readonly MovieSeatReservationDBEntities _database = new MovieSeatReservationDBEntities();

        private DateTime? ChosenDate { get; set; }

        public ActionResult MovieDetail(int id)
        {
            var model = _database.Movies.FirstOrDefault(item => item.Id == id);

            if (model == null)
            {
                return RedirectToAction("Index", "Home");
            }

            MovieModel movie = new MovieModel()
            {
                Title = model.Title,
                Synopsis = model.Synopsis,
                Cast = model.Cast,
                Category = model.Category,
                Price = (float)model.Price,
                RunTime = model.RunTime,
                Image = model.Image
            };

            ViewBag.Schedules = GetSchedules(id);

            return View(movie);
        }

        [HttpGet]
        public ActionResult AddSchedule(int id)
        {
            ViewBag.Movie = GetMovie(id);
            ViewBag.Date = GetSchedule();
            ViewBag.Schedules = GetSchedules(id);
            return View();
        }

        private DateTime? GetSchedule()
        {
            ChosenDate = DateTime.Now;

            return ChosenDate;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSchedule(MovieScheduleModel sched)
        {
            if (sched != null)
            {
                MovieSchedule schedule = new MovieSchedule();
                schedule.MovieId = sched.MovieId;
                schedule.ScheduleDate = sched.ScheduleDate;
                schedule.ScheduledTime = sched.ScheduleTime;

                var date = schedule.ScheduleDate.Value.Date;
                var time = schedule.ScheduledTime.Value.TimeOfDay;

                schedule.Schedule = date + time;
                _database.MovieSchedules.Add(schedule);

                var id = _database.MovieSchedules.Where(item => item.MovieId == schedule.Id && item.Schedule == (DateTime)schedule.Schedule).Select(item => item.Id).FirstOrDefault();

                AddSeatAvailability(id);
                _database.SaveChanges();

                return RedirectToAction("AddSchedule", schedule.Id);
            }

            return View();
        }

        private void AddSeatAvailability(int id)
        {
            var allSeats = _database.Seats.ToList();

            foreach (var seat in allSeats)
            {
                SeatAvailabilityPerSched seatsPerSched = new SeatAvailabilityPerSched();
                seatsPerSched.MovieScheduleId = id;
                seatsPerSched.SeatId = seat.Id;
                seatsPerSched.IsSeatAvailable = true;
                _database.SeatAvailabilityPerScheds.Add(seatsPerSched);
            }

        }

        public MovieModel GetMovie(int id)
        {
            var model = _database.Movies.FirstOrDefault(item => item.Id == id);

            MovieModel movie = new MovieModel()
            {
                Id = model.Id,
                Title = model.Title,
                Image = model.Image
            };

            return movie;
        }

        private Dictionary<DateTime, List<MovieScheduleModel>> GetSchedules(int id)
        {
            var model = _database.MovieSchedules.Where(item => item.MovieId == id).ToList();
            List<MovieScheduleModel> schedTimeList = new List<MovieScheduleModel>();

            foreach (var item in model)
            {
                MovieScheduleModel sched = new MovieScheduleModel();
                sched.Id = item.Id;
                sched.ScheduleTime = item.Schedule.Value;
                sched.ScheduleDate = item.ScheduleDate.Value.Date;
                sched.Time = item.ScheduledTime.Value.TimeOfDay;
                schedTimeList.Add(sched);
            }

            var timePerDate = schedTimeList.GroupBy(x => x.ScheduleDate.Date);

            List<MovieScheduleModel> availableTimes = new List<MovieScheduleModel>();
            var schedsPerDate = new Dictionary<DateTime, List<MovieScheduleModel>>();

            foreach (var date in timePerDate)
            {
                foreach (var item in date)
                {
                    availableTimes.Add(item);
                }
                schedsPerDate.Add(date.Key.Date, availableTimes);
            }

            return schedsPerDate;

        }
    }
}