using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCourses.BLL.Models.DTOs.BaseDTOs
{
    public class BaseAuditableDto<TKey> : BaseDto<TKey> where TKey : IEquatable<TKey>
    {
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; } = string.Empty;
        public DateTime LastModifiedOn { get; set; }
    }
}
