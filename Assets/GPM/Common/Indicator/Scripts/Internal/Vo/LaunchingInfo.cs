namespace Gpm.Common.Indicator.Internal
{
    public class LaunchingInfo
    {
        public class Header
        {
            public bool isSuccessful;
            public int resultCode;
            public string resultMessage;
        }

        public class Launching
        {
            public class Indicator
            {
                public class Zone
                {
                    public string logVersion;
                    public string appKey;
                    public string url;
                    public string activation;
                }

                public Zone alpha;
                public Zone real;

                public Indicator()
                {
                    alpha = new Zone();
                    real = new Zone();
                }
            }

            public Indicator indicator;

            public Launching()
            {
                indicator = new Indicator();
            }
        }

        public Header header;
        public Launching launching;

        public LaunchingInfo()
        {
            header = new Header();
            launching = new Launching();
        }
    }
}