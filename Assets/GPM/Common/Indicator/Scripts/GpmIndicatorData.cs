using System.Collections.Generic;

namespace Gpm.Common.Indicator
{
    public class GpmIndicatorData
    {
        private const string KEY_ACTION = "ACTION";
        private const string KEY_ACTION_DETAIL = "ACTION_DETAIL_";

        private Dictionary<string, string> dictionary;
        private int index = 1;

        public GpmIndicatorData(string action)
        {
            dictionary = new Dictionary<string, string>();
            dictionary.Add(KEY_ACTION, action);
        }

        public int AddActionDetail(string actionDetail)
        {
            dictionary.Add(string.Format("{0}{1}", KEY_ACTION_DETAIL, index), actionDetail);
            return index++;
        }

        public Dictionary<string, string> ToDictionary()
        {
            return dictionary;
        }
    }
}