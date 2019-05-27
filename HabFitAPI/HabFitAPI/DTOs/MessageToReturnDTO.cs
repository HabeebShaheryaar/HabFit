using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HabFitAPI.DTOs
{
    public class MessageToReturnDTO
    {
        public string ID { get; set; }

        public string SenderId { get; set; }

        public string SenderKnownAs { get; set; }

        public string SenderPhotoUrl { get; set; }

        public string RecipientId { get; set; }

        public string RecipientKnownAs { get; set; }

        public string RecipientPhotoUrl { get; set; }

        public string Content { get; set; }

        public bool IsRead { get; set; }

        public DateTime? DateRead { get; set; }

        public DateTime MessageSent { get; set; }
    }
}
