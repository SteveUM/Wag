namespace WagLib
{
    internal class LogEntry : BaseViewModel
    {
        public LogEntry()
        {
            SearchString = "line 1";
        }

        string entry;

        public string Entry
        {
            get
            {
                return entry;
            }
            set
            {
                entry = value;
            }
        }


        public static string SearchString { get; set; }

        private bool match;

        public bool Match
        {
            get
            {
                if (SearchString.Equals(string.Empty))
                {
                    return false;
                }
                else
                {
                    return Entry.Contains(SearchString);
                }
            }
            set { match = value; }
        }


    }
}
