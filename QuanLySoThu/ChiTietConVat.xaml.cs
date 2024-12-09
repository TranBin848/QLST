using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Microsoft.EntityFrameworkCore;
using QuanLySoThu.Models;
using QuanLySoThu.TableViewModels;
namespace QuanLySoThu
{
    /// <summary>
    /// Interaction logic for ChiTietConVat.xaml
    /// </summary>
    public partial class ChiTietConVat : UserControl
    {
        private ConVatViewModel? _currentConvat; // Thuộc tính lưu DataContext

        public ChiTietConVat()
        {
            InitializeComponent();
            DataContextChanged += ChiTietConVat_DataContextChanged; // Gắn sự kiện DataContextChanged

        }
        private void ChiTietConVat_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _currentConvat = DataContext as ConVatViewModel;
            

            LoadComboBoxData();
        }
        
        private void LoadComboBoxData()
        {
            if (_currentConvat.GioiTinh == "Đực")
            {
                cbDuc.IsChecked = true;
            }
            else
            {
                cbCai.IsChecked = true;
            }
            using (var context = new QLSTContext())
            {
                var loaiList = context.LOAI.ToList();
                List<CHUONG> filteredChuong = new List<CHUONG>();
                // Lấy giá trị ID của LOAI được chọn
                var selectedLoaiID = (int?)_currentConvat.IDLoai;               
                    // Truy vấn LOAI từ cơ sở dữ liệu
                    var selectedLoai = context.LOAI.FirstOrDefault(loai => loai.ID == selectedLoaiID);

                    if (selectedLoai != null)
                    {
                        string loaiDongVat = selectedLoai.LoaiDongVat;
                        string phanLoai = selectedLoai.PhanLoai;

                       

                        // Logic xác định danh sách Chuồng
                        if (selectedLoai.TenLoai.Contains("Rắn"))
                        {
                            filteredChuong = context.CHUONG
                                .Where(c => c.TenChuong == "Chuồng Rắn" && !c.IsDeleted && c.SoLuongConVat < c.SucChua)
                                .ToList();
                        }
                        else if (loaiDongVat == "Động Vật Trên Không")
                        {
                            filteredChuong = context.CHUONG
                                .Where(c => c.TenChuong == "Chuồng Chim" && !c.IsDeleted && c.SoLuongConVat < c.SucChua)
                                .ToList();
                        }
                        else if (loaiDongVat == "Động Vật Dưới Nước")
                        {
                            if (phanLoai == "Nước Mặn")
                            {
                                filteredChuong = context.CHUONG
                                    .Where(c => c.TenChuong == "Thủy Cung" && !c.IsDeleted && c.SoLuongConVat < c.SucChua)
                                    .ToList();
                            }
                            else if (phanLoai == "Nước Ngọt")
                            {
                                filteredChuong = context.CHUONG
                                    .Where(c => c.TenChuong == "Làng Cá Nước Ngọt" && !c.IsDeleted && c.SoLuongConVat < c.SucChua)
                                    .ToList();
                            }
                        }
                        else if (loaiDongVat == "Động Vật Trên Cạn")
                        {
                            if (phanLoai == "Ăn Thịt")
                            {
                                filteredChuong = context.CHUONG
                                .Where(c =>
                                    (c.TenChuong == "Trại Hổ" ||
                                     c.TenChuong == "Trại Gấu" ||
                                     c.TenChuong == "Trại Sư Tử")
                                    && !c.IsDeleted &&
                                    c.SoLuongConVat < c.SucChua)
                                .ToList();
                            }
                            else if (phanLoai == "Ăn Thực Vật")
                            {
                                filteredChuong = context.CHUONG
                                    .Where(c => c.TenChuong == "Chuồng Ăn Cỏ" && !c.IsDeleted && c.SoLuongConVat < c.SucChua)
                                    .ToList();
                            }
                        }

                        // Gán danh sách Chuồng vào cbChuong
                        cbChuong.ItemsSource = filteredChuong;
                    }
                
            
                cbLoai.ItemsSource = loaiList;
                
                
                // Thiết lập giá trị của ComboBox
                if (_currentConvat != null)
                {
                    
                    var loai = loaiList.FirstOrDefault(l => l.ID == _currentConvat.IDLoai);
                    var chuong = filteredChuong.FirstOrDefault(c => c.ID == _currentConvat.IDChuong);
                    // Chọn giá trị TenLoai và TenChuong trong ComboBox dựa trên ID
                    
                    cbLoai.Text = loai?.TenLoai;
                    cbChuong.Text = chuong?.TenChuong;
                    cbTTSK.Text = _currentConvat.SucKhoe;
                }
            }
        }
        // Sự kiện nút Lưu
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            

