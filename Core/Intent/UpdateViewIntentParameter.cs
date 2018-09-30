using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Foxpict.Client.Sdk.Intent
{
    public class UpdateViewIntentParameter
    {
        public string ScreenName { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public UpdateType UpdateType { get; set; }
    }

    public enum UpdateType
    {
        SET, PUSH, POP
    }
}
