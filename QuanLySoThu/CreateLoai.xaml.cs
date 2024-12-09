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

namespace QuanLySoThu
{
    /// <summary>
    /// Interaction logic for CreateLoai.xaml
    /// </summary>
    public partial class CreateLoai : Window
    {
        public CreateLoai()
        {
            InitializeComponent();
        }
        private void btnSaveLoai_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra tính hợp lệ của ô tên loài
            string tenLoai = txtTenLoai.Text;
            if (string.IsNullOrWhiteSpace(tenLoai))
            {
                MessageBox.Show("Tên loài không được bỏ trống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Khai báo các biến cho kích thước và tuổi thọ
            decimal? kichThuoc = null;
            decimal? tuoiTho = null;

            // Kiểm tra ô kích thước nếu không rỗng và phải là số hợp lệ
            if (!string.IsNullOrWhiteSpace(txtKichThuoc.Text))
            {
                if (!decimal.TryParse(txtKichThuoc.Text, out decimal parsedKichThuoc))
                {
                    MessageBox.Show("Kích thước phải là một số hợp lệ.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                kichThuoc = parsedKichThuoc;
            }

            // Kiểm tra ô tuổi thọ nếu không rỗng và phải là số hợp lệ
            if (!string.IsNullOrWhiteSpace(txtTuoiTho.Text))
            {
                if (!decimal.TryParse(txtTuoiTho.Text, out decimal parsedTuoiTho))
                {
                    MessageBox.Show("Tuổi thọ phải là một số hợp lệ.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                tuoiTho = parsedTuoiTho;
            }

            // Lấy giá trị từ các TextBox
            string moTaNgoaiHinh = txtMoTaNgoaiHinh.Text;
            string thoiQuenAnUong = txtThoiQuenAnUong.Text;
            string mts = txtMTS.Text;

            // Lấy giá trị từ ComboBox Loại Động Vật và Phân Loại
            string loaiDongVat = (cbLoaiDongVat.SelectedItem as ComboBoxItem)?.Content.ToString();
            string phanLoai = (cbPhanLoai.SelectedItem as ComboBoxItem)?.Content.ToString();

            // Thực hiện thêm dữ liệu vào cơ sở dữ liệu
            var newLoai = new LOAI
            {
                TenLoai = tenLoai,
                MoTaNgoaiHinh = moTaNgoaiHinh,
                KichThuoc = kichThuoc,
                TuoiTho = tuoiTho,
                ThoiQuenAnUong = thoiQuenAnUong,
                MTSTuNhien = mts,
                LoaiDongVat = loaiDongVat, // Lưu loại động vật
                PhanLoai = phanLoai,       // Lưu phân loại
                IsDeleted = false          // Đánh dấu là chưa xóa
            };

            using (var context = new QLSTContext())
            {
                context.LOAI.Add(newLoai);
                context.SaveChanges();
            }

            this.DialogResult = true; // Báo hiệu thêm thành công
            MessageBox.Show("Loại đã được tạo thành công!");
            this.Close(); // Đóng cửa sổ sau khi tạo thành công
        }
        
        


        private void txtTenLoai_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenLoai.Text))
            {
                icNameError.ToolTip = "Tên loài không được để trống!";
                icNameError.Visibility = Visibility.Visible;
                return;
            }

            icNameError.Visibility = Visibility.Collapsed;
        }

        
        private void cbLoaiDongVat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Xóa các mục hiện tại của ComboBox Phân Loại
            cbPhanLoai.Items.Clear();

            // Lấy giá trị được chọn từ ComboBox Loại Động Vật
            var selectedLoai = (cbLoaiDongVat.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (selectedLoai == "Động Vật Trên Cạn" || selectedLoai == "Động Vật Trên Không")
            {
                // Thêm các mục cho Động vật trên cạn và Động vật trên không
                cbPhanLoai.Items.Add(new ComboBoxItem { Content = "Ăn Thịt" });
                cbPhanLoai.Items.Add(new ComboBoxItem { Content = "Ăn Thực Vật" });
            }
            else if (selectedLoai == "Động Vật Dưới Nước")
            {
                // Thêm các mục cho Động vật dưới nước
                cbPhanLoai.Items.Add(new ComboBoxItem { Content = "Nước Ngọt" });
                cbPhanLoai.Items.Add(new ComboBoxItem { Content = "Nước Mặn" });
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
