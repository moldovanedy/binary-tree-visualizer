using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using BinaryTreeVisualizer.Graphics;

namespace BinaryTreeVisualizer.Views.Dialogs
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

        public async Task OpenDialog(Controls dialogs)
        {
            _dialogRef = dialogs;
            this.SizeChanged += this.RemoveNodeDialog_SizeChanged;

            dialogs.Add(this);
            await FadeInDialog();
            KeyInput.Focus();
        }

        public async Task CloseDialog(Controls dialogs)
        {
            await FadeOutDialog();
            dialogs.Remove(this);
            this.SizeChanged -= this.RemoveNodeDialog_SizeChanged;

            _dialogRef = null;
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
            CancelButton.IsEnabled = false;
            await CloseDialog(_dialogRef!);
        }

        private async void RemoveButton_Click(object? sender, RoutedEventArgs e)
        {
            if (KeyInput.Value != null)
            {
                bool success = CanvasController.DeleteNode((int)KeyInput.Value.Value);
                RemoveButton.IsEnabled = false;
                await CloseDialog(_dialogRef!);

                if (!success)
                {
                    if (Application.Current!.TryGetResource("Strings.RemoveFailed", out object? errorMessage))
                    {
                        await Snackbar.ShowSnackbar(
                            MainView.Instance!.MainCanvas,
                            (errorMessage!.ToString() ?? string.Empty) + "!",
                            Snackbar.MessageSeverity.Error);
                    }
                }
            }
        }

        private async void Grid_KeyUp(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                await CloseDialog(_dialogRef!);
            }
            if (e.Key == Key.Enter)
            {
                RemoveButton_Click(null, new RoutedEventArgs());
            }
        }
    }
}
