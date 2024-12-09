using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using QuanLySoThu.Models;
using QuanLySoThu.TableViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static MaterialDesignThemes.Wpf.Theme;

namespace QuanLySoThu
{
    /// <summary>
    /// Interaction logic for QuanLyConVat.xaml
    /// </summary>
    public partial class QuanLyConVat : UserControl
    {
        public ObservableCollection<ConVatViewModel> ConVats { get; set; } = new ObservableCollection<ConVatViewModel>();
        private static Random random = new Random();
        public QuanLyConVat()
        {
            InitializeComponent();
            DataContext = this; // Gán DataContext để binding
            LoadData();
        }
        public void LoadData()
        {
            try
            {
                using (var context = new QLSTContext())
                {
                    // Tải dữ liệu từ ConVat và liên kết với DOCGIA hoặc ADMIN dựa trên IDPhanQuyen
                    var convats = context.CONVAT
                        .Where(cv => !cv.IsDeleted)
                        .Include(cv => cv.IDLoaiNavigation) // Tải dữ liệu từ bảng LOAI
                        .Include(cv => cv.IDChuongNavigation) // Tải dữ liệu từ bảng CHUONG
                        .Select(cv => new ConVatViewModel
                        {
                            MaConVat = cv.MaConVat,
                            TenConVat = cv.TenConVat,
                            GioiTinh = cv.GioiTinh,
                            IDLoai = cv.IDLoai,
                            IDChuong = cv.IDChuong,
                            Loai = cv.IDLoaiNavigation.TenLoai,
                            Chuong = cv.IDChuongNavigation.TenChuong,
                            SucKhoe = cv.TrangThaiSucKHoe,
                            Tuoi = Math.Round((DateTime.Now - cv.NgaySinh).TotalDays / 365.25, 2), // Lấy 2 chữ số thập phân
                            NgaySinh = cv.NgaySinh,
                            NgayNhap = cv.NgayNhap,
                            BgColor = GenerateRandomColor(),
                            AnhConVat = cv.AnhConVat
                        })
                        .ToList();

                    // Thêm dữ liệu vào ObservableCollection
                    ConVats.Clear(); // Xóa dữ liệu cũ (nếu có)
                    foreach (var convat in convats)
                    {
                        ConVats.Add(convat);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private static string GenerateRandomColor()
        {
            int red = random.Next(0, 256);
            int green = random.Next(0, 256);
            int blue = random.Next(0, 256);

            // Chuyển đổi các giá trị RGB thành mã màu hex
            return $"#{red:X2}{green:X2}{blue:X2}";
        }
        private void btnLamMoi_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Đã cập nhật thông tin");
            LoadData();
        }
        

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as System.Windows.Controls.CheckBox;
            if (checkBox?.DataContext is ConVatViewModel convat)
            {
                convat.IsChecked = true;

                // Thêm dòng vào SelectedItems nếu chưa có
                if (!dgconvat.SelectedItems.Contains(convat))
                {
                    dgconvat.SelectedItems.Add(convat);
                }
            }

            // Đảm bảo SelectedItems không bị xóa
            foreach (var item in dgconvat.Items)
            {
                if (item is ConVatViewModel convatInList && convatInList.IsChecked && !dgconvat.SelectedItems.Contains(convatInList))
                {
                    dgconvat.SelectedItems.Add(convatInList);
                }
            }
            // Kiểm tra nếu tất cả các dòng đã được chọn
            
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Lấy checkbox bị bỏ tick
            var checkbox = sender as System.Windows.Controls.CheckBox;
            if (checkbox != null)
            {
                // Lấy item (dòng) tương ứng với checkbox
                var item = checkbox.DataContext as ConVatViewModel;
                if (item != null)
                {
                    // Cập nhật thuộc tính IsChecked của ConVatViewModel
                    item.IsChecked = false;

                    // Loại bỏ dòng khỏi danh sách SelectedItems
                    if (dgconvat.SelectedItems.Contains(item))
                    {
                        dgconvat.SelectedItems.Remove(item); // Loại bỏ dòng
                    }
                }
            }
            // Kiểm tra nếu tất cả các dòng đã được chọn
            
        }
        // Kiểm tra và cập nhật trạng thái checkbox ở header
       

        // Tìm checkbox header trong DataGrid

        
        private void HeaderCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            
            foreach (var item in dgconvat.Items)
            {
                if (item is ConVatViewModel convat)
                {
                    convat.IsChecked = true;

                    // Thêm tất cả các dòng vào SelectedItems
                    if (!dgconvat.SelectedItems.Contains(convat))
                    {
                        dgconvat.SelectedItems.Add(convat);
                    }
                    
                    
                }
            }
            
        }
        

        private void HeaderCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var item in dgconvat.Items)
            {
                if (item is ConVatViewModel convat)
                {
                    convat.IsChecked = false;

                    // Xóa tất cả các dòng khỏi SelectedItems
                    if (dgconvat.SelectedItems.Contains(convat))
                    {
                        dgconvat.SelectedItems.Remove(convat);
                    }
                    
                }
            }
        }

