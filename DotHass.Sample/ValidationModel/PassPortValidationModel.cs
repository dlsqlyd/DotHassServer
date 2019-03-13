using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DotHass.Sample.ValidationModel
{
    public class PassPortValidationModel
    {
        [Required]
        [StringLength(24, MinimumLength = 6, ErrorMessage = "用户名的长度必须大于6小于24 ")]
        public string Pid { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "密码的长度必须大于6小于24")]
        [DataType(DataType.Password)]
        public string Pwd { get; set; }
    }
}
