using QuanLySoThu.Models;
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
using System.Windows.Shapes;
using QuanLySoThu.Properties;

namespace QuanLySoThu
{
    /// <summary>
    /// Interaction logic for LoginScreen.xaml
    /// </summary>
    public partial class LoginScreen : Window
    {
        private readonly QLSTContext _context;
        public LoginScreen()
        {
            InitializeComponent();
            _context = new QLSTContext();
            CheckSession();
            
        }
        private void CheckSession()
        {
            string sessionToken = Settings.Default.SessionToken;
            if (!string.IsNullOrEmpty(sessionToken))
            {
                var activeSession = _context.ACTIVE_SESSION
                    .FirstOrDefault(s => s.SessionToken == sessionToken && s.ExpiryTime > DateTime.Now);
                if (activeSession != null)
                {
                    MainWindow mw = new MainWindow();
                    mw.Show();
                    this.Close();
                }
            }
        }
        private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = pbPassword.Password;
            // Xác thực người dùng
            var user = ValidateUser(username, password);
            
            if (user != null)
            {
                // Nếu người dùng hợp lệ, kiểm tra session
                if (IsSessionValid(user.ID))
                {
                    MainWindow mw = new MainWindow();
                    mw.Show();
                    this.Close();
                }
                else
                {
                    // Nếu chưa có session hoặc session hết hạn, tạo session mới
                    CreateSession(user);
                    
                    // Lưu ID người dùng vào Properties.Settings
                    Settings.Default.CurrentUserID = user.ID;
                    
                    Settings.Default.Save();
                    // Mở MainWindow và đóng cửa sổ đăng nhập
                    MainWindow mw = new MainWindow();
                    mw.Show();
                    this.Close();
                }
            }    
            else
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }    
            
        }
        private bool IsSessionValid(int userId)
        {
            // Kiểm tra xem người dùng có session hợp lệ không
            var session = _context.ACTIVE_SESSION
                .Where(s => s.IDTaiKhoan == userId && s.ExpiryTime > DateTime.Now)
                .OrderByDescending(s => s.ExpiryTime)
                .FirstOrDefault();

            return session != null;
        }
        private void CreateSession(TAIKHOAN user)
        {
            var session = new ACTIVE_SESSION
            {
                IDTaiKhoan = user.ID,
                SessionToken = Guid.NewGuid().ToString(),
                ExpiryTime = DateTime.Now.AddMinutes(30)
            };

            _context.ACTIVE_SESSION.Add(session);
            _context.SaveChanges();

            Settings.Default.SessionToken = session.SessionToken;
            Settings.Default.Save();
        }
        private void btnForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            ResetPassword rs = new ResetPassword();
            rs.Show();
            this.Close();
            
        }
        private TAIKHOAN ValidateUser(string username, string password)
        {
            // Tìm tài khoản người dùng từ cơ sở dữ liệu
            TAIKHOAN user = null;

            user = _context.TAIKHOAN.FirstOrDefault(u => u.TenTaiKhoan == username && u.MatKhau == password);
            return user;
        }
    }
}
