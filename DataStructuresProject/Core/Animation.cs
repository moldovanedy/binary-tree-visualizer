using System;
using System.Threading.Tasks;
using DataStructuresProject.Views;

namespace DataStructuresProject.Core
{
    public class Animation<T>
    {
        public Animation(T startValue, T endValue)
        {
            MainWindow.UpdatedFrame += Update;
        }

        ~Animation()
        {
            MainWindow.UpdatedFrame -= Update;
        }

        public async Task Animate(double duration, double delay, EasingType easingType, Action<T> frameCallback)
        {
            delay = Math.Abs(delay);
            if (Math.Round(delay, 2) > 0.01)
            {
                await Task.Delay((int)(delay * 1000));
            }

            await Task.Delay((int)(duration * 1000));
        }

        private void Update(TimeSpan delta)
        {
        }
    }

    [Flags]
    public enum EasingType
    {
        Linear = 0,
        Sine = 1,
        Quadratic = 2,
        Cubic = 3,

        EaseIn = 0b00100000_00000000,
        EaseOut = 0b01000000_00000000,
        EaseInOut = 0b10000000_00000000,
    }
}
