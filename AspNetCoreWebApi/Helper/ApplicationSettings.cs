namespace AspNetCoreWebApi
{
    public class ApplicationSettings {
        public Logging Logging {get; set;}
        public Authentication Authentication {get; set;}
        public string AllowedHosts {get; set;}
    }
    public class LogLevel { 
        public string Default {get; set;}
    }

    public class Logging {
        public LogLevel LogLevel {get; set;}
    }

    public class Authentication  {
        public string Name {get; set;}
        public string Key {get; set;}
    }
}