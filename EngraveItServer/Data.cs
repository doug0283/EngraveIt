namespace EngraveItServer
{
    // Contains runtime data. This class should be thread-safe.
    public class Data
    {
        private int _maxRandom = 1000;
        private object _maxRandomLock = new object();

        private Customer _customer = new Customer();
        private object _customerLock = new object();

        public Customer customer
        {
            get
            {
                lock (_customerLock)
                {
                    return _customer;
                }
            }

            set
            {
                lock (_maxRandomLock)
                {
                    _customerLock = value;
                }
            }
        }

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
