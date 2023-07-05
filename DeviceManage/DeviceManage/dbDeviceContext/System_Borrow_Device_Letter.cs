namespace DeviceManage.dbDeviceContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class System_Borrow_Device_Letter
    {
        public int Id { get; set; }

        public int BorrowerId { get; set; }

        [StringLength(100)]
        public string Note { get; set; }

        public int RoomId { get; set; }

        public int? ManagerDeviceId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? DueDate { get; set; }

        public int? CreatedUserId { get; set; }

        public bool? IsDeleted { get; set; }

        public int? Status { get; set; }
    }
}
