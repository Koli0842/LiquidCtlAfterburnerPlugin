using System.Collections.Generic;
using System.Globalization;

namespace LiquidCtlAfterburnerPlugin
{
    public class LiquidctlStatusJSON
    {
        public class StatusRecord
        {
            public string key { get; set; }
            public string value { get; set; }
            public string unit { get; set; }

            public float? GetValueAsFloat() {
                if (float.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out float valueAsFloat))
                    return valueAsFloat;
                return null;
            }
        }
        public string bus { get; set; }
        public string address { get; set; }

        public string description { get; set; }

        public List<StatusRecord> status { get; set; }
    }
}
