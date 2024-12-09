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
using OfficeOpenXml;
using System.IO;

namespace QuanLySoThu
{
    /// <summary>
    /// Interaction logic for CreateConVat.xaml
    /// </summary>
    public partial class CreateConVat : Window
    {
        private QLSTContext _dbContext;
        public event Action ReloadRequested;

        public CreateConVat()
        {
            InitializeComponent();
            _dbContext = new QLSTContext();
            LoadComboBoxes();
        }
        private void LoadComboBoxes()
        {
            //cbTTSK.ItemsSource = new List<string> { "Tốt", "Khá tốt", "Ổn", "Yếu" };
            // Load dữ liệu cho ComboBox Loại
            cbLoai.ItemsSource = _dbContext.LOAI.Where(l => !l.IsDeleted).ToList();

            //// Load dữ liệu cho ComboBox Chuồng
            //cbChuong.ItemsSource = _dbContext.CHUONG.Where(c => !c.IsDeleted && c.SoLuongConVat < c.SucChua).ToList();
        }
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu
                if (string.IsNullOrWhiteSpace(txtTenConVat.Text) ||
                    (cbDuc.IsChecked == false && cbCai.IsChecked == false) ||
                    cbLoai.SelectedItem == null ||
                    cbChuong.SelectedItem == null ||
                    dpNgaySinh.SelectedDate == null ||
                    dpNgayNhap.SelectedDate == null ||
                    cbTTSK.SelectedItem == null) // Kiểm tra giá trị từ ComboBox TTSK
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                // Kiểm tra nếu ngày sinh lớn hơn ngày nhập
                if (dpNgaySinh.SelectedDate > dpNgayNhap.SelectedDate)
                {
                    MessageBox.Show("Ngày sinh không thể lớn hơn ngày nhập!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Lấy giá trị trạng thái sức khỏe từ ComboBox
                string tenLoai = ((LOAI)cbLoai.SelectedItem).TenLoai;
                string tenChuong = ((CHUONG)cbChuong.SelectedItem).TenChuong;
                string trangThaiSucKhoe = (cbTTSK.SelectedItem as ComboBoxItem)?.Content.ToString();

                var loai = _dbContext.LOAI.FirstOrDefault(l => l.TenLoai == tenLoai && !l.IsDeleted);
                if (loai == null)
                {
                    MessageBox.Show($"Không tìm thấy Loài với tên: {tenLoai}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Tìm ID của Chuồng dựa trên TenChuong
                var chuong = _dbContext.CHUONG.FirstOrDefault(c => c.TenChuong == tenChuong && !c.IsDeleted);
                if (chuong == null || chuong.SoLuongConVat >= chuong.SucChua)
                {
                    MessageBox.Show($"Chuồng không khả dụng hoặc đã đầy: {tenChuong}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Tạo đối tượng CONVAT mới
                var newConVat = new CONVAT
                {
                    TenConVat = txtTenConVat.Text,
                    GioiTinh = cbDuc.IsChecked == true ? "Đực" : "Cái",
                    NgaySinh = dpNgaySinh.SelectedDate.Value,
                    NgayNhap = dpNgayNhap.SelectedDate.Value,
                    TrangThaiSucKHoe = trangThaiSucKhoe,
                    IDLoai = loai.ID,
                    IDChuong = chuong.ID,
                    IsDeleted = false,
                };

                // Lưu vào database
                _dbContext.CONVAT.Add(newConVat);
                
                _dbContext.SaveChanges();
                
                if (chuong != null)
                {
                    if (chuong.SoLuongConVat != 0) chuong.SoLuongConVat += 1;
                    else chuong.SoLuongConVat = 1; // Tăng số lượng con vật
                    _dbContext.SaveChanges(); // Lưu thay đổi
                }

                MessageBox.Show("Thêm con vật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearFields();
                ReloadRequested?.Invoke();
                this.Close(); // Đóng cửa sổ sau khi tạo thành công
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException?.Message ?? ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
        private void ClearFields()
        {
            txtTenConVat.Text = string.Empty;
            cbDuc.IsChecked = false;
            cbCai.IsChecked = false;
            
            dpNgaySinh.SelectedDate = null;
            dpNgayNhap.SelectedDate = null;
            
            cbTTSK.Text = null;
            cbLoai.Text = null;
            cbChuong.Text = null;
        }
        private void cbDuc_Checked(object sender, RoutedEventArgs e)
        {
            // Nếu CheckBox "Đực" được tick, kiểm tra và xóa item "Có Thai" khỏi ComboBox
            var item = cbTTSK.Items
                .OfType<ComboBoxItem>()
                .FirstOrDefault(i => (string)i.Content == "Có Thai");

            if (item != null)
            {
                cbTTSK.Items.Remove(item);
            }

            // Đảm bảo bỏ tick CheckBox "Cái"
            cbCai.IsChecked = false;
        }

        
        private void cbCai_Checked(object sender, RoutedEventArgs e)
        {
            // Khi CheckBox "Cái" được tick, thêm lại item "Có Thai" nếu nó bị xóa trước đó
            if (!cbTTSK.Items
                .OfType<ComboBoxItem>()
                .Any(i => (string)i.Content == "Có Thai"))
            {
                cbTTSK.Items.Add(new ComboBoxItem { Content = "Có Thai" });
            }

            // Đảm bảo bỏ tick CheckBox "Đực"
            cbDuc.IsChecked = false;
        }
        private void btnCreateLoai_Click(object sender, RoutedEventArgs e)
        {
            CreateLoai createLoaiWindow = new CreateLoai();
            if (createLoaiWindow.ShowDialog() == true) // Kiểm tra nếu người dùng nhấn "OK" hoặc lưu thành công
            {
                LoadComboBoxes();  // Tải lại dữ liệu vào ComboBox sau khi thêm Loại mới
            }
        }

        private void cbLoai_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Xóa các mục hiện tại của cbChuong
            cbChuong.ItemsSource = null;

            // Lấy giá trị ID của LOAI được chọn
            var selectedLoaiID = (int?)cbLoai.SelectedValue;

            if (selectedLoaiID == null)
                return;

            using (var context = new QLSTContext())
            {
                // Truy vấn LOAI từ cơ sở dữ liệu
                var selectedLoai = context.LOAI.FirstOrDefault(loai => loai.ID == selectedLoaiID);

                if (selectedLoai != null)
                {
                    string loaiDongVat = selectedLoai.LoaiDongVat;
                    string phanLoai = selectedLoai.PhanLoai;
                   
                    List<CHUONG> filteredChuong = new List<CHUONG>();

                    // Logic xác định danh sách Chuồng
                    if(selectedLoai.TenLoai.Contains("Rắn"))
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
            }

            // Đặt lại SelectedIndex của cbChuong
            cbChuong.SelectedIndex = -1;
        }

    }
}
