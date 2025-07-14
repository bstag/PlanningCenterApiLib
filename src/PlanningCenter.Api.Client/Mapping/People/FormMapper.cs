using PlanningCenter.Api.Client.Models.JsonApi.Core;
using PlanningCenter.Api.Client.Models.JsonApi.People;
using PlanningCenter.Api.Client.Models.People;
using PlanningCenter.Api.Client.Models.Requests;
using System.Collections.Generic;

namespace PlanningCenter.Api.Client.Mapping.People
{
    /// <summary>
    /// Mapper for Form entities between domain models and DTOs.
    /// </summary>
    public static class FormMapper
    {
        /// <summary>
        /// Maps a FormDto to a Form domain model.
        /// </summary>
        public static Form MapToDomain(FormDto dto)
        {
            return new Form
            {
                Id = dto.Id,
                Name = dto.Attributes.Name ?? string.Empty,
                Description = dto.Attributes.Description,
                IsArchived = dto.Attributes.IsArchived,
                SubmissionCount = dto.Attributes.SubmissionCount,
                CreatedAt = dto.Attributes.CreatedAt,
                UpdatedAt = dto.Attributes.UpdatedAt,
                ArchivedAt = dto.Attributes.ArchivedAt,
                DataSource = "People"
            };
        }

        /// <summary>
        /// Maps a FormSubmissionDto to a FormSubmission domain model.
        /// </summary>
        public static FormSubmission MapToDomain(FormSubmissionDto dto)
        {
            return new FormSubmission
            {
                Id = dto.Id,
                PersonId = dto.Relationships?.Person?.Id,
                FormId = dto.Relationships?.Form?.Id ?? string.Empty,
                CreatedAt = dto.Attributes.CreatedAt,
                UpdatedAt = dto.Attributes.UpdatedAt,
                FieldData = dto.Attributes.FieldData,
                DataSource = "People"
            };
        }

        /// <summary>
        /// Maps a FormSubmitRequest to a JSON:API request DTO.
        /// </summary>
        public static Dictionary<string, object> MapToSubmitRequest(string formId, FormSubmitRequest request)
        {
            var jsonApiRequest = new Dictionary<string, object>
            {
                ["data"] = new Dictionary<string, object>
                {
                    ["type"] = "FormSubmission",
                    ["attributes"] = new Dictionary<string, object>
                    {
                        ["field_data"] = request.FieldData
                    }
                }
            };

            // Add person relationship if provided
            if (!string.IsNullOrEmpty(request.PersonId))
            {
                var data = (Dictionary<string, object>)jsonApiRequest["data"];
                data["relationships"] = new Dictionary<string, object>
                {
                    ["person"] = new Dictionary<string, object>
                    {
                        ["data"] = new Dictionary<string, string>
                        {
                            ["type"] = "Person",
                            ["id"] = request.PersonId
                        }
                    }
                };
            }

            return jsonApiRequest;
        }
    }
}
