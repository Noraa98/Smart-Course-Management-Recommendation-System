using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCourses.BLL.Models.DTOs.BaseDTOs
{
    public class BaseDto<TKey> where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; } = default!;
    }
}
