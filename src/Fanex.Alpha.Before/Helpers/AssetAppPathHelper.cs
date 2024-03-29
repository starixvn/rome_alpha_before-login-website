using Fanex.Alpha.Before.Configurations;

namespace Fanex.Alpha.Before.Helpers
{
    public static class AssetAppPathHelper
    {
        private static string[] assetAppPathsArray;

        private static string[] dcsStaticAppPathsArray;

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

        public static string[] DCSStaticAppPathsArray
        {
            get
            {
                dcsStaticAppPathsArray ??= AppSetting.DCSStaticAppPaths
                    .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(path => path.Trim())
                    .ToArray();

                return dcsStaticAppPathsArray;
            }
        }

        public static bool IsValidDCSStaticAppPath(string appPath)
            => DCSStaticAppPathsArray.Length > 0
            && !string.IsNullOrWhiteSpace(appPath)
            && DCSStaticAppPathsArray.Any(path => path.Equals(appPath, StringComparison.OrdinalIgnoreCase));


        public static bool IsValidAssetAppPath(string assetAppPath)
            => AssetAppPathsArray.Length > 0
            && !string.IsNullOrWhiteSpace(assetAppPath)
            && AssetAppPathsArray.Any(path => path.Equals(assetAppPath, StringComparison.OrdinalIgnoreCase));
    }
}
