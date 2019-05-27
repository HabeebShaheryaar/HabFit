using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HabFitAPI.Entities
{
    public class Message
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }

        public string SenderID { get; set; }

        public Users Sender { get; set; }

        public string RecipientID { get; set; }

        public Users Recipient { get; set; }

        public string Content { get; set; }

        public bool IsRead { get; set; }

        public DateTime? DateRead { get; set; }

        public DateTime MessageSent { get; set; }

        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }
    }
}
