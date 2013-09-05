using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace XPathSearch
{
    public class XPathNodeCounter
    {
        private readonly ConfigProvider _configProvider;
        private const string NotAvailable = "N/A";
        private const string NamespaceRegex = @"(xmlns:?[^=]*=[""][^""]*[""])";

        public XPathNodeCounter(ConfigProvider configProvider)
        {
            _configProvider = configProvider;
        }

        public IOrderedEnumerable<WordCount> GetNodeValueCountsOrdered()
        {
            return Directory
                    .GetFiles(_configProvider.DirectoryPath)
                    .AsParallel()
                    .WithDegreeOfParallelism(_configProvider.DesiredThreadCount)
                    .SelectMany(ProcessFile)
                    .ToLookup(word => word)
                    .Select(wordGroup => new WordCount { Key = wordGroup.Key, Count = wordGroup.Count() })
                    .OrderByDescending(arg => arg.Count);
        }

        private IEnumerable<string> ProcessFile(string filePath)
        {
            var doc = LoadXmlDocument(filePath);
            XmlNodeList nodes = doc.SelectNodes(_configProvider.XPath);
            if (nodes == null || nodes.Count == 0)
            {
                yield return NotAvailable;
                yield break;
            }
            foreach (XmlNode node in nodes)
            {
                yield return node.InnerText;
            }
        }
        private XmlDocument LoadXmlDocument(string filePath)
        {
            string fileText = File.ReadAllText(filePath);
            string withoudNs = Regex.Replace(fileText, NamespaceRegex, "", 
                                RegexOptions.IgnoreCase | RegexOptions.Multiline);
            var doc = new XmlDocument();
            doc.LoadXml(withoudNs);
            return doc;
        }
    }
}