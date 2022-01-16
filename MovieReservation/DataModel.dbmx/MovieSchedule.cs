//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MovieReservation.DataModel.dbmx
{
    using System;
    using System.Collections.Generic;
    
    public partial class MovieSchedule
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MovieSchedule()
        {
            this.SeatAvailabilityPerScheds = new HashSet<SeatAvailabilityPerSched>();
        }
    
        public int Id { get; set; }
        public Nullable<System.DateTime> ScheduleDate { get; set; }
        public Nullable<System.DateTime> ScheduledTime { get; set; }
        public Nullable<System.DateTime> Schedule { get; set; }
        public int MovieId { get; set; }
    
        public virtual Movie Movie { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SeatAvailabilityPerSched> SeatAvailabilityPerScheds { get; set; }
    }
}