using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _2001230507_NhanTuManh_B2.Models
{
    public class ContactModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        public string HoTen { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Required(ErrorMessage = "Vui lòng nhập email")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string SoDienThoai { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn chủ đề góp ý")]
        public string ChuDe { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung góp ý")]
        [StringLength(1000, ErrorMessage = "Nội dung quá dài")]
        public string NoiDung { get; set; }
    }
}



