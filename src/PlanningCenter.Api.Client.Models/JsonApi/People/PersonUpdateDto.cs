using System;

namespace PlanningCenter.Api.Client.Models.JsonApi.People
{
    /// <summary>
    /// Person update DTO for JSON:API requests.
    /// </summary>
    public class PersonUpdateDto
    {
        public string Type { get; set; } = "Person";
        public string Id { get; set; } = string.Empty;
        public PersonUpdateAttributesDto Attributes { get; set; } = new();
    }

    /// <summary>
    /// Person update attributes DTO.
    /// </summary>
    public class PersonUpdateAttributesDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MiddleName { get; set; }
        public string? Nickname { get; set; }
        public string? Gender { get; set; }
        public DateTime? Birthdate { get; set; }
        public DateTime? Anniversary { get; set; }
        public string? Status { get; set; }
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
