using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using DataStructuresProject.Graphics;
using DataStructuresProject.Views.Dialogs;

namespace DataStructuresProject.Views
{
    public partial class MainView : UserControl
    {
        public static MainView? Instance { get; private set; }

        private Point _lastPointerPosition = new Point(0, 0);

        public MainView()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                return;
            }

            InitializeComponent();
        }

        public static void AddItemToCanvas(Control control)
        {
            Instance?.MainCanvas.Children.Add(control);
        }

        public static void RemoveItemFromCanvas(Control control)
        {
            Instance?.MainCanvas.Children.Remove(control);
        }


        private void MainCanvas_SizeChanged(object? sender, SizeChangedEventArgs e)
        {
            CanvasController.UpdateCanvasSize(e.NewSize.Width, e.NewSize.Height);
        }

        private void MainCanvas_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            if (e.Delta.Y == 0)
            {
                return;
            }

            //+/- 10%
            double newZoomLevel = e.Delta.Y > 0 ?
                (CanvasController.Zoom + (CanvasController.Zoom * 0.1)) :
                (CanvasController.Zoom - (CanvasController.Zoom * 0.1));
            CanvasController.UpdateZoomLevel(newZoomLevel);
        }

        private void MainCanvas_PointerMoved(object? sender, PointerEventArgs e)
        {
            //if not 0, it means the pointer was pressed; consider eventual precision loss
            if (_lastPointerPosition.X > 0.01 && _lastPointerPosition.Y > 0.01)
            {
                Point newPosition = e.GetPosition(MainCanvas);
                CanvasController.UpdateOffset(
                    _lastPointerPosition.X - newPosition.X,
                    //Y axis is up, we need to invert the operands
                    newPosition.Y - _lastPointerPosition.Y);
                _lastPointerPosition = newPosition;
            }
        }

        private void MainCanvas_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            Point pressPosition = e.GetPosition(MainCanvas);
            _lastPointerPosition = new Point(pressPosition.X, pressPosition.Y);
        }

        private void MainCanvas_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            _lastPointerPosition = new Point(0, 0);
        }

        private async void InsertButton_Click(object? sender, RoutedEventArgs e)
        {
            InsertNodeDialog insertNodeDialog = new InsertNodeDialog();
            await insertNodeDialog.OpenDialog(DialogPanel.Children);
        }

        private async void RemoveButton_Click(object? sender, RoutedEventArgs e)
        {
            RemoveNodeDialog removeNodeDialog = new RemoveNodeDialog();
            await removeNodeDialog.OpenDialog(DialogPanel.Children);

            //await Snackbar.ShowSnackbar(MainCanvas, "AAAAAAAA", Snackbar.MessageSeverity.Warning);
        }
    }
}
