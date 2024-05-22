using System;
using Avalonia.Controls;

namespace DataStructuresProject.Views
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Invoked on every frame, acts like a graphical update loop for animations.
        /// </summary>
        public static event Action<TimeSpan>? UpdatedFrame;

        public MainWindow()
        {
            InitializeComponent();
            GetTopLevel(this)?.RequestAnimationFrame((TimeSpan delta) =>
            {
                RequestUpdate(delta);
            });
        }

        private void RequestUpdate(TimeSpan delta)
        {
            UpdatedFrame?.Invoke(delta);
            GetTopLevel(this)?.RequestAnimationFrame((TimeSpan delta) =>
            {
                RequestUpdate(delta);
            });
        }
    }
}
