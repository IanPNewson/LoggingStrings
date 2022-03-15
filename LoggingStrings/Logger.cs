using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggingStrings
{
    public class Logger : ILogger
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
            Log(str, str.Labels, null);
            return str;
        }

        private void Log(string log, Dictionary<string,object> logValues, Action<BsonDocument>? withLogDocument)
        {
            var bsonDoc = new BsonDocument();

            bsonDoc.Add(new BsonElement("Message", new BsonString(log)));
            bsonDoc.Add(new BsonElement("Time", new BsonDateTime(DateTime.Now)));

            var values = new BsonDocument();
            foreach (var kvp in logValues)
            {
                var propName = CleanPropertyName(kvp.Key);
                if (string.IsNullOrWhiteSpace(propName)) continue;

                values.Add(new BsonElement(propName, ToBsonValue(kvp.Value)));
            }

            bsonDoc.Add("Values", values);

            withLogDocument?.Invoke(bsonDoc);

            _col.InsertOne(bsonDoc);
        }

        private static BsonValue ToBsonValue(object value)
        {
            if (value == null)
                return BsonNull.Value;

            switch (value)
            {
                case int:
                case float:
                case long:
                case bool:
                case double:
                case string:
                case DateTime:
                    return BsonValue.Create(value);
                default:
                    return new BsonString(value.ToString());
            }
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

        public ILogSection LogTimeTaken(LoggingStringInterpolator str)
        {
            return new TimeTakenSection(this, str);
        }

        private struct TimeTakenSection : ILogSection
        {
            private Action _onDispose;

            public TimeTakenSection(Logger logger, LoggingStringInterpolator str)
            {
                string log = str;
                var logValues = str.Labels;

                var sw = new Stopwatch();
                sw.Start();
                _onDispose = () =>
                {
                    var elapsed = sw.ElapsedMilliseconds;

                    logger.Log(log, logValues, log =>
                    {
                        log.Add("TimeTakenMs", new BsonInt64(elapsed));
                    });
                };
            }

            public void Dispose()
            {
                _onDispose();
            }
        }
    }

}
