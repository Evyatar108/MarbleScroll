namespace MarbleScroll.Core
{
    using System;
    using System.Threading.Tasks;
    using MarbleScroll.Configuration;
    using MarbleScroll.Interop;
    using MarbleScroll.Models;

    internal class ScrollProcessor
    {
        private readonly ScrollConfiguration config = new();
        private bool suppressButtonClick = false;
        private ScrollPosition startPosition;
        private ScrollDelta scrollDelta;

        public void InitializeScroll(MSLLHOOKSTRUCT hookStruct)
        {
            suppressButtonClick = false;
            startPosition = new ScrollPosition(hookStruct.pt.x, hookStruct.pt.y);
            scrollDelta = new ScrollDelta();
        }

        public bool ProcessMouseMove(MSLLHOOKSTRUCT hookStruct)
        {
            UpdateScrollDeltas(hookStruct);
            ProcessScrolling();
            return true;
        }

        public bool ShouldSuppressClick()
        {
            return suppressButtonClick;
        }

        private void UpdateScrollDeltas(MSLLHOOKSTRUCT hookStruct)
        {
            // Calculate delta from start position (like original implementation)
            var deltaX = hookStruct.pt.x - startPosition.X;
            var deltaY = hookStruct.pt.y - startPosition.Y;
            scrollDelta = scrollDelta.Add(deltaX, deltaY);
        }

        private void ProcessScrolling()
        {
            if (ShouldScrollHorizontally())
            {
                var direction = CalculateHorizontalDirection();
                ResetVerticalDelta();
                ExecuteHorizontalScroll(direction);
                suppressButtonClick = true;
            }

            if (ShouldScrollVertically())
            {
                var direction = CalculateVerticalDirection();
                ExecuteVerticalScroll(direction);
                suppressButtonClick = true;
            }
        }

        private bool ShouldScrollHorizontally() => Math.Abs(scrollDelta.X) > config.HorizontalSensitivity;
        private bool ShouldScrollVertically() => Math.Abs(scrollDelta.Y) > config.VerticalSensitivity;

        private int CalculateHorizontalDirection()
        {
            var direction = scrollDelta.X < 0 ? -config.HorizontalDistance : config.HorizontalDistance;
            scrollDelta = scrollDelta.AdjustHorizontal(config.HorizontalSensitivity);
            return direction;
        }

        private int CalculateVerticalDirection()
        {
            var direction = scrollDelta.Y > 0 ? -config.VerticalDistance : config.VerticalDistance;
            scrollDelta = scrollDelta.AdjustVertical(config.VerticalSensitivity);
            return direction;
        }

        private void ResetVerticalDelta()
        {
            scrollDelta = scrollDelta.ResetVertical();
        }

        private static void ExecuteHorizontalScroll(int direction)
        {
            Task.Run(() => WindowsApi.mouse_event((uint)MouseEvents.HWHEEL, 0, 0, direction, UIntPtr.Zero));
        }

        private static void ExecuteVerticalScroll(int direction)
        {
            Task.Run(() => WindowsApi.mouse_event((uint)MouseEvents.WHEEL, 0, 0, direction, UIntPtr.Zero));
        }
    }
}