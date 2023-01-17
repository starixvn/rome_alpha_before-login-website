namespace Fanex.Alpha.Before.NetThrottling.Models;

public class CacheIdentity
{
    public string ThrottleKey { get; private set; }

    public string BlockKey { get; private set; }

    public CacheIdentity(string throttleKey, string blockKey)
    {
        ThrottleKey = throttleKey;
        BlockKey = blockKey;
    }

    public static CacheIdentity From(string policyName, string siteGroup, string[] keys)
    {
        if (keys == null || !keys.Any())
        {
            throw new ArgumentNullException(nameof(keys));
        }

        var prefix = $"{policyName}}}:{{{siteGroup}}}";
        var suffix = $"{{{string.Join("_", keys)}";
        var throttleKey = $"{prefix}:{{{Constants.Throttle}}}:{suffix}";
        var blockKey = $"{prefix}:{{{Constants.Block}}}:{suffix}";
        return new CacheIdentity(throttleKey, blockKey);
    }
}