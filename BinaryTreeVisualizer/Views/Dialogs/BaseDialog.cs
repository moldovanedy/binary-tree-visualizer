using System;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace BinaryTreeVisualizer.Views.Dialogs
{
    public interface IDialog
    {
        public const int ANIMATION_DURATION = 300;

        public Task OpenDialog(Controls dialogs);
        public Task CloseDialog(Controls dialogs);

        public void Draw(Grid dialogRoot, double rootWidth, double rootHeight, DialogProperties dialogProperties)
        {
            dialogRoot.ColumnDefinitions[1].Width = new GridLength(
                Math.Clamp(
                    rootWidth * dialogProperties.WidthPercent,
                    dialogProperties.MinWidth,
                    dialogProperties.MaxWidth));
            dialogRoot.RowDefinitions[1].Height = new GridLength(
                Math.Clamp(
                    rootHeight * dialogProperties.HeightPercent,
                    dialogProperties.MinHeight,
                    dialogProperties.MaxHeight));
        }
    }

    public class DialogProperties
    {
        /// <summary>
        /// 0 means 0%, 1 means 100%.
        /// </summary>
        public double WidthPercent { get; set; }
        /// <summary>
        /// 0 means 0%, 1 means 100%.
        /// </summary>
        public double HeightPercent { get; set; }

        public double MinWidth { get; set; }
        public double MinHeight { get; set; }
        public double MaxWidth { get; set; } = double.MaxValue;
        public double MaxHeight { get; set; } = double.MaxValue;
    }
}
