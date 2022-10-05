namespace Fanex.Alpha.Before.Common.ActivityTracking
{
    using Fanex.ActivityTracking.Services;
    using Newtonsoft.Json;

    public class JsonConverter : IJsonConverter
    {
        public string SerializeObject(object value)
            => JsonConvert.SerializeObject(
                value,
                new JsonSerializerSettings
                {
                    Formatting = Formatting.None,
                    NullValueHandling = NullValueHandling.Ignore
                });
    }
}
