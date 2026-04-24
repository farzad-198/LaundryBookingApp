using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class LaundryRoom
    {
        public Guid Id { get; set; }
        public string Name { get; set; }= string.Empty;
        public string Location { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public bool HasDryer { get; set; }
        public bool HasIron { get; set; }
        public string Description { get; set; } = string.Empty;
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
