namespace ConsoleApp1
{
    using System;
    using System.Text.RegularExpressions;

    public static class JsonExtensions
    {
        /// <summary>
        /// 仅解码 Unicode 转义序列 (\uXXXX)，保留其他 JSON 转义字符
        /// </summary>
        /// <param name="str">JSON 字符串</param>
        /// <returns>解码后的字符串</returns>
        public static string DecodeUnicodeOnly(this string str)
        {
            return Regex.Replace(
                str,
                @"\\u([0-9a-fA-F]{4})",
                match => char.ConvertFromUtf32(Convert.ToInt32(match.Groups[1].Value, 16))
            );
        }

        /// <summary>
        /// 对 JSON 字符串进行解码，包括 Unicode 转义序列
        /// </summary>
        /// <param name="json">JSON 字符串</param>
        /// <returns>解码后的 JSON 字符串</returns>
        public static string DecodeUnicodeInJson(this string json)
        {
            // 解析 JSON 为对象
            var parsedObject = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            // 遍历对象并解码 Unicode
            var decodedObject = new Dictionary<string, object>();
            if (parsedObject != null)
            {
                foreach (var kv in parsedObject)
                {
                    if (kv.Value is string strValue)
                    {
                        decodedObject[kv.Key] = DecodeUnicode(strValue);
                    }
                    else
                    {
                        decodedObject[kv.Key] = kv.Value;
                    }
                }
            }

            // 序列化为 JSON 字符串
            return System.Text.Json.JsonSerializer.Serialize(decodedObject);
        }

        /// <summary>
        /// 解码字符串中的 Unicode 转义序列 (\uXXXX)
        /// </summary>
        /// <param name="str">输入字符串</param>
        /// <returns>解码后的字符串</returns>
        private static string DecodeUnicode(string str)
        {
            return Regex.Replace(
                str,
                @"\\u([0-9a-fA-F]{4})",
                match => char.ConvertFromUtf32(Convert.ToInt32(match.Groups[1].Value, 16))
            );
        }
    }

    class Program
    {
        static void Main()
        {
            var obj = new
            {
                Directory = "Business\\Line1",
                Name = "Test\\Name",
                Description = "Unicode\u4e2d\u6587"
            };

            string jsonString = System.Text.Json.JsonSerializer.Serialize(obj);
            Console.WriteLine("Original JSON: " + jsonString);

            // 解码 Unicode 转义序列
            string decodedString = jsonString.DecodeUnicodeOnly();
            Console.WriteLine("Decoded JSON: " + decodedString);
        }
    }


}
