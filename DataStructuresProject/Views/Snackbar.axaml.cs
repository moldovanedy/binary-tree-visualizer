using System.Threading.Tasks;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace DataStructuresProject.Views
{
    public partial class Snackbar : UserControl
    {
        public string Message
        {
            get
            {
                return (string)(MainLabel.Content ?? string.Empty);
            }
            set
            {
                MainLabel.Content = value;
            }
        }

        public MessageSeverity Severity
        {
            get
            {
                return _severity;
            }
            set
            {
                _severity = value;
                if (Border.Background is SolidColorBrush brush)
                {
                    switch (value)
                    {
                        case MessageSeverity.Information:
                            brush.Color = new Color(0xff, 0x02, 0x77, 0xbd);
                            break;
                        case MessageSeverity.Warning:
                            brush.Color = new Color(0xff, 0xf5, 0x7f, 0x17);
                            break;
                        case MessageSeverity.Error:
                            brush.Color = new Color(0xff, 0xd3, 0x2f, 0x2f);
                            break;
                    }
                }
                else
                {
                    Border.Background = new SolidColorBrush()
                    {
                        Color = value switch
                        {
                            MessageSeverity.Information => new Color(0xff, 0x02, 0x77, 0xbd),
                            MessageSeverity.Warning => new Color(0xff, 0xf5, 0x7f, 0x17),
                            MessageSeverity.Error => new Color(0xff, 0xd3, 0x2f, 0x2f),
                            _ => new Color(0xff, 0x42, 0x42, 0x42)
                        }
                    };
                }
            }
        }
        private MessageSeverity _severity = MessageSeverity.Information;

        public Snackbar()
        {
            InitializeComponent();
            this.DataContext = this;

            this.Transitions = [
                new DoubleTransition() {
                    Duration = new System.TimeSpan(0, 0,0 ,0, 300),
                    Property = Control.OpacityProperty
                }
            ];
        }

        public static async Task<Snackbar> ShowSnackbar(Canvas parent, string message, MessageSeverity severity = MessageSeverity.Information)
        {
            Snackbar snackbar = new Snackbar()
            {
                Message = message,
                Severity = severity,
                Opacity = 0
            };

            parent.Children.Add(snackbar);

            Canvas.SetBottom(snackbar, 30);
            Canvas.SetLeft(snackbar, 15);

            snackbar.Opacity = 1;
            await Task.Delay(300);

            //auto-delete
            Dispatcher.UIThread.Post(async () =>
            {
                await Task.Delay(6000);
                await HideSnackbar(parent, snackbar);
            });
            return snackbar;
        }

        public static async Task HideSnackbar(Canvas parent, Snackbar snackbar)
        {
            snackbar.Opacity = 0;
            await Task.Delay(300);
            parent.Children.Remove(snackbar);
        }

        public enum MessageSeverity
        {
            Information = 0,
            Warning = 1,
            Error = 2
        }
    }
}
