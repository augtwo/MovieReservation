using MovieReservation.DataModel.dbmx;
using MovieReservation.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieReservation.Controllers
{
    public class HomeController : Controller
    {
        private readonly MovieSeatReservationDBEntities _database = new MovieSeatReservationDBEntities();
        public ActionResult Index()
        {
            var movies = _database.Movies.ToList();

            return View(movies);
        }

        

        public ActionResult AdminIndex()
        {
            var movies = _database.Movies.ToList();
            ViewBag.History = GetReservationHistory();

            return View(movies);
        }

        public IEnumerable<MovieReservationModel> GetReservationHistory()
        {
            var reservations = _database.MovieReserves.ToList();
            List<MovieReservationModel> reserves = new List<MovieReservationModel>();

            foreach (var item in reservations)
            {
                var mreservation = _database.MovieReserves.FirstOrDefault(x => x.Id == item.Id);
                var findId = _database.MovieReserves.Find(mreservation.Id);

                if (findId.MovieSchedule < DateTime.Now)
                {
                    findId.ReservationStatusId = 3;
                }
                _database.SaveChanges();

                MovieReservationModel model = new MovieReservationModel();
                model.CustomerName = item.CustomerName;
                model.MovieName = item.Movie.Title;
                model.ReservationDate = item.ReservationDate.Value.Date.ToLongDateString();
                model.SeatNumber = item.Seat.SeatNumber;
                model.ReservationStatusName = item.ReservationStatu.ReservationStatusName;
                reserves.Add(model);
            }

            var reservationList = reserves
                            .GroupBy(x => new { x.ReservationDate, x.CustomerName })
                            .OrderBy(g => g.Key.ReservationDate).ThenBy(g => g.Key.CustomerName)
                            .Select(g => new
                            {
                                ReserveDate = g.Key.ReservationDate,
                                CustName = g.Key.CustomerName,
                                Reservation = g.OrderBy(x => x.ReservationDate).ToList()
                            });

            string joinSeats = "";
            int tempID = 0;
            string movieName= "", reservationStat = "";
            List<string> allSeats = new List<string>();
            List<MovieReservationModel> reser = new List<MovieReservationModel>();
            List<MovieReservationModel> reser2 = new List<MovieReservationModel>();

            foreach (var reserve in reservationList)
            {

                reser = new List<MovieReservationModel> { new MovieReservationModel { Id = 0, CustomerName = reserve.CustName, ReservationDate = reserve.ReserveDate } };


                foreach (var seats in reserve.Reservation)
                {
                    allSeats.Add(seats.SeatNumber);
                    tempID = seats.SeatId;
                    movieName = seats.MovieName;
                    reservationStat = seats.ReservationStatusName;

                }
                joinSeats = string.Join(", ", allSeats);

                reser2.Add(new MovieReservationModel { Id = tempID, CustomerName = reserve.CustName, ReservationStatusName = reservationStat, SeatNumber = joinSeats, ReservationDate = reserve.ReserveDate, MovieName = movieName });

                allSeats.Clear();

            }

            return reser2;
        }

        public ActionResult AddMovie()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMovie(MovieModel movie, HttpPostedFileBase file)
        {
            try
            {
                
                if (ModelState.IsValid)
                {
                    Movie model = new Movie();
                    model.Title = movie.Title;
                    model.Cast = movie.Cast;
                    model.Category = movie.Category;
                    model.RunTime = movie.RunTime;

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~\\Content\\MovieCovers\\"));
                        file.SaveAs(path + fileName);

                        model.Image = fileName;
                    }
                    else
                        model.Image = null;
                    model.Synopsis = movie.Synopsis;
                    model.Price = movie.Price;
                    _database.Movies.Add(model);
                    _database.SaveChanges();
                }

                return RedirectToAction("AdminIndex", "Home");
            }
            catch
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(movie);
        }


        public ActionResult DeleteMovie(int id)
        {
            var movie = _database.Movies.Where(data => data.Id == id).FirstOrDefault();
            _database.Movies.Remove(movie);
            _database.SaveChanges();

            return RedirectToAction("AdminIndex");
        }

        [HttpGet]
        public ActionResult EditMovie(int id)
        {
            var model = _database.Movies.FirstOrDefault(data => data.Id == id);
            
            if (model == null)
            {
                return RedirectToAction("AdminIndex");
            }

            MovieModel movie = new MovieModel()
            {
                Id = model.Id,
                Title = model.Title,
                Synopsis = model.Synopsis,
                Category = model.Category,
                Cast = model.Cast,
                RunTime = model.RunTime,
                Price = (float)model.Price,
                Image = model.Image
            };

            return View(movie);
        }

        [HttpPost]
        public ActionResult EditMovie(MovieModel model, HttpPostedFileBase file)
        {
            var item = _database.Movies.FirstOrDefault(data => data.Id == model.Id);

            Movie movie = _database.Movies.Find(item.Id);

            if (ModelState.IsValid && movie != null)
            {
                movie.Title = model.Title;
                movie.Synopsis = model.Synopsis;
                movie.Category = model.Category;
                movie.Cast = model.Cast;
                movie.RunTime = model.RunTime;
                movie.Price = model.Price;

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~\\Content\\MovieCovers\\"));
                    file.SaveAs(path + fileName);

                    movie.Image = fileName;
                }
                else
                {
                    movie.Image = movie.Image;
                }
                
                _database.SaveChanges();
            }

            return RedirectToAction("AdminIndex");
        }
    }
}