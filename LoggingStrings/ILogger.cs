namespace LoggingStrings
{
    public interface ILogger
    {
        LoggingStringInterpolator Log(LoggingStringInterpolator str);

        ILogSection LogTimeTaken(LoggingStringInterpolator str);

    }

}
