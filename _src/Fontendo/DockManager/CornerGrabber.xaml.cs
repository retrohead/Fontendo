using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Fontendo.Controls
{
    /// <summary>
    /// Interaction logic for CornerGrabber.xaml
    /// </summary>
    public partial class CornerGrabber : UserControl
    {
        public class DotPosition
        {
            public double X { get; set; }
            public double Y { get; set; }

            public DotPosition(double x, double y)
            {
                X = x;
                Y = y;
            }
        }
        public ObservableCollection<DotPosition> DotPositions { get; } = new()
        {
            // bottom row (3 dots)
            new DotPosition(8, 48),
            new DotPosition(24, 48),
            new DotPosition(40, 48),

            // middle row (2 dots)
            new DotPosition(16, 32),
            new DotPosition(32, 32),

            // top row (1 dot)
            new DotPosition(24, 16),
        };
        public CornerGrabber()
        {
            InitializeComponent();

        }
    }
}
