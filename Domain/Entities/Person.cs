using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Person
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string AddressOrDepartment { get; set; } = string.Empty;
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
