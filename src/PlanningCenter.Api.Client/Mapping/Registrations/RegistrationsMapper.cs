using PlanningCenter.Api.Client.Models.JsonApi.Core;
using PlanningCenter.Api.Client.Models.JsonApi.Registrations;
using PlanningCenter.Api.Client.Models.Registrations;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Models.JsonApi;

namespace PlanningCenter.Api.Client.Mapping.Registrations;

/// <summary>
/// Mapper for Registrations entities between domain models and DTOs.
/// </summary>
public static class RegistrationsMapper
{
    #region Signup Mapping

    /// <summary>
    /// Maps a SignupDto to a Signup domain model.
    /// </summary>
    public static Signup MapToDomain(SignupDto dto)
    {
        return new Signup
        {
            Id = dto.Id,
            Name = dto.Attributes.Name,
            Description = dto.Attributes.Description,
            Archived = dto.Attributes.Archived,
            OpenAt = dto.Attributes.OpenAt,
            CloseAt = dto.Attributes.CloseAt,
            LogoUrl = dto.Attributes.LogoUrl,
            NewRegistrationUrl = dto.Attributes.NewRegistrationUrl,
            RegistrationLimit = dto.Attributes.RegistrationLimit,
            WaitlistEnabled = dto.Attributes.WaitlistEnabled,
            RegistrationCount = dto.Attributes.RegistrationCount,
            WaitlistCount = dto.Attributes.WaitlistCount,
            RequiresApproval = dto.Attributes.RequiresApproval,
            Status = dto.Attributes.Status,
            CategoryId = dto.Relationships?.Category?.Id,
            CampusId = dto.Relationships?.Campus?.Id,
            SignupLocationId = dto.Relationships?.SignupLocation?.Id,
            CreatedAt = dto.Attributes.CreatedAt,
            UpdatedAt = dto.Attributes.UpdatedAt,
            DataSource = "Registrations"
        };
    }

    /// <summary>
    /// Maps a SignupCreateRequest to a JSON:API request.
    /// </summary>
    public static JsonApiRequest<SignupCreateDto> MapCreateRequestToJsonApi(SignupCreateRequest request)
    {
        var dto = new SignupCreateDto
        {
            Type = "Signup",
            Attributes = new SignupCreateAttributesDto
            {
                Name = request.Name,
                Description = request.Description,
                OpenAt = request.OpenAt,
                CloseAt = request.CloseAt,
                LogoUrl = request.LogoUrl,
                RegistrationLimit = request.RegistrationLimit,
                WaitlistEnabled = request.WaitlistEnabled,
                RequiresApproval = request.RequiresApproval
            }
        };

        if (!string.IsNullOrEmpty(request.CategoryId) || !string.IsNullOrEmpty(request.CampusId))
        {
            dto.Relationships = new SignupCreateRelationshipsDto();

            if (!string.IsNullOrEmpty(request.CategoryId))
                dto.Relationships.Category = new RelationshipData { Type = "Category", Id = request.CategoryId };

            if (!string.IsNullOrEmpty(request.CampusId))
                dto.Relationships.Campus = new RelationshipData { Type = "Campus", Id = request.CampusId };
        }

        return new JsonApiRequest<SignupCreateDto> { Data = dto };
    }

    /// <summary>
    /// Maps a SignupUpdateRequest to a JSON:API request.
    /// </summary>
    public static JsonApiRequest<SignupUpdateDto> MapUpdateRequestToJsonApi(string id, SignupUpdateRequest request)
    {
        var dto = new SignupUpdateDto
        {
            Type = "Signup",
            Id = id,
            Attributes = new SignupUpdateAttributesDto
            {
                Name = request.Name,
                Description = request.Description,
                OpenAt = request.OpenAt,
                CloseAt = request.CloseAt,
                LogoUrl = request.LogoUrl,
                RegistrationLimit = request.RegistrationLimit,
                WaitlistEnabled = request.WaitlistEnabled,
                RequiresApproval = request.RequiresApproval,
                Archived = request.Archived
            }
        };

        return new JsonApiRequest<SignupUpdateDto> { Data = dto };
    }

    #endregion

    #region Registration Mapping

