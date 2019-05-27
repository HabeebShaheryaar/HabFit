using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HabFitAPI.DTOs
{
    public class MessageForCreationDTO
    {
        public string SenderId { get; set; }
        public string RecipientId { get; set; }
        public string Content { get; set; }
        public DateTime MessageSent { get; set; }

        public MessageForCreationDTO()
        {
            MessageSent = DateTime.Now;
        }
    }
}