            try
            {
                if (string.IsNullOrWhiteSpace(txtTenConVat.Text) ||
                    (cbDuc.IsChecked != true && cbCai.IsChecked != true) ||
                    cbTTSK.SelectedItem == null ||
                    !dpNgaySinh.SelectedDate.HasValue ||
                    !dpNgayNhap.SelectedDate.HasValue ||
                    cbLoai.SelectedItem == null ||
                    cbChuong.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                using (var context = new QLSTContext())
                {
                    // Lấy thông tin từ các TextBox và các điều khiển UI khác
                    var convat = context.CONVAT
                        .FirstOrDefault(u => u.MaConVat == _currentConvat.MaConVat && !u.IsDeleted);

                    if (convat == null)
                    {
                        MessageBox.Show("Không tìm thấy con vật!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Cập nhật thông tin vào đối tượng CONVAT
                    convat.TenConVat = txtTenConVat.Text;
                    convat.GioiTinh = cbDuc.IsChecked == true ? "Đực" : "Cái";
                    string trangThaiSucKhoe = (cbTTSK.SelectedItem as ComboBoxItem)?.Content.ToString();                   
                    convat.TrangThaiSucKHoe = trangThaiSucKhoe;
                    // Cập nhật ngày sinh và ngày nhập
                    convat.NgaySinh = dpNgaySinh.SelectedDate.Value;
                    convat.NgayNhap = dpNgayNhap.SelectedDate.HasValue ? dpNgayNhap.SelectedDate.Value : DateTime.Now;

                    // Cập nhật Loại và Chuồng
                    // Lấy đối tượng được chọn trong ComboBox cbLoai
                    var selectedLoai = cbLoai.SelectedItem as LOAI;
                    if (selectedLoai != null)
                    {
                        convat.IDLoai = selectedLoai.ID; // Gán ID của Loại vào Con Vật
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng chọn loại con vật!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Lấy đối tượng được chọn trong ComboBox cbChuong
                    var selectedChuong = cbChuong.SelectedItem as CHUONG;
                    if (selectedChuong != null && selectedChuong.SoLuongConVat < selectedChuong.SucChua)
                    {
                        convat.IDChuong = selectedChuong.ID; // Gán ID của Chuồng vào Con Vật
                        if (selectedChuong.SoLuongConVat == 0) selectedChuong.SoLuongConVat = 1;
                        else selectedChuong.SoLuongConVat += 1;
                    }
                    else
                    {
                        MessageBox.Show("Chuồng đã đầy!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

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
        private void cbDuc_Checked(object sender, RoutedEventArgs e)
        {
            // Tái tải lại các quyền khi checkbox "Độc giả" được chọn

            cbCai.IsChecked = false;
        }


        private void cbCai_Checked(object sender, RoutedEventArgs e)
        {
            // Tái tải lại các quyền khi checkbox "Admin" được chọn

            cbDuc.IsChecked = false;
        }
        private void ChangeAvatarButton_Click(object sender, RoutedEventArgs e)
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
                            var conVat = context.CONVAT.FirstOrDefault(cv => cv.MaConVat == _currentConvat.MaConVat);
                            if (conVat != null)
                            {
                                conVat.AnhConVat = selectedImagePath;
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
                            AvatarImage.ImageSource = bitmapImage; // Cập nhật hình ảnh hiển thị
                        }
                    MessageBox.Show("Cập nhật ảnh thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void txtTenConVat_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenConVat.Text))
            {
                icNameError.ToolTip = "Tên con vật không được để trống!";
                icNameError.Visibility = Visibility.Visible;
                return;
            }

            icNameError.Visibility = Visibility.Collapsed;
        }
    }
}
