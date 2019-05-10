using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HabFitAPI.Entities
{
    public class Like
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }

        /// <summary>
        /// User likes another user
        /// </summary>
        public string LikerID { get; set; }

        /// <summary>
        /// User being liked by another user
        /// </summary>
        public string LikeeID { get; set; }

        //public Users Liker { get; set; }
        //public Users Likee { get; set; }
    }
}
