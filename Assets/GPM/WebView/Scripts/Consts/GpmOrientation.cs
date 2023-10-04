namespace Gpm.WebView
{
    public static class GpmOrientation
    {
        public const int UNSPECIFIED = 0;
        public const int PORTRAIT = 1;
        public const int PORTRAIT_REVERSE = 2;      // Android
        public const int PORTRAIT_UPSIDEDOWN = 2;   // iOS
        public const int LANDSCAPE_LEFT = 4;       // Android/iOS
        public const int LANDSCAPE_REVERSE = 8;     // Android
        public const int LANDSCAPE_RIGHT = 8;        // iOS
    }
}
