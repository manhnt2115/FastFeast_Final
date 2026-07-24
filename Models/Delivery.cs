namespace _2001230507_NhanTuManh_B2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Delivery
    {
        public int DeliveryID { get; set; }

        public int OrderID { get; set; }

        public int? DriverID { get; set; }

        public DateTime? PickupTime { get; set; }

        public DateTime? DeliveredTime { get; set; }

        [Required]
        [StringLength(50)]
        public string DeliveryStatus { get; set; }

        public string Notes { get; set; }

        public virtual DeliveryDriver DeliveryDriver { get; set; }

        public virtual Order Order { get; set; }
    }
}
