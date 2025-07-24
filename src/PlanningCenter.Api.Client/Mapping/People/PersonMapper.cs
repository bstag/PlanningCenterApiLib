using System;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.JsonApi.People;
using PlanningCenter.Api.Client.Models.People;
using PlanningCenter.Api.Client.Models.Requests;

namespace PlanningCenter.Api.Client.Mapping.People
{
    /// <summary>
    /// Mapper for Person entities between domain models, DTOs, and request models.
    /// </summary>
    public static class PersonMapper
    {
        /// <summary>
        /// Maps a PersonDto to a Person domain model.
        /// </summary>
        public static Person MapToDomain(PersonDto dto)
        {
            return new Person
            {
                Id = dto.Id,
                FirstName = dto.Attributes.FirstName ?? string.Empty,
                LastName = dto.Attributes.LastName ?? string.Empty,
                MiddleName = dto.Attributes.MiddleName,
                Nickname = dto.Attributes.Nickname,
                Gender = dto.Attributes.Gender,
                Birthdate = dto.Attributes.Birthdate,
                Anniversary = dto.Attributes.Anniversary,
                Status = dto.Attributes.Status ?? "active",
                MembershipStatus = dto.Attributes.MembershipStatus,
                MaritalStatus = dto.Attributes.MaritalStatus,
                School = dto.Attributes.School,
                Grade = dto.Attributes.Grade,
                GraduationYear = dto.Attributes.GraduationYear,
                MedicalNotes = dto.Attributes.MedicalNotes,
                EmergencyContactName = dto.Attributes.EmergencyContactName,
                EmergencyContactPhone = dto.Attributes.EmergencyContactPhone,
                AvatarUrl = dto.Attributes.AvatarUrl,
                // New fields added from API specification
                Avatar = dto.Attributes.Avatar,
                DemographicAvatarUrl = dto.Attributes.DemographicAvatarUrl,
                Name = dto.Attributes.Name,
                RemoteId = dto.Attributes.RemoteId,
                AccountingAdministrator = dto.Attributes.AccountingAdministrator,
                Child = dto.Attributes.Child,
                GivenName = dto.Attributes.GivenName,
                PassedBackgroundCheck = dto.Attributes.PassedBackgroundCheck,
                CanCreateForms = dto.Attributes.CanCreateForms,
                CanEmailLists = dto.Attributes.CanEmailLists,
                SiteAdministrator = dto.Attributes.SiteAdministrator,
                PeoplePermissions = dto.Attributes.PeoplePermissions,
                CreatedAt = dto.Attributes.CreatedAt,
                UpdatedAt = dto.Attributes.UpdatedAt,
                DataSource = "People"
            };
        }
        
        /// <summary>
        /// Maps a PersonCreateRequest to a PersonCreateDto for JSON:API.
        /// </summary>
        public static PersonCreateAttributesDto MapToCreateAttributes(PersonCreateRequest request)
        {
            return new PersonCreateAttributesDto
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                MiddleName = request.MiddleName,
                Nickname = request.Nickname,
                Gender = request.Gender,
                Birthdate = request.Birthdate,
                Anniversary = request.Anniversary,
                Status = request.Status,
                MembershipStatus = request.MembershipStatus,
                MaritalStatus = request.MaritalStatus,
                School = request.School,
                Grade = request.Grade,
                GraduationYear = request.GraduationYear,
                MedicalNotes = request.MedicalNotes,
                EmergencyContactName = request.EmergencyContactName,
                EmergencyContactPhone = request.EmergencyContactPhone
            };
        }
        
        /// <summary>
        /// Maps a PersonUpdateRequest to a PersonUpdateDto for JSON:API.
        /// </summary>
        public static PersonUpdateAttributesDto MapToUpdateAttributes(PersonUpdateRequest request)
        {
            return new PersonUpdateAttributesDto
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                MiddleName = request.MiddleName,
                Nickname = request.Nickname,
                Gender = request.Gender,
                Birthdate = request.Birthdate,
                Anniversary = request.Anniversary,
                Status = request.Status,
                MembershipStatus = request.MembershipStatus,
                MaritalStatus = request.MaritalStatus,
                School = request.School,
                Grade = request.Grade,
                GraduationYear = request.GraduationYear,
                MedicalNotes = request.MedicalNotes,
                EmergencyContactName = request.EmergencyContactName,
                EmergencyContactPhone = request.EmergencyContactPhone
            };
        }
    }
}
