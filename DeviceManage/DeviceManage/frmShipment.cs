﻿using BUS.BusinessObject;
using DTO.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DeviceManage
{
    public partial class frmShipment : Form
    {

        public frmShipment()
        {
            InitializeComponent();
            user = 1;
            Setting();
            LoadForm();
        }
        public frmShipment(int UserId)
        {
            InitializeComponent();
            user = UserId;
            Setting();
            LoadForm();
        }
        #region Model

        public int user = 0;
        public ShipmentModel currentShipment = null;
        public ShipmentDetailModel currentShipmentDetail = null;
        private List<ShipmentModel> listShipment = null;
        private List<ShipmentDetailModel> listShipmentDetails = null;
        BindingSource bs = new BindingSource();
        BindingSource bsDtl = new BindingSource();


        #endregion


        #region Load form

        private void Setting()
        {
            dtp_ImportDate.CustomFormat = "dd/MM/yyyy";
            dtp_ShipmentDate.CustomFormat = "dd/MM/yyyy";
            dtgv_Shipment.AutoGenerateColumns = false;
            dtgv_ShipmentDetail.AutoGenerateColumns = false;
            dtgv_Shipment.AllowUserToAddRows = false;
            dtgv_ShipmentDetail.AllowUserToAddRows = false;
        }

        private void LoadForm()
        {
            LoadShipmentSource();
            if(listShipment.Count > 0)
            {
                currentShipment = listShipment[0];
                
            }
            #region ShipmentDetail
            LoadComboboxInvoice();
            #endregion

            LoadBeginData();
            LoadDataGridViewShipment();
            LoadComboboxBrand();
            LoadShipmentDetailSource();
            LoadDataGridViewShipmentDetail();
            
        }
        private void LoadComboboxInvoice()
        {
            cb_Shipment_Invoice.DisplayMember = "Invoice";
            cb_Shipment_Invoice.ValueMember = "Id";
            cb_Shipment_Invoice.DataSource = listShipment;
        }

        private void LoadBeginData()
        {
            if(currentShipment != null)
            {
                txt_ShipmentName.Text = currentShipment.Name;
                cb_Brand.SelectedValue = currentShipment.BrandId;
                txt_Invoice.Text = currentShipment.Invoice;
                dtp_ImportDate.Value = currentShipment.ImportDate.Value;
                txt_Note.Text = currentShipment.Description;
            }
        }

        private void LoadShipmentSource()
        {
            listShipment = ShipmentBus.SelectAllDynamicWhere(null, null, null, null, null, null, null, null, null, false, null);
            foreach(ShipmentModel dh in listShipment)
            {
                TeacherModel t = TeacherBus.SelectTeacherById(dh.ApproverId);
                dh.Person = t.FullName;
                dh.TotalPrice = ShipmentBus.GetTotalPrice(dh.Id);
            }
        }

        private void LoadDataGridViewShipment()
        {
            bs.DataSource = listShipment;
            dtgv_Shipment.DataSource = bs;
        }

        private void LoadComboboxBrand()
        {
            List<BrandModel> brand = BrandBus.GetBrandAfterDelete();
            cb_Brand.DisplayMember = "Name";
            cb_Brand.ValueMember = "Id";
            cb_Brand.DataSource = brand;

            cb_DeviceBrand.DisplayMember = "Name";
            cb_DeviceBrand.ValueMember = "Id";
            cb_DeviceBrand.DataSource = brand;
        }

        private void ReloadShipmentDetail()
        {
            LoadShipmentDetailSource();
            LoadDataGridViewShipmentDetail();
        }
        private void LoadShipmentDetailSource()
        {
            if (currentShipment != null)
            {
                listShipmentDetails = ShipmentDetailBus.SelectAllDynamicWhere(null, currentShipment.Id, null, null, null, null, false, null,null);
                if(listShipmentDetails.Count>0)
                {
                    foreach (ShipmentDetailModel sdtl in listShipmentDetails)
                    {
                        DeviceModel d = DeviceBus.SelectByPrimaryKey(sdtl.DeviceId);
                        sdtl.DeviceName = d.Name;
                        sdtl.DevicePrice = d.Price.HasValue ? Math.Round(d.Price.Value,1) : 0;
                        sdtl.BrandId = d.BrandId;
                    }
                }
            }
            else
            {
                listShipment = new List<ShipmentModel>();
            }
            currentShipmentDetail = listShipmentDetails.Count > 0 ? listShipmentDetails[0] : null;
        }

        private void LoadDataGridViewShipmentDetail()
        {
            bsDtl.DataSource = listShipmentDetails.ToList();
            dtgv_ShipmentDetail.DataSource = bsDtl;
        }

        #endregion

        #region Create and Update Shipment
        private void btn_Create_Click(object sender, EventArgs e)
        {
            if(CheckInfoShipment())
            {
                try
                {
                    ShipmentModel phieu = GetShipmentData();
                    int id = ShipmentBus.Insert(phieu);
                    phieu.Id = id;
                    listShipment.Add(phieu);
                    LoadDataGridViewShipment();
                }
                catch(Exception ex)
                {
                    MessageClass.Message_Event(ex.Message, SettingClass.TextTitle_ThongBao, true);
                }
            }
        }

        private bool CheckInfoShipment()
        {
            if(String.IsNullOrEmpty(txt_Invoice.Text.Trim()))
            {
                MessageClass.Message_CheckData("Mã lô hàng", SettingClass.TextTitle_Warning);
                return false;
            }

            if (String.IsNullOrEmpty(txt_ShipmentName.Text.Trim()))
            {
                MessageClass.Message_CheckData("Tên phiếu nhập", SettingClass.TextTitle_Warning);
                return false;
            }
            if (dtp_ImportDate.Value.Date > DateTime.Now.Date)
            {
                MessageClass.Message_CheckData("Ngày nhập hàng", SettingClass.TextTitle_Warning);
                return false;
            }
            return true;
        }

        private ShipmentModel GetShipmentData()
        {
            ShipmentModel ship = new ShipmentModel();
            TeacherModel teacher = TeacherBus.SelectTeacherByUserId(user,false);
            //TeacherModel teacher = TeacherBus.SelectTeacherByUserId(1,false);
            ship.ApproverId = teacher != null ? teacher.Id : 1;
            ship.ImportDate = dtp_ImportDate.Value;
            ship.Invoice = txt_Invoice.Text.Trim();
            ship.Name = txt_ShipmentName.Text.Trim();
            ship.Description = txt_Note.Text.Trim();
            ship.BrandId = (int)cb_Brand.SelectedValue;
            ship.IsDeleted = false;
            ship.Status = 0;
            //ship.CreatedUserId = user.Id;
            ship.CreatedUserId = 1;
            return ship;
        }

        private void UpdateListDetail(object sender, ActionAddDeviceFromShipmentEventArgs e)
        {
            //throw new NotImplementedException();
        }
        private void btn_AddDevice_Click(object sender, EventArgs e)
        {
            if(currentShipment!=null)
            {
                //AddDeviceFromShipment frmAddDevice = new AddDeviceFromShipment(currentShipment,user.Id);
                AddDeviceFromShipment frmAddDevice = new AddDeviceFromShipment(currentShipment,1);
                frmAddDevice.addDevice += UpdateListDetail;
                frmAddDevice.Show();
            }
            else
            {
                MessageClass.Message_Event("Chưa chọn phiếu", SettingClass.TextTitle_ThongBao,false);
            }    
        }
        private void btn_Del_Click(object sender, EventArgs e)
        {
            if(listShipment.Count>0)
            {
                if(currentShipment!=null)
                {
                    if (MessageClass.Message_Event_YesNo("xóa phiếu nhập"))
                    {
                        ShipmentModel sh = ShipmentBus.SelectByPrimaryKey(currentShipment.Id);
                        sh.IsDeleted = true;
                        ShipmentBus.Delete(sh.Id);
                        listShipment.Remove(currentShipment);
                        btn_Del.Enabled = false;
                        LoadDataGridViewShipment();
                        currentShipment = null;
                        dtgv_Shipment.ClearSelection();
                    }
                }
            }
        }

        private void btn_Update_Click(object sender, EventArgs e)
        {
            if (CheckInfoShipment())
            {
                try
                {
                    ShipmentModel phieu = GetShipmentData();
                    ShipmentBus.Update(phieu);
                    
                    listShipment.Add(phieu);
                    LoadDataGridViewShipment();
                }
                catch (Exception ex)
                {
                    MessageClass.Message_Event(ex.Message, SettingClass.TextTitle_ThongBao, true);
                }
            }
        }




        #endregion


        #region ChangeForm
        private void dtgv_Shipment_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dtgv_Shipment.SelectedCells.Count > 0)
            {
                int Id = (int)dtgv_Shipment.SelectedCells[0].OwningRow.Cells["Id"].Value;
                foreach (ShipmentModel sh in listShipment)
                {
                    if(sh.Id == Id)
                    {
                        currentShipment = sh;
                        btn_Update.Enabled = true;
                        btn_Del.Enabled = true;
                        LoadBeginData();
                        ReloadShipmentDetail();
                    }
                }
            }
        }
        #endregion

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void dtp_ImportDate_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dtgv_ShipmentDetail_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(dtgv_ShipmentDetail.SelectedRows.Count > 0)
            {
                int id = (int) dtgv_ShipmentDetail.SelectedCells[0].OwningRow.Cells["ShipmentDetailId"].Value;
                foreach(ShipmentDetailModel sd in listShipmentDetails)
                {
                    if(sd.Id == id)
                    {
                        txt_DevicePrice.Text = sd.DevicePrice + "";
                        txtDevice.Text = sd.DeviceName;
                        cb_DeviceBrand.SelectedValue = sd.BrandId.Value;
                    }
                }
            }
        }

        private void dtgv_Shipment_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == dtgv_Shipment.Columns["Status"].Index)
            {
                if (e.Value != null)
                {
                    int status = (int)e.Value;
                    if (status == 0)
                    {
                        e.Value = "Mới";
                    }
                    else
                        e.Value = "Hoàn thành";
                }
            }

        }
    }
}
