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
using OnlineChessServer.ViewModels;

namespace OnlineChessServer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private ChessViewModel _viewModel;
    public MainWindow()
    {
        InitializeComponent();
        _viewModel= new ChessViewModel();
        DataContext = _viewModel;
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        _viewModel.CloseConnections();
    }
}
