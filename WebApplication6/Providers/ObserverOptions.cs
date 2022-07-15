namespace WebApplication6.Providers
{
    public class ObserverOptions
    {
        public int PhotoDeviceId { get; set; }
        public int ObserveTimeoutInMinutes { get; set; }

        public string OutsidePort { get; set; }

        public string Smpt { get; set; }

        public int SmtpPort { get; set; }

        public string Login { get; set; }

        public string Password { get; set; } 

        public string ObserverEmail { get; set; }

        public int Threshold { get; set; }

        public int NonBlack { get; set; }

    }
}