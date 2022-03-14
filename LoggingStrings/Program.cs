using LoggingStrings;

var logger = new Logger();
logger.Log($"Now: {DateTime.Now:HH:mm}, Yesterday: {DateTime.Now.AddDays(-1)}");
