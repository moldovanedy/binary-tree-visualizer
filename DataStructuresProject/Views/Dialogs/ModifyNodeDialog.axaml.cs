using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using DataStructuresProject.Graphics;

namespace DataStructuresProject.Views.Dialogs
{
    public partial class ModifyNodeDialog : UserControl, IDialog
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

        public ModifyNodeDialog()
        {
            InitializeComponent();
        }

        private void ModifyNodeDialog_SizeChanged(object? sender, RoutedEventArgs e)
        {
            ((IDialog)this).Draw(Root, Root.Bounds.Width, Root.Bounds.Height, DialogProperties);
        }

        public async Task OpenDialog(Controls dialogs)
        {
            _dialogRef = dialogs;
            this.SizeChanged += this.ModifyNodeDialog_SizeChanged;

            dialogs.Add(this);
            await FadeInDialog();
            OldKeyInput.Focus();
        }

        public async Task CloseDialog(Controls dialogs)
        {
            await FadeOutDialog();
            dialogs.Remove(this);
            this.SizeChanged -= this.ModifyNodeDialog_SizeChanged;

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
            CancelButton.IsEnabled = false;
            await CloseDialog(_dialogRef!);
        }

        private async void ModifyButton_Click(object? sender, RoutedEventArgs e)
        {
            if (
                OldKeyInput.Value != null &&
                NewKeyInput.Value != null &&
                OldKeyInput.Value != NewKeyInput.Value)
            {
                bool success = CanvasController.DeleteNode((int)OldKeyInput.Value.Value);
                if (!success)
                {
                    goto ErrorHandling;
                }
                success = CanvasController.AddNode((int)NewKeyInput.Value.Value);
                if (!success)
                {
                    goto ErrorHandling;
                }
            }
            ModifyButton.IsEnabled = false;
            await CloseDialog(_dialogRef!);

            Core.BinarySearchTreeNode? node = CanvasController.PhysicalItems.Search((int)(NewKeyInput.Value ?? 0));
            if (node != null)
            {
                if (node.Data[CanvasController.DataProps[DataNodeProperties.PhysicalNode]] is TreeNode treeNode)
                {
                    await treeNode.AnimateHighlightAsync();
                }
            }
            return;

        ErrorHandling:
            if (Application.Current!.TryGetResource("Strings.ModifyFailed", out object? errorMessage))
            {
                await Snackbar.ShowSnackbar(
                    MainView.Instance!.MainCanvas,
                    (errorMessage!.ToString() ?? string.Empty) + "!",
                    Snackbar.MessageSeverity.Error);
            }
            await CloseDialog(_dialogRef!);
        }

        private async void Grid_KeyUp(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                await CloseDialog(_dialogRef!);
            }
            if (e.Key == Key.Enter)
            {
                ModifyButton_Click(null, new RoutedEventArgs());
            }
        }
    }
}
