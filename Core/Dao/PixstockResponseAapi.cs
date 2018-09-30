using Foxpict.Common.Core;
using Newtonsoft.Json;

namespace Foxpict.Client.Sdk.Dao
{
    public class PixstockResponseAapi<T> : ResponseAapi<T>
    {
        public RT GetRelative<RT>(string key)
        {
            return JsonConvert.DeserializeObject<RT>(this.Rel[key]);
        }
    }
}
