using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace XPathSearch
{
    public class XPathNodeCounter
    {
        private readonly ConfigProvider _configProvider;
        private const string NotAvailable = "N/A";

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
                    .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                    .SelectMany(ProcessFile)
                    .ToLookup(s => s)
                    .Select(wordGroup => new WordCount { Key = wordGroup.Key, Count = wordGroup.Count() })
                    .OrderByDescending(arg => arg.Count);
        }

        private IEnumerable<string> ProcessFile(string filePath)
        {
            var doc = new XmlDocument();
            doc.Load(filePath);
            var namespaceManager = new XmlNamespaceManager(doc.NameTable);
            namespaceManager.AddNamespace("a", "x-schema:mxschema://docsearch");
            XmlNodeList nodes = doc.SelectNodes(_configProvider.XPath, namespaceManager);
            if (nodes == null || nodes.Count == 0)
            {
                yield return NotAvailable;
                yield break;
            }
            foreach (object node in nodes)
            {
                yield return ((XmlNode)node).InnerText;
            }
        }
    }
}