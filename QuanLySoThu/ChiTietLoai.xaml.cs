using QuanLySoThu.Models;
using QuanLySoThu.TableViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace QuanLySoThu
{
    /// <summary>
    /// Interaction logic for ChiTietLoai.xaml
    /// </summary>
    public partial class ChiTietLoai : UserControl
    {
        private LOAI? _currentLoai; // Thuộc tính lưu DataContext
        

        
        public ChiTietLoai()
        {
            InitializeComponent();
            DataContextChanged += ChiTietLoai_DataContextChanged; // Gắn sự kiện DataContextChanged
            
        }
        private void ChiTietLoai_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _currentLoai = DataContext as LOAI;
            
        }

        private void SaveLoai_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra xem txtKichThuoc và txtTuoiTho có phải là số hợp lệ không
                if (!decimal.TryParse(txtKichThuoc.Text, out decimal kichThuoc) || !decimal.TryParse(txtTuoiTho.Text, out decimal tuoiTho))
                {
                    // Nếu có trường không hợp lệ, hiển thị thông báo lỗi và dừng lại
                    MessageBox.Show("Có một số ô không hợp lệ. Vui lòng nhập đúng định dạng số.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                using (var context = new QLSTContext())
                {
                    // Lấy thông tin từ các TextBox và các điều khiển UI khác
                    var convat = context.LOAI
                        .FirstOrDefault(u => u.MaLoai == _currentLoai.MaLoai && !u.IsDeleted);

                    if (convat == null)
                    {
                        MessageBox.Show("Không tìm thấy loài!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Cập nhật thông tin vào đối tượng CONVAT
                    convat.MoTaNgoaiHinh = txtMoTaNgoaiHinh.Text;
                    convat.KichThuoc = decimal.Parse(txtKichThuoc.Text);
                    convat.MTSTuNhien = txtMTS.Text;
                    convat.TuoiTho = decimal.Parse(txtTuoiTho.Text);
                    convat.ThoiQuenAnUong = txtTQAU.Text;
                    
                    // Lưu thay đổi vào cơ sở dữ liệu
                    context.SaveChanges();
                    MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDoiAnh_Click(object sender, RoutedEventArgs e)
        {
            // Hiển thị hộp thoại để chọn ảnh mới
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png|All Files (*.*)|*.*",
                Title = "Chọn ảnh mới"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Lấy đường dẫn file ảnh
                    string selectedImagePath = openFileDialog.FileName;

                    // Binding dữ liệu cho AnhConVat


                    // Nếu cần, lưu dữ liệu vào cơ sở dữ liệu
                    using (var context = new QLSTContext())
                    {
                        var conVat = context.LOAI.FirstOrDefault(cv => cv.MaLoai == _currentLoai.MaLoai);
                        if (conVat != null)
                        {
                            conVat.AnhLoai = selectedImagePath;
                            context.SaveChanges();
                        }
                    }
                    if (!string.IsNullOrEmpty(selectedImagePath))
                    {
                        var bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.UriSource = new Uri(selectedImagePath, UriKind.Absolute);
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        AnhLoai.ImageSource = bitmapImage; // Cập nhật hình ảnh hiển thị
                    }
                    MessageBox.Show("Cập nhật ảnh thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);


                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void txtKichThuoc_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Lấy giá trị từ TextBox
            string kichThuocText = txtKichThuoc.Text;

            // Kiểm tra nếu giá trị không phải là số
            if (!decimal.TryParse(kichThuocText, out decimal result))
            {
                // Hiển thị error icon và tooltip
                icKichThuocError.Visibility = Visibility.Visible;
                icKichThuocError.ToolTip = "Kích thước trung bình chỉ bao gồm số";
            }
            else
            {
                // Ẩn error icon nếu giá trị hợp lệ
                icKichThuocError.Visibility = Visibility.Collapsed;
            }
        }

        private void txtTuoiTho_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Lấy giá trị từ TextBox
            string tuoithoText = txtTuoiTho.Text;

            // Kiểm tra nếu giá trị không phải là số
            if (!decimal.TryParse(tuoithoText, out decimal result))
            {
                // Hiển thị error icon và tooltip
                icTuoiThoError.Visibility = Visibility.Visible;
                icTuoiThoError.ToolTip = "Tuổi thọ trung bình chỉ bao gồm số";
            }
            else
            {
                // Ẩn error icon nếu giá trị hợp lệ
                icTuoiThoError.Visibility = Visibility.Collapsed;
            }
        }
    }
}
