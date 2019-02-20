using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GYM.Models
{
    public class GymClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan Durartion { get; set; }
        public DateTime EndTime => StartTime + Durartion;
        public string Description { get; set; }

        public ICollection<ApplicationUserGymClass> AttendingMembers { get; set; }
    }
}
