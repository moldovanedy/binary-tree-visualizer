using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using BinaryTreeVisualizer.Core;
using BinaryTreeVisualizer.Graphics;
using BinaryTreeVisualizer.Views.Dialogs;

namespace BinaryTreeVisualizer.Views
{
    public partial class MainView : UserControl
    {
        public static MainView? Instance { get; private set; }

        private const double SENSITIVITY = 0.5;

        private Point _lastPointerPosition = new(0, 0);
        private bool _isLeftKeyPressed;
        private bool _isUpKeyPressed;
        private bool _isRightKeyPressed;
        private bool _isDownKeyPressed;

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
            UpdateTreeInfo(0, 0);

            MainWindow.UpdatedFrame += Update;
        }

        private void Update(TimeSpan delta)
        {
            //Debug.WriteLine($"Pressed: {_isUpKeyPressed}, {_isRightKeyPressed}, {_isDownKeyPressed}, {_isLeftKeyPressed}");

            double deltaX = 0, deltaY = 0;
            if (_isUpKeyPressed)
            {
                deltaY = SENSITIVITY * delta.TotalMilliseconds;
            }
            if (_isDownKeyPressed)
            {
                deltaY = -SENSITIVITY * delta.TotalMilliseconds;
            }
            if (_isLeftKeyPressed)
            {
                deltaX = -SENSITIVITY * delta.TotalMilliseconds;
            }
            if (_isRightKeyPressed)
            {
                deltaX = SENSITIVITY * delta.TotalMilliseconds;
            }

            CanvasController.UpdateOffset(deltaX, deltaY);
        }

        public static void AddItemToCanvas(Control control)
        {
            Instance?.MainCanvas.Children.Add(control);
        }

