using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggingStrings
{
    public class Logger
    {
        private IMongoCollection<BsonDocument> _col;

        public Logger()
        {
            _col = new MongoDB.Driver.MongoClient("mongodb://localhost")
                .GetDatabase("LoggingStringTest")
                .GetCollection<BsonDocument>("Logs");
        }

        public LoggingStringInterpolator Log(LoggingStringInterpolator str)
        {
            var jsonDoc = Newtonsoft.Json.JsonConvert.SerializeObject(str.Labels);
            var bsonDoc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(jsonDoc);
            _col.InsertOne(bsonDoc);
            return str;
        }

    }
}
