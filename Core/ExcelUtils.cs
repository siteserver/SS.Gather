using System.Collections.Generic;
using System.Data;
using System.IO;
using ExcelDataReader;

namespace SS.Gather.Core
{
    public static class ExcelUtils
    {
        public static List<string> GetContentUrls(string filePath)
        {
            var urls = new List<string>();

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx)
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    do
                    {
                        while (reader.Read())
                        {
                            var url = reader.GetString(0);
                            if (StringUtils.IsHttpUrl(url) && !urls.Contains(url))
                            {
                                urls.Add(url);
                            }
                        }
                    } while (reader.NextResult());
                }
            }

            return urls;
        }
    }
}