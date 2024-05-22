using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using DataStructuresProject.Graphics;

namespace DataStructuresProject.Views.Dialogs
{
    public partial class RemoveNodeDialog : UserControl, IDialog
    {
        private Controls? _dialogRef;

        private static DialogProperties DialogProperties { get; set; } = new DialogProperties()
        {
            MinHeight = 100,
            MinWidth = 150,
            MaxHeight = 300,
            MaxWidth = 450,
            WidthPercent = 1,
            HeightPercent = 0.6,
        };

        public RemoveNodeDialog()
        {
            InitializeComponent();
        }

        private void RemoveNodeDialog_SizeChanged(object? sender, SizeChangedEventArgs e)
        {
            ((IDialog)this).Draw(Root, Root.Bounds.Width, Root.Bounds.Height, DialogProperties);
        }

        public async Task<object?> OpenDialog(Controls dialogs)
        {
            _dialogRef = dialogs;
            this.SizeChanged += this.RemoveNodeDialog_SizeChanged;

            dialogs.Add(this);
            await FadeInDialog();
            return null;
        }

        public async Task CloseDialog(Controls dialogs)
        {
            await FadeOutDialog();
            dialogs.Remove(this);
            this.SizeChanged -= this.RemoveNodeDialog_SizeChanged;

            _dialogRef = null;
            return;
        }

        private async Task FadeInDialog()
        {
            await Task.Delay(5);
            Root.Background = new SolidColorBrush(0x80_00_00_00);

            await Task.Delay(5);
            Body.Opacity = 1;

            await Task.Delay(IDialog.ANIMATION_DURATION);
        }

        private async Task FadeOutDialog()
        {
            await Task.Delay(5);
            Root.Background = new SolidColorBrush(0x00_00_00_00);

            await Task.Delay(5);
            Body.Opacity = 0;

            await Task.Delay(IDialog.ANIMATION_DURATION);
        }

        private async void CancelButton_Click(object? sender, RoutedEventArgs e)
        {
            await CloseDialog(_dialogRef!);
        }

        private async void RemoveButton_Click(object? sender, RoutedEventArgs e)
        {
            if (KeyInput.Value != null)
            {
                CanvasController.DeleteNode((int)KeyInput.Value.Value);
                await CloseDialog(_dialogRef!);
            }
        }
    }
}
