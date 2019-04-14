using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace HabFitAPI.Entities
{
    public class Photo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }

        public string URL { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsMain { get; set; }
        [BsonElement("UserID")]
        public string UserID { get; set; }
    }
}