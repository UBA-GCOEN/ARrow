namespace Gpm.Communicator
{
    public class GpmCommunicatorVO
    {
        public class Configuration
        {            
            public string className;
        }

        public class Message
        {
            public string domain;
            public string data;
            public string extra;
        }
    }
}
