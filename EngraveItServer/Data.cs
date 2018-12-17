namespace EngraveItServer
{
    // Contains runtime data. This class should be thread-safe.
    public class Data
    {
        private int _maxRandom = 1000;
        private object _maxRandomLock = new object();

        public int MaxRandom
        {
            get
            {
                lock (_maxRandomLock)
                {
                    return _maxRandom;
                }
            }

            set
            {
                lock (_maxRandomLock)
                {
                    _maxRandom = value;
                }
            }
        }
    }
}
