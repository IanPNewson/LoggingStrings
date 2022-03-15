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

        private const string _validCharsLower = @"abcdefghijklmnopqrstuvwxyz";
        private static readonly string _validChars = _validCharsLower + _validCharsLower.ToUpper();

        public Logger()
        {
            _col = new MongoDB.Driver.MongoClient("mongodb://localhost")
                .GetDatabase("LoggingStringTest")
                .GetCollection<BsonDocument>("Logs");
        }

        public LoggingStringInterpolator Log(LoggingStringInterpolator str)
        {
            var bsonDoc = new BsonDocument();

            bsonDoc.Add(new BsonElement("Message", new BsonString(str)));
            bsonDoc.Add(new BsonElement("Time", new BsonDateTime(DateTime.Now)));

            var values = new BsonDocument();
            foreach (var kvp in str.Labels)
            {
                var propName = CleanPropertyName(kvp.Key);
                if (string.IsNullOrWhiteSpace(propName)) continue;

                values.Add(new BsonElement(propName, BsonValue.Create(kvp.Value)));
            }

            bsonDoc.Add("Values", values);

            _col.InsertOne(bsonDoc);
            return str;
        }

        private static string CleanPropertyName(string name)
        {
            var start = 0;
            while (!char.IsLetterOrDigit(name[start]))
                ++start;

            var end = name.Length - 1;
            while (!char.IsLetterOrDigit(name[end]))
                --end;

            return name.Substring(start, end - start + 1);
        }

    }

}
