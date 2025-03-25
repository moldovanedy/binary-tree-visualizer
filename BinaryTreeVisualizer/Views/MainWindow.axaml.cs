using System;
using Avalonia.Controls;

namespace BinaryTreeVisualizer.Views
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Invoked on every frame, acts like a graphical update loop for animations.
        /// </summary>
        public static event Action<TimeSpan>? UpdatedFrame;

        private static TimeSpan _lastDelta = new TimeSpan(0, 0, 0);

        public MainWindow()
        {
            InitializeComponent();
            GetTopLevel(this)?.RequestAnimationFrame(RequestUpdate);
        }

        private void RequestUpdate(TimeSpan delta)
        {
            UpdatedFrame?.Invoke(delta - _lastDelta);
            _lastDelta = delta;

            //callback
            GetTopLevel(this)?.RequestAnimationFrame(RequestUpdate);
        }
    }
}
