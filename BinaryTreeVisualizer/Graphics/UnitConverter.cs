using System.Runtime.CompilerServices;

namespace BinaryTreeVisualizer.Graphics
{
    public static class UnitConverter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ConvertPixelsToVirtualUnits(double pixels)
        {
            return pixels / 100.0 / CanvasController.Zoom;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ConvertVirtualUnitsToPixels(double virtualUnits)
        {
            return virtualUnits * 100.0 * CanvasController.Zoom;
        }
    }
}