    /// <summary>
    /// Maps a RegistrationDto to a Registration domain model.
    /// </summary>
    public static Registration MapToDomain(RegistrationDto dto)
    {
        return new Registration
        {
            Id = dto.Id,
            SignupId = dto.Relationships?.Signup?.Id ?? string.Empty,
            Status = dto.Attributes.Status,
            TotalCost = dto.Attributes.TotalCost,
            AmountPaid = dto.Attributes.AmountPaid,
            PaymentStatus = dto.Attributes.PaymentStatus,
            PaymentMethod = dto.Attributes.PaymentMethod,
            ConfirmationCode = dto.Attributes.ConfirmationCode,
            Notes = dto.Attributes.Notes,
            Email = dto.Attributes.Email,
            FirstName = dto.Attributes.FirstName,
            LastName = dto.Attributes.LastName,
            PhoneNumber = dto.Attributes.PhoneNumber,
            CompletedAt = dto.Attributes.CompletedAt,
            AttendeeCount = dto.Attributes.AttendeeCount,
            RequiresApproval = dto.Attributes.RequiresApproval,
            ApprovalStatus = dto.Attributes.ApprovalStatus,
            ApprovedAt = dto.Attributes.ApprovedAt,
            ApprovedBy = dto.Attributes.ApprovedBy,
            CreatedAt = dto.Attributes.CreatedAt,
            UpdatedAt = dto.Attributes.UpdatedAt,
            DataSource = "Registrations"
        };
    }

    /// <summary>
    /// Maps a RegistrationCreateRequest to a JSON:API request.
    /// </summary>
    public static JsonApiRequest<RegistrationCreateDto> MapCreateRequestToJsonApi(RegistrationCreateRequest request)
    {
        var dto = new RegistrationCreateDto
        {
            Type = "Registration",
            Attributes = new RegistrationCreateAttributesDto
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Notes = request.Notes
            }
        };

        return new JsonApiRequest<RegistrationCreateDto> { Data = dto };
    }

    #endregion

    #region Attendee Mapping

    /// <summary>
    /// Maps an AttendeeDto to an Attendee domain model.
    /// </summary>
    public static Attendee MapToDomain(AttendeeDto dto)
    {
        return new Attendee
        {
            Id = dto.Id,
            SignupId = dto.Relationships?.Signup?.Id ?? string.Empty,
            RegistrationId = dto.Relationships?.Registration?.Id,
            PersonId = dto.Relationships?.Person?.Id,
            FirstName = dto.Attributes.FirstName,
            LastName = dto.Attributes.LastName,
            Email = dto.Attributes.Email,
            PhoneNumber = dto.Attributes.PhoneNumber,
            Birthdate = dto.Attributes.Birthdate,
            Gender = dto.Attributes.Gender,
            Grade = dto.Attributes.Grade,
            School = dto.Attributes.School,
            Status = dto.Attributes.Status,
            OnWaitlist = dto.Attributes.OnWaitlist,
            WaitlistPosition = dto.Attributes.WaitlistPosition,
            CheckedIn = dto.Attributes.CheckedIn,
            CheckedInAt = dto.Attributes.CheckedInAt,
            MedicalNotes = dto.Attributes.MedicalNotes,
            DietaryRestrictions = dto.Attributes.DietaryRestrictions,
            SpecialNeeds = dto.Attributes.SpecialNeeds,
            Notes = dto.Attributes.Notes,
            EmergencyContactId = dto.Relationships?.EmergencyContact?.Id,
            CustomFields = dto.Attributes.CustomFields,
            CreatedAt = dto.Attributes.CreatedAt,
            UpdatedAt = dto.Attributes.UpdatedAt,
            DataSource = "Registrations"
        };
    }

    /// <summary>
    /// Maps an AttendeeCreateRequest to a JSON:API request.
    /// </summary>
    public static JsonApiRequest<AttendeeCreateDto> MapCreateRequestToJsonApi(AttendeeCreateRequest request)
    {
        var dto = new AttendeeCreateDto
        {
            Type = "Attendee",
            Attributes = new AttendeeCreateAttributesDto
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Birthdate = request.Birthdate,
                Gender = request.Gender,
                Grade = request.Grade,
                School = request.School,
                MedicalNotes = request.MedicalNotes,
                DietaryRestrictions = request.DietaryRestrictions,
                SpecialNeeds = request.SpecialNeeds,
                Notes = request.Notes,
                CustomFields = request.CustomFields
            }
        };

        if (!string.IsNullOrEmpty(request.PersonId))
        {
            dto.Relationships = new AttendeeCreateRelationshipsDto
            {
                Person = new RelationshipData { Type = "Person", Id = request.PersonId }
            };
        }

        return new JsonApiRequest<AttendeeCreateDto> { Data = dto };
    }

    /// <summary>
    /// Maps an AttendeeUpdateRequest to a JSON:API request.
    /// </summary>
    public static JsonApiRequest<AttendeeUpdateDto> MapUpdateRequestToJsonApi(string id, AttendeeUpdateRequest request)
    {
        var dto = new AttendeeUpdateDto
        {
            Type = "Attendee",
            Id = id,
            Attributes = new AttendeeUpdateAttributesDto
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                MedicalNotes = request.MedicalNotes,
                DietaryRestrictions = request.DietaryRestrictions,
                SpecialNeeds = request.SpecialNeeds,
                Notes = request.Notes,
                Status = request.Status,
                CustomFields = request.CustomFields
            }
        };

        return new JsonApiRequest<AttendeeUpdateDto> { Data = dto };
    }

    #endregion
}