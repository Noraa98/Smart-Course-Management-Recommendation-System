using SmartCourses.BLL.Models.DTOs.Response_ResultDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCourses.BLL.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<ServiceResult<DashboardStatsDto>> GetAdminDashboardAsync();
        Task<ServiceResult<InstructorDashboardDto>> GetInstructorDashboardAsync(string instructorId);
        Task<ServiceResult<StudentDashboardDto>> GetStudentDashboardAsync(string studentId);

    }
}
