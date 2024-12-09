using QuanLySoThu.Models;
using QuanLySoThu.Properties;
using QuanLySoThu.TableViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuanLySoThu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly QLSTContext _context;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new CLClockViewModel();
            _context = new QLSTContext();
        }
        
        
        private void Button_Click_LogOut(object sender, RoutedEventArgs e)
        {
            LoginScreen loginScreen = new LoginScreen();
            loginScreen.Show();
            this.Close();

        }

        private void btnTrangChu_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnQLCV_Click(object sender, RoutedEventArgs e)
        {
            FrameMain.Content = new QuanLyConVat();
        }

        private void btnQLChuong_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnQLLoai_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnBCTK_Click(object sender, RoutedEventArgs e)
        {
            FrameMain.Content = new BaoCaoThongKe();
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            string sessionToken = Settings.Default.SessionToken;
            int userID = Settings.Default.CurrentUserID;
            var session = _context.ACTIVE_SESSION.FirstOrDefault(s => s.SessionToken == sessionToken);
            var user = _context.TAIKHOAN.FirstOrDefault(u => u.ID == userID);
            if (session != null)
            {
                _context.ACTIVE_SESSION.Remove(session);
                _context.SaveChanges();
            }
            if (user != null)
            {
                _context.SaveChanges();
            }

            Settings.Default.SessionToken = string.Empty;
            Settings.Default.Save();
            Settings.Default.CurrentUserID = -1;
            Settings.Default.Save();

            LoginScreen loginScreen = new LoginScreen();
            loginScreen.Show();
            this.Close();
        }

        
    }
}