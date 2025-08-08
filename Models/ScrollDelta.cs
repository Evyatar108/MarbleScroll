namespace MarbleScroll.Models
{
    internal readonly record struct ScrollDelta(int X = 0, int Y = 0)
    {
        public ScrollDelta Add(int deltaX, int deltaY) => new(X + deltaX, Y + deltaY);
        public ScrollDelta AdjustHorizontal(int sensitivity) => new(X < 0 ? X + sensitivity : X - sensitivity, Y);
        public ScrollDelta AdjustVertical(int sensitivity) => new(X, Y > 0 ? Y - sensitivity : Y + sensitivity);
        public ScrollDelta ResetVertical() => new(X, 0);
    }
}