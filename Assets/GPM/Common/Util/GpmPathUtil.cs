using System.IO;
using System.Text;

namespace Gpm.Common.Util
{
    public static class GpmPathUtil
    {
        public static string Combine(params string[] path)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < path.Length; i++)
            {
                if (i == 0)
                {
                    builder.Append(path[i]);
                }
                else
                {
                    builder.AppendFormat("{0}{1}", Path.DirectorySeparatorChar, path[i].Trim('/'));
                }
            }

            return builder.ToString();
        }
        
        public static string UrlCombine(params string[] path)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < path.Length; i++)
            {
                string pathPart = path[i].Trim('/');

                if (i == 0)
                {
                    builder.Append(pathPart);
                }
                else
                {
                    builder.AppendFormat("/{0}", pathPart);
                }
            }

            return builder.ToString();
        }
    }
}