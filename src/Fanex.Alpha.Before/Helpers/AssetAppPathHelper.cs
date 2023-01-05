using Fanex.Alpha.Before.Configurations;

namespace Fanex.Alpha.Before.Helpers
{
    public static class AssetAppPathHelper
    {
        private static string[] assetAppPathsArray;
        public static string[] AssetAppPathsArray
        {
            get
            {
                assetAppPathsArray = assetAppPathsArray ?? AppSetting.AssetAppPaths
                    .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(path => path.Trim())
                    .ToArray();

                return assetAppPathsArray;
            }
        }

        public static bool IsValidAssetAppPath(string assetAppPath)
            => AssetAppPathsArray.Length > 0
            && !string.IsNullOrWhiteSpace(assetAppPath)
            && AssetAppPathsArray.Any(path => path.Equals(assetAppPath, StringComparison.OrdinalIgnoreCase));
    }
}
