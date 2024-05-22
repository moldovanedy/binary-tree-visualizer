using System;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace DataStructuresProject.Graphics
{
    public abstract class CanvasItem
    {
        /// <summary>
        /// The top left corner expressed in virtual units.
        /// </summary>
        public Vector2 Position { get; set; } = Vector2.Zero;
        /// <summary>
        /// The width expressed in virtual units.
        /// </summary>
        public double Width { get; set; }
        /// <summary>
        /// The height expressed in virtual units.
        /// </summary>
        public double Height { get; set; }

        public abstract Control PhysicalInstance { get; }

        /// <summary>
        /// Runs every frame like a game loop. For performance, return immediately if nothing needs to be changed.
        /// </summary>
        public abstract void Update();
    }

    public class TreeNode : CanvasItem
    {
        public override Control PhysicalInstance
        {
            get
            {
                if (_ellipse != null)
                {
                    return _ellipse;
                }
                else
                {
                    Initialize();
                    return _ellipse!;
                }
            }
        }
        private Ellipse? _ellipse;

        public TreeNode()
        {
            Initialize();
        }

        ~TreeNode()
        {
            if (_ellipse != null)
            {
                _ellipse.PointerEntered -= this.Ellipse_PointerEntered;
                _ellipse.PointerExited -= this.Ellipse_PointerExited;
                _ellipse.GotFocus -= this.Ellipse_GotFocus;
                _ellipse.LostFocus -= this.Ellipse_LostFocus;
            }

            _ellipse = null;

        }

        public void Initialize()
        {
            _ellipse = new Ellipse
            {
                //arrows -> 0, nodes - 1, other -> >= 2
                ZIndex = 1,
                Focusable = true,

                Fill = new SolidColorBrush(0xff_ff_ff_ff),
                StrokeThickness = 5
            };
            Width = 1;
            Height = 1;

            Transitions transitions =
            [
                new BrushTransition()
                {
                    Easing = new LinearEasing(),
                    Property = Shape.FillProperty,
                    Duration = new TimeSpan(0, 0, 0, 0, 100)
                },
                new BrushTransition()
                {
                    Easing = new LinearEasing(),
                    Property = Shape.StrokeProperty,
                    Duration = new TimeSpan(0, 0, 0, 0, 100)
                }
            ];
            _ellipse.Transitions = transitions;

            _ellipse.PointerEntered += this.Ellipse_PointerEntered;
            _ellipse.PointerExited += this.Ellipse_PointerExited;
            _ellipse.GotFocus += this.Ellipse_GotFocus;
            _ellipse.LostFocus += this.Ellipse_LostFocus;

            SetNormalStyle();
            CanvasRenderer.TransformItemToDeviceCoordinates(this);
        }

        public override void Update()
        {

        }

        #region Styles
        private bool _hasFocus = false;

        private void Ellipse_GotFocus(object? sender, GotFocusEventArgs e)
        {
            _hasFocus = true;
            //if it got focus, the pointer must be over it (except keyboard navigation, but that's not supported)
            SetFocusedHoverStyle();
        }

        private void Ellipse_LostFocus(object? sender, RoutedEventArgs e)
        {
            _hasFocus = false;
            SetNormalStyle();
        }

        private void Ellipse_PointerExited(object? sender, PointerEventArgs e)
        {
            if (_hasFocus)
            {
                SetFocusedStyle();
            }
            else
            {
                SetNormalStyle();
            }
        }

        private void Ellipse_PointerEntered(object? sender, PointerEventArgs e)
        {
            if (_hasFocus)
            {
                SetFocusedHoverStyle();
            }
            else
            {
                SetNormalHoverStyle();
            }
        }

        private void SetNormalStyle()
        {
            if (_ellipse == null)
            {
                return;
            }
            _ellipse.Stroke = new SolidColorBrush(0xff_15_65_c0);
            _ellipse.Fill = new SolidColorBrush(0xff_15_65_c0);
        }

        private void SetNormalHoverStyle()
        {
            if (_ellipse == null)
            {
                return;
            }
            _ellipse.Stroke = new SolidColorBrush(0xff_1e_88_e5);
            _ellipse.Fill = new SolidColorBrush(0xff_1e_88_e5);
        }

        private void SetFocusedStyle()
        {
            if (_ellipse == null)
            {
                return;
            }
            _ellipse.Stroke = new SolidColorBrush(0xff_21_96_f3);
            _ellipse.Fill = new SolidColorBrush(0xff_e6_51_00);
        }

        private void SetFocusedHoverStyle()
        {
            if (_ellipse == null)
            {
                return;
            }
            _ellipse.Stroke = new SolidColorBrush(0xff_64_b5_f6);
            _ellipse.Fill = new SolidColorBrush(0xff_ef_6c_00);
        }
        #endregion
    }

    public class Arrow : CanvasItem
    {
        public override Control PhysicalInstance
        {
            get
            {
                if (_line != null)
                {
                    return _line;
                }
                else
                {
                    Initialize();
                    return _line!;
                }
            }
        }
        private Line? _line;

        public void Initialize()
        {
            _line = new Line()
            {
                //arrows -> 0, nodes - 1, other -> >= 2
                ZIndex = 0,
                Stroke = new SolidColorBrush(0xff_ff_ff_ff),
                StrokeThickness = 3
            };
            Width = 1;
            Height = 1;

            CanvasRenderer.TransformItemToDeviceCoordinates(this);
        }

        public override void Update()
        {

        }
    }
}
