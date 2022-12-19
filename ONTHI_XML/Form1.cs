using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace ONTHI_XML
{
    public partial class Form1 : Form
    {
        XmlDocument tai_Lieu = new XmlDocument();
        XmlElement nut_Goc;
        int stt = -1;
        List<KHOA> Khoas = new List<KHOA>();
        List<SINHVIEN> Sinhviens = new List<SINHVIEN>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Nạp dữ liệu
            Khoi_tao_du_lieu();
            //Khởi tạo combobox
            khoi_tao_combo();
            btndau.PerformClick();
        }

        private void khoi_tao_combo()
        {
            cbokhoa.DataSource = Khoas;
            cbokhoa.DisplayMember = "TenKH";
            cbokhoa.ValueMember = "MaKH";
        }

        private void Khoi_tao_du_lieu()
        {
            tai_Lieu.Load(@"..\..\DS_SV.xml");
            nut_Goc = tai_Lieu.DocumentElement;
            //Nạp vào khoas
            XmlNodeList DSKHOA = nut_Goc.SelectNodes("//KHOA");
            foreach(XmlElement nut in DSKHOA)
            {
                KHOA kh = new KHOA();
                kh.MaKH = nut.GetAttribute("MaKH");
                kh.TenKH = nut.GetAttribute("TenKH");
                Khoas.Add(kh);
            }
            //Nạp vào Sinhviens
            XmlNodeList DSSV = nut_Goc.SelectNodes("//SINHVIEN");
            foreach(XmlElement nut in DSSV)
            {
                SINHVIEN sv = new SINHVIEN();
                sv.MaSV = nut.GetAttribute("MaSV");
                sv.HoSV = nut.GetAttribute("HoSV");
                sv.TenSV = nut.GetAttribute("TenSV");
                sv.Phai = Boolean.Parse(nut.GetAttribute("Phai"));
                sv.NgaySinh = DateTime.Parse(nut.GetAttribute("NgaySinh"));
                sv.NoiSinh = nut.GetAttribute("NoiSinh");
                sv.HocBong = int.Parse(nut.GetAttribute("HocBong"));
                XmlElement nut_khoa = nut.ParentNode as XmlElement;
                sv.MaKH = nut_khoa.GetAttribute("MaKH");
                Sinhviens.Add(sv);
            }
        }

        private void Gan_du_lieu(int stt)
        {
            SINHVIEN sv = Sinhviens[stt];
            txtmasv.Text = sv.MaSV;
            txtho.Text = sv.HoSV;
            txtten.Text = sv.TenSV;
            txtphai.Text = sv.Phai ? "Nam" : "Nữ";
            txtngaysinh.Text = sv.NgaySinh.ToShortDateString();
            txtnoisinh.Text = sv.NoiSinh;
            cbokhoa.SelectedValue = sv.MaKH;
            txthocbong.Text = sv.HocBong.ToString();
        }
        private void btndau_Click(object sender, EventArgs e)
        {
            stt = 0;
            Gan_du_lieu(stt);
        }

        private void btntruoc_Click(object sender, EventArgs e)
        {
            if (stt == 0)
            {
                MessageBox.Show("Không thể về trước được nữa");
                return;
            }
            stt --;
            Gan_du_lieu(stt);
           
        }

        private void btnsau_Click(object sender, EventArgs e)
        {
            if (stt == Sinhviens.Count - 1)
            {
                MessageBox.Show("Không thể về sau được nữa");
                return;
            }
            stt ++;
            Gan_du_lieu(stt);
            
        }

        private void btncuoi_Click(object sender, EventArgs e)
        {
            stt = Sinhviens.Count-1;
            Gan_du_lieu(stt);
        }

        private void btnthem_Click(object sender, EventArgs e)
        {
            txtmasv.ReadOnly = false;
            foreach(Control ctl in this.Controls)
            {
                if (ctl is TextBox)
                    (ctl as TextBox).Clear();
                else if (ctl is ComboBox)
                    (ctl as ComboBox).SelectedIndex = 0;
            }
            txtmasv.Focus();
        }

        private void btnhuy_Click(object sender, EventArgs e)
        {
            //Xóa trong list sinh vien
            SINHVIEN sv = Sinhviens[stt];
            Sinhviens.Remove(sv);
            btndau.PerformClick();
            //Hủy trong xml
            XmlElement nut_huy = nut_Goc.SelectSingleNode("//SINHVIEN[@MaSV='" + txtmasv.Text + "']") as XmlElement;
            nut_huy.ParentNode.RemoveChild(nut_huy);
        }

        private void btnghi_Click(object sender, EventArgs e)
        {
            SINHVIEN sv;
            //Sửa xong rồi ghi
            if (txtmasv.ReadOnly == true)
            {
                //Sửa trong list sinhvien
                sv = Sinhviens[stt];
                sv.HoSV = txtho.Text;
                sv.TenSV = txtten.Text;
                sv.Phai = txtphai.Text.ToUpper() == "NAM" ? true : false;
                sv.NgaySinh =  DateTime.Parse(txtngaysinh.Text);
                sv.NoiSinh = txtnoisinh.Text;
                sv.MaKH = cbokhoa.SelectedValue.ToString();
                sv.HocBong = int.Parse(txthocbong.Text);

                //sửa trong xml
                XmlElement nut_sua=nut_Goc.SelectSingleNode("//SINHVIEN[@MaSV='" + txtmasv.Text + "']") as XmlElement;
                nut_sua.SetAttribute("HoSV", txtho.Text);
                nut_sua.SetAttribute("TenSV", txtten.Text);
                nut_sua.SetAttribute("Phai", txtphai.Text.ToUpper() == "NAM" ? "true" : "false");
                nut_sua.SetAttribute("NgaySinh", txtngaysinh.Text);
                nut_sua.SetAttribute("NoiSinh", txtnoisinh.Text);
                nut_sua.SetAttribute("MaKH", cbokhoa.SelectedValue.ToString());
                nut_sua.SetAttribute("HocBong", txthocbong.Text.ToString());
            }
            else
            {
                //Thêm trong list sinhvien
                sv = new SINHVIEN();
                sv = Sinhviens[stt];
                sv.MaSV = txtmasv.Text;
                sv.HoSV = txtho.Text;
                sv.TenSV = txtten.Text;
                sv.Phai = txtphai.Text.ToUpper() == "NAM" ? true : false;
                sv.NgaySinh = DateTime.Parse(txtngaysinh.Text);
                sv.NoiSinh = txtnoisinh.Text;
                sv.MaKH = cbokhoa.SelectedValue.ToString();
                sv.HocBong = int.Parse(txthocbong.Text);
                Sinhviens.Add(sv);
                //Thêm trong xml
                XmlElement nut_them = tai_Lieu.CreateElement("SINHVIEN");
                XmlElement nut_cha = nut_Goc.SelectSingleNode("//KHOA[@MaKH='" + cbokhoa.SelectedValue.ToString() + "']") as XmlElement;
                nut_cha.AppendChild(nut_them);
                nut_them.SetAttribute("MaSV", txtmasv.Text);
                nut_them.SetAttribute("HoSV", txtho.Text);
                nut_them.SetAttribute("TenSV", txtten.Text);
                nut_them.SetAttribute("Phai", txtphai.Text.ToUpper() == "NAM" ? "true" : "false");
                nut_them.SetAttribute("NgaySinh", txtngaysinh.Text);
                nut_them.SetAttribute("NoiSinh", txtnoisinh.Text);
                nut_them.SetAttribute("HocBong", txthocbong.Text.ToString());
                txtmasv.ReadOnly = true;
            }
        }

        private void btnkhong_Click(object sender, EventArgs e)
        {
            txtmasv.ReadOnly = true;
            Gan_du_lieu(stt);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            tai_Lieu.Save(@"..\..\DS_SV.xml");
        }
    }
}
