using OnlineChessLibrary.BoardElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OnlineChessServer.Controls
{
    /// <summary>
    /// Interaction logic for CellView.xaml
    /// </summary>
    public partial class ChessPiece : UserControl
    {
        public static readonly DependencyProperty StateProperty = DependencyProperty.Register("State", typeof(State), typeof(ChessPiece));

        public State State
        {
            get => (State)GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        public ChessPiece()
        {
            InitializeComponent();
        }
    }
}
