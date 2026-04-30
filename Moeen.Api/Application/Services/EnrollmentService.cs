using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Requests.Enrollment;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Enrollment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moeen.Api.Application.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        public Task<GeneralResponse> AddTeacherAsync(AddTeacherRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> CancelMembershipAsync(CancelMembershipRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> DeleteParentAsync(DeleteParentRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> DeleteStudentAsync(DeleteStudentRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> DeleteTeacherAsync(DeleteTeacherRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> ExportMembersListAsync(ExportMembersRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> GetAllParentsAsync(GetAllParentsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> GetAllStudentsAsync(GetAllStudentsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> GetAllSupervisorsAsync(GetAllSupervisorsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> GetAllTeachersAsync(GetAllTeachersRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> GetChildrenByParentAsync(GetChildrenByParentRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> GetMemberProfileAsync(GetMemberProfileRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> GetMemberStatisticsAsync(GetMemberStatisticsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> RegisterParentAsync(RegisterParentRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> RegisterStudentAsync(RegisterStudentRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> SearchMembersAsync(SearchMembersRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> UpdateMemberInfoAsync(UpdateMemberInfoRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> UpdateMemberStatusAsync(UpdateMemberStatusRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> UpdateParentInfoAsync(UpdateParentInfoRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> UpdateStudentInfoAsync(UpdateStudentInfoRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> UpdateTeacherInfoAsync(UpdateTeacherInfoRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
