namespace Fanex.Alpha.Before.Constants;

public enum BotFlagged
{
    IsClean = 0,
    IsBrowserless = 1,
    IsGoogleV3Bot = 2,
    IsInvalidGoogleV3 = 4,
    IsHeadless = 8,
    IsFpsGogless = 16,
    IsInvalidDevice = 32,
    IsBlackDevice = 64
}