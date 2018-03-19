namespace Fanex.Alpha.Before.Common.Tests.Helpers
{
    using System;
    using System.Data;
    using System.IO;
    using System.Reflection;

    public static class FileHelper
    {
        public static string GetFilePath(string relativeFilePath)
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);

            return Path.GetDirectoryName(path) + relativeFilePath;
        }

        public static DataSet LoadDataSet(string filePath)
        {
            var dataSet = new DataSet();
            dataSet.ReadXml(GetFilePath(filePath));

            return dataSet;
        }
    }
}
