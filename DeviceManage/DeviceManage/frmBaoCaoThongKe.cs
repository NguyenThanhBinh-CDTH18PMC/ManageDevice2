﻿using DeviceManage.Reportting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeviceManage
{
    public partial class frmBaoCaoThongKe : Form
    {
        public frmBaoCaoThongKe()
        {
            InitializeComponent();
            ThongKeTheoPhong();
            ThongKeTheoTrangThai();
        }

        private void ThongKeTheoPhong()
        {
            ThongKeThietBiTheoRoom thietBiTheoPhong = new ThongKeThietBiTheoRoom();
            thietBiTheoPhong.TopLevel = false;
            thietBiTheoPhong.AutoScroll = true;
            thietBiTheoPhong.Dock=DockStyle.Fill;
            this.panel3.Controls.Add(thietBiTheoPhong);
            thietBiTheoPhong.Show();
        }

        private void ThongKeTheoTrangThai()
        {
            reportThietBiTheoThangThai thietBiTheoThangThai = new reportThietBiTheoThangThai();
            thietBiTheoThangThai.TopLevel = false;
            thietBiTheoThangThai.AutoScroll = true;
            thietBiTheoThangThai.Dock=DockStyle.Fill;
            this.panel2.Controls.Add(thietBiTheoThangThai);
            thietBiTheoThangThai.Show();
        }
    }
}
