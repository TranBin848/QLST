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
using MailKit.Net.Smtp;
using MimeKit;
using QuanLySoThu.Models;

namespace QuanLySoThu
{
    /// <summary>
    /// Interaction logic for ResetPassword.xaml
    /// </summary>
    public partial class ResetPassword : Window
    {
        private string verificationCode;
        public ResetPassword()
        {
            InitializeComponent();
        }

        private void btnSendCode_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text;

            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Vui lòng điền một email thích hợp");
                return;
            }

            verificationCode = GenerateVerificationCode();
            bool isSent = SendEmailUsingMailKit(email, verificationCode);
            if (isSent)
            {
                MessageBox.Show("Mã xác nhận đặt lại mật khẩu đã được gửi. Vui lòng kiểm tra hòm thư","Thông báo",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            } 
            else
            {
                MessageBox.Show("Không thể gửi mã xác nhận. Hãy thử lại", "Thông báo",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            }
        }
        private string GenerateVerificationCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString(); // Mã 6 chữ số
        }
        public bool SendEmailUsingMailKit(string recipientEmail, string code)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("QLST ZunFoo", "thunderstar848@gmail.com")); // Địa chỉ Gmail của bạn
                message.To.Add(new MailboxAddress("", recipientEmail));
                message.Subject = "Mã xác nhận đặt lại mật khẩu";

                message.Body = new TextPart("plain")
                {
                    Text = $"Mã code xác nhận của bạn là: {code}"
                };

                using (var client = new SmtpClient())
                {
                    // Kết nối tới máy chủ Gmail
                    client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

                    // Sử dụng App Password thay vì mật khẩu thông thường
                    client.Authenticate("thunderstar848@gmail.com", "123");

                    client.Send(message);
                    client.Disconnect(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return false;
            }
        }
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (txtCode.Text == verificationCode)
            {
                // Lấy mật khẩu từ hai ô PasswordBox
                string newPassword = passwordBox.Password;
                string repeatPassword = repeatPasswordBox.Password;

                // Kiểm tra xem mật khẩu nhập lại có khớp không
                if (newPassword != repeatPassword)
                {
                    MessageBox.Show("Mật khẩu không khớp. Thử lại", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                // Kiểm tra độ mạnh của mật khẩu
                if (newPassword.Length < 8 || !newPassword.Any(char.IsLetter) || !newPassword.Any(char.IsDigit))
                {
                    MessageBox.Show("Mật khẩu phải chứa ít nhất 8 ký tự bao gồm chữ và số", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                try
                {
                    using (var context = new QLSTContext())
                    {
                        // Tìm tài khoản theo email
                        var account = context.TAIKHOAN.FirstOrDefault(tk => tk.Email == txtEmail.Text);

                        if (account != null)
                        {
                            // Cập nhật mật khẩu
                            account.MatKhau = newPassword;

                            // Lưu thay đổi vào cơ sở dữ liệu
                            context.SaveChanges();
                            MessageBox.Show("Cập nhật mật khẩu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                            LoginScreen sc = new LoginScreen();
                            sc.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy tài khoản. Vui lòng kiểm tra lại Emial", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Mã xác nhận không đúng. Vui lòng thử lại", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            LoginScreen sc = new LoginScreen();
            sc.Show();
            this.Close();

        }
        
    }
}
