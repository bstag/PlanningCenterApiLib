using System;

namespace PlanningCenter.Api.Client.Models.JsonApi.People
{
    /// <summary>
    /// Person creation DTO for JSON:API requests.
    /// </summary>
    public class PersonCreateDto
    {
        public string Type { get; set; } = "Person";
        public PersonCreateAttributesDto Attributes { get; set; } = new();
    }

    /// <summary>
    /// Person creation attributes DTO.
    /// </summary>
    public class PersonCreateAttributesDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string? Nickname { get; set; }
        public string? Gender { get; set; }
        public DateTime? Birthdate { get; set; }
        public DateTime? Anniversary { get; set; }
        public string Status { get; set; } = "active";
        public string? MembershipStatus { get; set; }
        public string? MaritalStatus { get; set; }
        public string? School { get; set; }
        public string? Grade { get; set; }
        public int? GraduationYear { get; set; }
        public string? MedicalNotes { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
    }
}
