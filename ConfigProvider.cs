using System.Collections.Specialized;
using System.Configuration;

namespace XPathSearch
{
    public class ConfigProvider
    {
        private readonly NameValueCollection _configs = ConfigurationManager.AppSettings;

        public string XPath
        {
            get { return _configs["xpath"]; }
        }

        public string DirectoryPath
        {
            get { return _configs["directory"]; }
        }

        public int DesiredThreadCount
        {
            get { return int.Parse(_configs["threadCount"]); }
        }
    }
}