        public static void RemoveItemFromCanvas(Control control)
        {
            Instance?.MainCanvas.Children.Remove(control);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="depth">If -1, it won't update the depth label.</param>
        /// <param name="size">If -1, it won't update the depth size.</param>
        public static void UpdateTreeInfo(int depth = -1, int size = -1)
        {
            if (Application.Current!.TryGetResource("Strings.Depth", out object? depthDescription) && depth != -1)
            {
                Instance!.DepthLabel.Content = $"{depthDescription}: {depth}";
            }
            if (Application.Current!.TryGetResource("Strings.Size", out object? sizeDescription) && size != -1)
            {
                Instance!.SizeLabel.Content = $"{sizeDescription}: {size}";
            }
        }

        public static void UpdateNodeDetails(int key, int level, bool isLeafNode, bool shouldDisplayInfo = true)
        {
            if (shouldDisplayInfo)
            {
                Application.Current!.TryGetResource("Strings.Yes", out object? yesMessage);
                Application.Current!.TryGetResource("Strings.No", out object? noMessage);

                Instance!.DetailsNodeKey.Content = key;
                Instance.DetailsNodeLevel.Content = level;
                Instance.DetailsIsLeafNode.Content = isLeafNode ? yesMessage ?? "1" : noMessage ?? "0";
            }
            else
            {
                Instance!.DetailsNodeKey.Content = "-";
                Instance.DetailsNodeLevel.Content = "-";
                Instance.DetailsIsLeafNode.Content = "-";
            }
        }

        #region Canvas handling
        private void MainCanvas_SizeChanged(object? sender, SizeChangedEventArgs e)
        {
            CanvasController.UpdateCanvasSize(e.NewSize.Width, e.NewSize.Height);
            if (e.NewSize.Width > 750)
            {
                (MainCanvas.Parent as Grid)!.ColumnDefinitions[0].Width = new GridLength(0.5, GridUnitType.Star);
            }
            else
            {
                (MainCanvas.Parent as Grid)!.ColumnDefinitions[0].Width = new GridLength(200, GridUnitType.Pixel);
            }

            if (e.NewSize.Height > 650)
            {
                (MainCanvas.Parent as Grid)!.RowDefinitions[0].Height = new GridLength(0.5, GridUnitType.Star);
                //ensure there was a layout recalculation so Bounds is updated
                Task.Delay(15).ContinueWith(_ =>
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        TraversePanel.MaxHeight = TopLeftPanel.Bounds.Height;
                    });
                });
            }
            else
            {
                (MainCanvas.Parent as Grid)!.RowDefinitions[0].Height = new GridLength(200, GridUnitType.Pixel);
                TraversePanel.MaxHeight = 200;
            }
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
        #endregion

        #region Actions
        private async void InsertButton_Click(object? sender, RoutedEventArgs e)
        {
            ToggleHighlightForLeafNodes(false);
            ToggleDetailsAndTraversePanels(false);
            InsertNodeDialog insertNodeDialog = new();
            await insertNodeDialog.OpenDialog(DialogPanel.Children);
        }

        private async void RemoveButton_Click(object? sender, RoutedEventArgs e)
        {
            ToggleHighlightForLeafNodes(false);
            ToggleDetailsAndTraversePanels(false);
            RemoveNodeDialog removeNodeDialog = new();
            await removeNodeDialog.OpenDialog(DialogPanel.Children);
        }

        private async void ModifyButton_Click(object? sender, RoutedEventArgs e)
        {
            ToggleHighlightForLeafNodes(false);
            ToggleDetailsAndTraversePanels(false);
            ModifyNodeDialog modifyNodeDialog = new();
            await modifyNodeDialog.OpenDialog(DialogPanel.Children);
        }

        private void DestroyButton_Click(object? sender, RoutedEventArgs e)
        {
            CanvasController.PhysicalItems.Dispose();
            MainCanvas.Children.Clear();
            UpdateTreeInfo(0, 0);
        }

        private void RecenterButton_Click(object? sender, RoutedEventArgs e)
        {
            CanvasController.VirtualOffset = new Vector2(0, 0);
            CanvasController.UpdateOffset(0, 0);
        }
        
        private void HandleTraversing_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                ToggleDetailsAndTraversePanels(checkBox.IsChecked ?? false);
            }
        }
        
        private void HighlightLeafNodes_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                ToggleHighlightForLeafNodes(checkBox.IsChecked ?? false);
            }
        }
        #endregion

        #region Traversal
        private async void PreorderTraversalButton_Click(object? sender, RoutedEventArgs e)
        {
            PreorderTraversalButton.IsEnabled = false;
            InorderTraversalButton.IsEnabled = false;
            PostorderTraversalButton.IsEnabled = false;

            await AnimateNodesAsync(
                CanvasController.PhysicalItems.Traverse(TreeTraversalMode.Preorder),
                TreeTraversalMode.Preorder);

            PreorderTraversalButton.IsEnabled = true;
            InorderTraversalButton.IsEnabled = true;
            PostorderTraversalButton.IsEnabled = true;
        }

        private async void InorderTraversalButton_Click(object? sender, RoutedEventArgs e)
        {
            PreorderTraversalButton.IsEnabled = false;
            InorderTraversalButton.IsEnabled = false;
            PostorderTraversalButton.IsEnabled = false;

            await AnimateNodesAsync(
                CanvasController.PhysicalItems.Traverse(TreeTraversalMode.Inorder),
                TreeTraversalMode.Inorder);

            PreorderTraversalButton.IsEnabled = true;
            InorderTraversalButton.IsEnabled = true;
            PostorderTraversalButton.IsEnabled = true;
        }

        private async void PostorderTraversalButton_Click(object? sender, RoutedEventArgs e)
        {
            PreorderTraversalButton.IsEnabled = false;
            InorderTraversalButton.IsEnabled = false;
            PostorderTraversalButton.IsEnabled = false;

            await AnimateNodesAsync(
                CanvasController.PhysicalItems.Traverse(TreeTraversalMode.Postorder),
                TreeTraversalMode.Postorder);

            PreorderTraversalButton.IsEnabled = true;
            InorderTraversalButton.IsEnabled = true;
            PostorderTraversalButton.IsEnabled = true;
        }

        private async Task AnimateNodesAsync(List<int> keys, TreeTraversalMode mode)
        {
            foreach (int key in keys)
            {
                BinarySearchTreeNode? node = CanvasController.PhysicalItems.Search(key);
                if (node != null)
                {
                    if (node.Data[CanvasController.DataProps[DataNodeProperties.PhysicalNode]] is TreeNode treeNode)
                    {
                        await treeNode.AnimateHighlightAsync();
                        string stringToAdd = key + " ";
                        switch (mode)
                        {
                            case TreeTraversalMode.Preorder:
                                PreorderTraversalText.Text += stringToAdd;
                                break;
                            case TreeTraversalMode.Inorder:
                                InorderTraversalText.Text += stringToAdd;
                                break;
                            case TreeTraversalMode.Postorder:
                                PostorderTraversalText.Text += stringToAdd;
                                break;
                        }
                    }
                }
            }
        }
        #endregion

        private void Canvas_KeyDown(object? sender, KeyEventArgs e)
        {
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (e.Key)
            {
                case Key.Up:
                    _isUpKeyPressed = true;
                    break;
                case Key.Down:
                    _isDownKeyPressed = true;
                    break;
                case Key.Left:
                    _isLeftKeyPressed = true;
                    break;
                case Key.Right:
                    _isRightKeyPressed = true;
                    break;
            }
        }

        private void Canvas_KeyUp(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                _isUpKeyPressed = false;
            }
            if (e.Key == Key.Down)
            {
                _isDownKeyPressed = false;
            }
            if (e.Key == Key.Left)
            {
                _isLeftKeyPressed = false;
            }
            if (e.Key == Key.Right)
            {
                _isRightKeyPressed = false;
            }
        }

        private void ToggleDetailsAndTraversePanels(bool shouldShowTraverse)
        {
            if (shouldShowTraverse)
            {
                DetailsPanel.IsVisible = false;
                TraversePanel.IsVisible = true;
                HandleTraversingCheckBox.IsChecked = true;
            }
            else
            {
                DetailsPanel.IsVisible = true;
                TraversePanel.IsVisible = false;
                HandleTraversingCheckBox.IsChecked = false;
            }

            PreorderTraversalText.Text = string.Empty;
            InorderTraversalText.Text = string.Empty;
            PostorderTraversalText.Text = string.Empty;
        }

        private void ToggleHighlightForLeafNodes(bool shouldHighlight)
        {
            //enable highlight for all leaf nodes
            List<int> allNodesKeys = CanvasController.PhysicalItems.Traverse(TreeTraversalMode.Preorder);
            foreach (int nodeKey in allNodesKeys)
            { 
                BinarySearchTreeNode? node = CanvasController.PhysicalItems.Search(nodeKey);
                if (node?.IsLeafNode ?? false)
                {
                    var ellipse = (node.Data[CanvasController.DataProps[DataNodeProperties.PhysicalNode]]) as TreeNode;
                    ellipse?.ToggleHighlightNode(shouldHighlight);
                }
            }
            HighlightLeafNodesCheckBox.IsChecked = shouldHighlight;
        }
    }
}
