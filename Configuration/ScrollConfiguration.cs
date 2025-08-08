namespace MarbleScroll.Configuration
{
    internal sealed class ScrollConfiguration
    {
        public const uint BackButton = 1;
        public const uint ForwardButton = 2;

        public int HorizontalSensitivity { get; } = 200;
        public int HorizontalDistance { get; } = 200;
        public int VerticalSensitivity { get; } = 50;
        public int VerticalDistance { get; } = 200;
    }
}