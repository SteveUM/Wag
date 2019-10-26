namespace WagProject
{
    internal class LogEntry : BaseViewModel
    {
        string entry;

        public string Entry
        {
            get {
                return entry;
            }
            set {
                entry = value;
            }
        }
    }
}
