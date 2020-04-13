namespace CaptchaSharp.Services.DeCaptcher
{
    internal class DeCaptcherResponse
    {
        public int ResultCode { get; set; }
        public int MajorID { get; set; }
        public int MinorID { get; set; }
        public int Type { get; set; }
        public int Timeout { get; set; }
        public string Text { get; set; }

        public static DeCaptcherResponse Parse(string str)
        {
            // ResultCode|MajorID|MinorID|Type|Timeout|Text
            // 0|107|44685|0|0|n7hjks
            var split = str.Split(new char[] { '|' }, 6);
            return new DeCaptcherResponse
            {
                ResultCode = int.Parse(split[0]),
                MajorID = int.Parse(split[1]),
                MinorID = int.Parse(split[2]),
                Type = int.Parse(split[3]),
                Timeout = int.Parse(split[4]),
                Text = split[5]
            };
        }

        public static bool TryParse(string str, out DeCaptcherResponse response)
        {
            try
            {
                response = Parse(str);
                return true;
            }
            catch
            {
                response = null;
                return false;
            }
        }
    }
}
