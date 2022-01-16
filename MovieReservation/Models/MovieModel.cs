using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MovieReservation.Models
{
    public class MovieModel
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Cast { get; set; }
        public string Category { get; set; }
        public string RunTime { get; set; }
        public string Synopsis { get; set; }
        public float Price { get; set; }
    }
}