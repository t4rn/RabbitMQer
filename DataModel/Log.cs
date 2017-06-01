namespace DataModel
{
    public class Log
    {
        public LogLevel Level { get; set; }

        public string Message { get; set; }

        public string Source { get; set; }

        public enum LogLevel
        {
            info, warning, error
        }
    }
}