        private void DeleteSelectedItems()
        {
            // Lấy danh sách các dòng đang được chọn
            var selectedconvats = new List<ConVatViewModel>();
            foreach (var selectedItem in dgconvat.SelectedItems)
            {

                if (selectedItem is ConVatViewModel convat)
                {

                    selectedconvats.Add(convat);
                    // Thêm mục vào danh sách xóa
                }
            }

            if (selectedconvats.Any())
            {
                try
                {
                    using (var context = new QLSTContext())
                    {
                        // Dùng Transaction để đảm bảo tính toàn vẹn khi thao tác với cơ sở dữ liệu
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            try
                            {
                                // Lặp qua danh sách các tài khoản được chọn
                                foreach (var convat in selectedconvats)
                                {

                                    // Tìm tài khoản trong cơ sở dữ liệu
                                    var convatToDelete = context.CONVAT.FirstOrDefault(a => a.MaConVat == convat.MaConVat);
                                    var chuong = context.CHUONG.FirstOrDefault(c => c.ID == convat.IDChuong);

                                    if (convatToDelete != null)
                                    {
                                        // Đánh dấu tài khoản là đã xóa
                                        convatToDelete.IsDeleted = true;
                                    }
                                    if(chuong != null)
                                    {
                                        chuong.SoLuongConVat -= 1;
                                    } 
                                        
                                }

                                // Lưu thay đổi vào cơ sở dữ liệu
                                context.SaveChanges();

                                // Commit Transaction
                                transaction.Commit();

                                // Xóa các dòng khỏi ObservableCollection để cập nhật giao diện
                                foreach (var convat in selectedconvats)
                                {
                                    ConVats.Remove(convat);
                                }

                                MessageBox.Show("Đã xóa các con vật được chọn thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            catch (Exception ex)
                            {
                                // Nếu có lỗi trong quá trình xóa, rollback transaction
                                transaction.Rollback();
                                MessageBox.Show($"Đã xảy ra lỗi khi xóa dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Đã xảy ra lỗi khi kết nối với cơ sở dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn ít nhất một con vật để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnDetail_Click(object sender, RoutedEventArgs e)
        {
            // Lấy nút Detail được nhấn
            var button = sender as System.Windows.Controls.Button;

            // Kiểm tra nếu DataContext của button là ConVatViewModel
            if (button?.DataContext is ConVatViewModel selectedConVat)
            {
                // Kiểm tra nếu TabControl không null
                if (tcQLCV != null)
                {
                    // Kiểm tra xem Tab với tên con vật này đã tồn tại chưa
                    var existingTab = tcQLCV.Items
                                            .OfType<TabItem>()
                                            .FirstOrDefault(tab => tab.Header?.ToString() == $"Profile - {selectedConVat.TenConVat}");

                    if (existingTab != null)
                    {
                        // Nếu Tab đã tồn tại, chuyển đến Tab đó
                        tcQLCV.SelectedItem = existingTab;
                    }
                    else
                    {
                        // Nếu chưa có Tab, tạo Tab mới
                        var profileTab = new TabItem
                        {
                            Header = $"Profile - {selectedConVat.TenConVat}", // Tiêu đề tab
                            Content = new ChiTietConVat
                            {
                                DataContext = selectedConVat // Truyền ConVatViewModel vào DataContext của ChiTietConVat
                            }
                        };

                        // Thêm Tab vào TabControl
                        tcQLCV.Items.Add(profileTab);

                        // Chuyển sang Tab vừa tạo
                        tcQLCV.SelectedItem = profileTab;
                    }
                }
                else
                {
                    MessageBox.Show("TabControl không được tìm thấy trong UserControl!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void btnDetailLoai_Click(object sender, RoutedEventArgs e)
        {
            // Lấy nút Detail được nhấn
            var button = sender as System.Windows.Controls.Button;

            // Kiểm tra nếu DataContext của button là ConVatViewModel
            if (button?.DataContext is ConVatViewModel selectedConVat)
            {
                // Lấy IDLoai từ dòng được chọn
                int selectedIDLoai = selectedConVat.IDLoai;

                // Kiểm tra nếu TabControl không null
                if (tcQLCV != null)
                {
                    // Kiểm tra xem Tab với tên loại này đã tồn tại chưa
                    var existingTab = tcQLCV.Items
                                            .OfType<TabItem>()
                                            .FirstOrDefault(tab => tab.Header?.ToString() == $"Chi tiết - {selectedConVat.Loai}");

                    if (existingTab != null)
                    {
                        // Nếu Tab đã tồn tại, chuyển đến Tab đó
                        tcQLCV.SelectedItem = existingTab;
                    }
                    else
                    {
                        // Lấy thông tin loại từ cơ sở dữ liệu dựa vào IDLoai
                        LOAI loaiData = GetLoaiByID(selectedIDLoai);

                        // Nếu tìm thấy loại, tạo Tab mới
                        if (loaiData != null)
                        {
                            var detailTab = new TabItem
                            {
                                Header = $"Chi tiết - {loaiData.TenLoai}", // Tiêu đề Tab
                                Content = new ChiTietLoai
                                {
                                    DataContext = loaiData // Truyền dữ liệu loại vào DataContext của UserControl ChiTietLoai
                                }
                            };

                            // Thêm Tab vào TabControl
                            tcQLCV.Items.Add(detailTab);

                            // Chuyển sang Tab vừa tạo
                            tcQLCV.SelectedItem = detailTab;
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy thông tin loại tương ứng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("TabControl không được tìm thấy trong UserControl!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private LOAI GetLoaiByID(int idLoai)
        {
            using (var context = new QLSTContext())
            {
                return context.LOAI
                    .Where(l => l.ID == idLoai && !l.IsDeleted)
                    .FirstOrDefault();
            }
        }
        private void btnTaoConVat_Click(object sender, RoutedEventArgs e)
        {
            var createConVatWindow = new CreateConVat();
            createConVatWindow.ReloadRequested += LoadData; // Gọi lại hàm LoadData
            createConVatWindow.ShowDialog();
        }
        private void btnTaoLoai_Click(object sender, RoutedEventArgs e)
        {
            var createLoaiWindow = new CreateLoai();
           
            createLoaiWindow.ShowDialog();
        }

        private void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedItems();  
            
        }
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                using (var context = new QLSTContext())
                {
                    // Lấy giá trị nhập từ TextBox
                    string searchValue = txtSearch.Text.Trim();

                    // Lấy loại tìm kiếm từ ComboBox
                    var selectedSearchType = (cbSearchType.SelectedItem as ComboBoxItem)?.Tag?.ToString();

                    // Nếu chưa chọn loại tìm kiếm hoặc ô tìm kiếm trống
                    if (string.IsNullOrEmpty(selectedSearchType) || string.IsNullOrEmpty(searchValue))
                    {
                        LoadData(); // Hiển thị toàn bộ dữ liệu
                        return;
                    }

                    // Lọc dữ liệu theo loại tìm kiếm
                    IQueryable<CONVAT> query = context.CONVAT
                        .Where(tk => !tk.IsDeleted);

                    switch (selectedSearchType)
                    {
                        case "TenConVat":
                            query = query.Where(cv => cv.TenConVat.Contains(searchValue));
                            break;
                        case "GioiTinh":
                            query = query.Where(cv => cv.GioiTinh.Contains(searchValue));
                            break;
                        case "Loai":
                            query = query.Where(cv => cv.IDLoaiNavigation.TenLoai.Contains(searchValue));
                            break;
                        case "Chuong":
                            query = query.Where(cv => cv.IDChuongNavigation.TenChuong.Contains(searchValue));
                            break;
                        case "SucKhoa":
                            query = query.Where(cv => cv.TrangThaiSucKHoe.Contains(searchValue));
                            break;
                        
                        default:
                            LoadData(); // Nếu không tìm thấy loại, hiển thị toàn bộ dữ liệu
                            return;
                    }

                    // Chuyển dữ liệu từ CONVAT sang ConVatViewModel
                    var result = query
                        .Include(cv => cv.IDLoaiNavigation)
                        .Include(cv => cv.IDChuongNavigation)
                        .Select(cv => new ConVatViewModel
                        {
                            MaConVat = cv.MaConVat,
                            TenConVat = cv.TenConVat,
                            GioiTinh = cv.GioiTinh,
                            Loai = cv.IDLoaiNavigation.TenLoai,
                            Chuong = cv.IDChuongNavigation.TenChuong,
                            SucKhoe = cv.TrangThaiSucKHoe,
                            Tuoi = Math.Floor((DateTime.Now - cv.NgaySinh).TotalDays / 365), // Tính tuổi
                            NgaySinh = cv.NgaySinh,
                            NgayNhap = cv.NgayNhap,
                            AnhConVat = cv.AnhConVat,
                            BgColor = GenerateRandomColor()
                        })
                        .ToList();

                    // Cập nhật dữ liệu trong ObservableCollection
                    ConVats.Clear();
                    foreach (var item in result)
                    {
                        ConVats.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private List<CHUONG> LoadChuongTuongUng(int selectedLoaiID)
        {
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
                    return filteredChuong;

                    // Gán danh sách Chuồng vào cbChuong

                }
                return new List<CHUONG>();
            }
        }
        private void btnThemFile_Click(object sender, RoutedEventArgs e)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx|All Files (*.*)|*.*",
                Title = "Chọn file Excel"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (var package = new ExcelPackage(new FileInfo(openFileDialog.FileName)))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        int soDongThanhCong = 0;
                        int soDongBiLoi = 0;
                        List<string> danhSachLoi = new List<string>();

                        using (var context = new QLSTContext())
                        {
                            for (int row = 2; row <= rowCount; row++)
                            {
                                try
                                {
                                    var tenConVat = worksheet.Cells[row, 1].Text;

                                    var convatcheckten = context.CONVAT.FirstOrDefault(cv => cv.TenConVat == tenConVat);
                                    if(convatcheckten != null)
                                    {
                                        throw new Exception($"Tên con vật đã có trong sở thú.");
                                    }    
                                    var gioiTinh = worksheet.Cells[row, 2].Text;

                                    if (gioiTinh != "Đực" && gioiTinh != "Cái")
                                    {
                                        throw new Exception($"Giới tính không hợp lệ (chỉ chấp nhận 'Đực' hoặc 'Cái'): {gioiTinh}");
                                    }

                                    var ngaySinh = worksheet.Cells[row, 3].Text;
                                    DateTime parsedNgaySinh;
                                    if (string.IsNullOrWhiteSpace(ngaySinh) ||
                                        !DateTime.TryParseExact(ngaySinh, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out parsedNgaySinh))
                                    {
                                        throw new Exception($"Ngày sinh không hợp lệ hoặc không đúng định dạng 'dd/MM/yyyy' : {ngaySinh}");
                                    }

                                    var ngayNhap = worksheet.Cells[row, 4].Text;
                                    DateTime parsedNgayNhap;
                                    if (string.IsNullOrWhiteSpace(ngayNhap) ||
                                        !DateTime.TryParseExact(ngayNhap, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out parsedNgayNhap))
                                    {
                                        throw new Exception($"Ngày nhập không hợp lệ hoặc không đúng định dạng 'dd/MM/yyyy' : {ngayNhap}");
                                    }
                                    // Kiểm tra nếu Ngày sinh lớn hơn Ngày nhập
                                    if (parsedNgaySinh > parsedNgayNhap)
                                    {
                                        throw new Exception($"Ngày sinh ({parsedNgaySinh:dd/MM/yyyy}) không thể lớn hơn ngày nhập ({parsedNgayNhap:dd/MM/yyyy}).");
                                    }
                                    var tenLoai = worksheet.Cells[row, 5].Text;
                                    var loai = context.LOAI.FirstOrDefault(l => l.TenLoai == tenLoai);
                                    if (loai == null)
                                    {
                                        throw new Exception($"Tên Loài không tồn tại trong cơ sở dữ liệu: {tenLoai}");
                                    }
                                    var idLoai = loai.ID;
                                    List<CHUONG> chuongtuongung = LoadChuongTuongUng(idLoai);
                                    
                                    var tenChuong = worksheet.Cells[row, 6].Text;
                                    var chuong = context.CHUONG.FirstOrDefault(c => c.TenChuong == tenChuong);
                                    
                                    if (chuong == null)
                                    {
                                        throw new Exception($"Tên Chuồng không tồn tại trong cơ sở dữ liệu: {tenChuong}");
                                    }

                                    else if (!chuongtuongung.Any(c => c.ID == chuong.ID))
                                    {
                                        throw new Exception($"Tên Chuồng không phù hợp với Loài: {tenChuong}");
                                    }
                                    else if(chuong.SoLuongConVat == chuong.SucChua)
                                    {
                                        throw new Exception($"Chuồng {tenChuong} đã đầy, không thể thêm con vật mới.");
                                    }

                                    var idChuong = chuong.ID;

                                    var trangThaiSucKhoe = worksheet.Cells[row, 7].Text;
                                    if (trangThaiSucKhoe != "Khỏe" &&
                                        trangThaiSucKhoe != "Bệnh" &&
                                        trangThaiSucKhoe != "Thương Tật" &&
                                        trangThaiSucKhoe != "Có Thai")
                                    {
                                        // Xử lý nếu giá trị không hợp lệ
                                        throw new Exception($"Trạng thái sức khỏe {trangThaiSucKhoe} không hợp lệ!");
                                        
                                    }

                                    if (gioiTinh == "Đực" && trangThaiSucKhoe == "Có Thai")
                                    {
                                        throw new Exception($"Con Đực không thể có thai.");
                                    }

                                   
                                    var convatExists = context.CONVAT.Any(cv => cv.TenConVat == tenConVat);
                                    if (convatExists)
                                    {
                                        throw new Exception($"Tên con vật đã tồn tại: {tenConVat}");
                                    }

                                    var convat = new CONVAT
                                    {
                                        TenConVat = tenConVat,
                                        GioiTinh = gioiTinh,
                                        NgaySinh = parsedNgaySinh,
                                        NgayNhap = parsedNgayNhap,
                                        IDLoai = idLoai,
                                        IDChuong = idChuong,
                                        TrangThaiSucKHoe = trangThaiSucKhoe,
                                        IsDeleted = false
                                    };

                                    context.CONVAT.Add(convat);
                                    if (chuong.SoLuongConVat == 0)
                                        chuong.SoLuongConVat = 1;
                                    else
                                        chuong.SoLuongConVat += 1;
                                    soDongThanhCong++;
                                }
                                catch (Exception ex)
                                {
                                    soDongBiLoi++;
                                    danhSachLoi.Add($"Dòng {row}: {ex.Message}");
                                }
                            }

                            context.SaveChanges();
                        }

                        string ketQua = $"Thêm dữ liệu từ file Excel hoàn tất!\n" +
                                        $"Số dòng thêm thành công: {soDongThanhCong}\n" +
                                        $"Số dòng bị lỗi: {soDongBiLoi}";

                        if (soDongBiLoi > 0)
                        {
                            string fileLog = "LogLoiImport.txt";
                            File.WriteAllLines(fileLog, danhSachLoi);
                            ketQua += $"\nChi tiết lỗi được ghi tại: {fileLog}";
                            System.Diagnostics.Process.Start("notepad.exe", fileLog);
                        }

                        MessageBox.Show(ketQua, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    var innerMessage = ex.InnerException != null ? ex.InnerException.Message : "Không có chi tiết bổ sung.";

                    MessageBox.Show($"Đã xảy ra lỗi khi thêm dữ liệu từ file Excel: {ex.Message}\nChi tiết: {innerMessage}",
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                }
            }
        }

        private void btnXuatFile_Click(object sender, RoutedEventArgs e)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Hiển thị hộp thoại lưu file
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx|All Files (*.*)|*.*",
                Title = "Lưu file Excel",
                FileName = "DanhSachConVat.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Tạo file Excel
                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Danh sách con vật");

                        // Thêm tiêu đề cột
                        worksheet.Cells[1, 1].Value = "Mã Con Vật";
                        worksheet.Cells[1, 2].Value = "Tên Con Vật";
                        worksheet.Cells[1, 3].Value = "Giới Tính";
                        worksheet.Cells[1, 4].Value = "Ngày Sinh";
                        worksheet.Cells[1, 5].Value = "Ngày Nhập";
                        worksheet.Cells[1, 6].Value = "Tên Loài";
                        worksheet.Cells[1, 7].Value = "Tên Chuồng";
                        worksheet.Cells[1, 8].Value = "Trạng Thái Sức Khỏe";

                        // Định dạng tiêu đề
                        using (var range = worksheet.Cells[1, 1, 1, 8])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        }

                        // Thêm dữ liệu từ cơ sở dữ liệu
                        int row = 2;
                        using (var context = new QLSTContext())
                        {
                            // Lấy dữ liệu cần thiết từ bảng CONVAT, LOAI, và CHUONG
                            var danhSachConVat = (from cv in context.CONVAT
                                                  where !cv.IsDeleted // Loại bỏ những bản ghi có isDeleted = true
                                                  join loai in context.LOAI on cv.IDLoai equals loai.ID into loaiJoin
                                                  from loai in loaiJoin.DefaultIfEmpty()
                                                  join chuong in context.CHUONG on cv.IDChuong equals chuong.ID into chuongJoin
                                                  from chuong in chuongJoin.DefaultIfEmpty()
                                                  select new
                                                  {
                                                      cv.MaConVat,
                                                      cv.TenConVat,
                                                      cv.GioiTinh,
                                                      cv.NgaySinh,
                                                      cv.NgayNhap,
                                                      TenLoai = loai != null ? loai.TenLoai : "Không xác định",
                                                      TenChuong = chuong != null ? chuong.TenChuong : "Không xác định",
                                                      cv.TrangThaiSucKHoe
                                                  }).ToList();

                            foreach (var conVat in danhSachConVat)
                            {
                                worksheet.Cells[row, 1].Value = conVat.MaConVat;
                                worksheet.Cells[row, 2].Value = conVat.TenConVat;
                                worksheet.Cells[row, 3].Value = conVat.GioiTinh;
                                worksheet.Cells[row, 4].Value = conVat.NgaySinh.ToString("dd/MM/yyyy");
                                worksheet.Cells[row, 5].Value = conVat.NgayNhap.ToString("dd/MM/yyyy");
                                worksheet.Cells[row, 6].Value = conVat.TenLoai;
                                worksheet.Cells[row, 7].Value = conVat.TenChuong;
                                worksheet.Cells[row, 8].Value = conVat.TrangThaiSucKHoe;
                                row++;
                            }
                        }

                        // Tự động điều chỉnh kích thước cột
                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                        // Lưu file
                        var filePath = saveFileDialog.FileName;
                        package.SaveAs(new FileInfo(filePath));

                        MessageBox.Show("Xuất file Excel thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Đã xảy ra lỗi khi xuất file Excel: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void CloseTab_Click(object sender, RoutedEventArgs e)
        {
            // Get the button that was clicked
            var button = sender as System.Windows.Controls.Button;

            // Find the parent TabItem
            if (button != null)
            {
                var tabItem = FindParent<TabItem>(button);

                // Remove the TabItem from the TabControl
                if (tabItem != null)
                {
                    tcQLCV.Items.Remove(tabItem);
                }
            }
        }

        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            // Search up the visual tree to find the parent of type T
            var parent = VisualTreeHelper.GetParent(child);
            while (parent != null && !(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as T;
        }

        
            private void btnThem_Click(object sender, RoutedEventArgs e)
            {
                // Toggle trạng thái của popup
                popupOptions.IsOpen = !popupOptions.IsOpen;
            }
        
    }
}
