using System;


namespace DataProvider.Cal
{
    public class CalSessionInfo
    {
        public string AspSessionId { get; set; }
        public string ShivokInteger { get; set; }
        public string AspAuth { get; set; }

        public string ViewState { get; set; }
        public string ViewStateGenerator { get; set; }
        public string EventValidation { get; set; }
    }
}
