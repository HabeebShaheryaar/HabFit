using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HabFitAPI.DTOs
{
    public class PhotoForReturnDTO
    {
        public string ID { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }
    }
}
