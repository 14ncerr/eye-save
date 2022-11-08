using EyeSave.ViewModels;
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

namespace EyeSave.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowViewModel _viewModel;
        public MainWindow()
        {
            InitializeComponent();
            _viewModel = (MainWindowViewModel)DataContext;    
        }

        private void btnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedPage.pageNum == 1)
                return;

            var currentPageNum = _viewModel.SelectedPage.pageNum;

            _viewModel.SelectedPage = _viewModel.Pages[currentPageNum - 2];        
        }

        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedPage.pageNum == _viewModel.Pages.Count)
                return;

            var currentPageNum = _viewModel.SelectedPage.pageNum;

            _viewModel.SelectedPage = _viewModel.Pages[currentPageNum];
        }
    }
}
