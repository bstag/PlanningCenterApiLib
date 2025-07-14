using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.JsonApi.Core;
using PlanningCenter.Api.Client.Models.JsonApi.People;
using PlanningCenter.Api.Client.Models.People;
using PlanningCenter.Api.Client.Models.Requests;
using System.Collections.Generic;
using System.Linq;

namespace PlanningCenter.Api.Client.Mapping.People
{
    /// <summary>
    /// Mapper for WorkflowCard entities between domain models, DTOs, and request models.
    /// </summary>
    public static class WorkflowCardMapper
    {
        /// <summary>
        /// Maps a WorkflowCardDto to a WorkflowCard domain model.
        /// </summary>
        public static WorkflowCard MapToDomain(WorkflowCardDto dto)
        {
            return new WorkflowCard
            {
                Id = dto.Id,
                Stage = dto.Attributes.Stage ?? string.Empty,
                Status = dto.Attributes.Status ?? string.Empty,
                Notes = dto.Attributes.Notes,
                SnoozeUntil = dto.Attributes.SnoozeUntil,
                DueAt = dto.Attributes.DueAt,
                CompletedAt = dto.Attributes.CompletedAt,
                AssigneeId = dto.Attributes.AssigneeId,
                PersonId = dto.Attributes.PersonId,
                WorkflowId = dto.Attributes.WorkflowId ?? string.Empty,
                StepId = dto.Attributes.StepId,
                CreatedAt = dto.Attributes.CreatedAt,
                UpdatedAt = dto.Attributes.UpdatedAt,
                DataSource = "People"
            };
        }
        
        /// <summary>
        /// Maps a WorkflowCardCreateRequest to a WorkflowCardCreateDto for JSON:API.
        /// </summary>
        public static WorkflowCardCreateDto MapToCreateDto(WorkflowCardCreateRequest request, string workflowId)
        {
            var dto = new WorkflowCardCreateDto
            {
                Type = "WorkflowCard",
                Attributes = new WorkflowCardCreateAttributesDto
                {
                    Notes = request.Notes,
                    DueAt = request.DueAt
                },
                Relationships = new WorkflowCardCreateRelationshipsDto()
            };
            
            // Add person relationship
            if (!string.IsNullOrEmpty(request.PersonId))
            {
                dto.Relationships.Person = new RelationshipData
                {
                    Type = "Person",
                    Id = request.PersonId
                };
            }
            
            // Add assignee relationship if provided
            if (!string.IsNullOrEmpty(request.AssigneeId))
            {
                dto.Relationships.Assignee = new RelationshipData
                {
                    Type = "Person",
                    Id = request.AssigneeId
                };
            }
            
            // Add step relationship if provided
            if (!string.IsNullOrEmpty(request.StepId))
            {
                dto.Relationships.Step = new RelationshipData
                {
                    Type = "WorkflowStep",
                    Id = request.StepId
                };
            }
            
            return dto;
        }
        
        /// <summary>
        /// Maps a WorkflowCardUpdateRequest to a WorkflowCardUpdateDto for JSON:API.
        /// </summary>
        public static WorkflowCardUpdateDto MapToUpdateDto(string id, WorkflowCardUpdateRequest request)
        {
            var dto = new WorkflowCardUpdateDto
            {
                Type = "WorkflowCard",
                Id = id,
                Attributes = new WorkflowCardUpdateAttributesDto
                {
                    Notes = request.Notes,
                    DueAt = request.DueAt,
                    Status = request.Status,
                    SnoozeUntil = request.SnoozeUntil
                }
            };
            
            // Add relationships if needed
            if (!string.IsNullOrEmpty(request.AssigneeId) || !string.IsNullOrEmpty(request.StepId))
            {
                dto.Relationships = new WorkflowCardUpdateRelationshipsDto();
                
                // Add assignee relationship if provided
                if (!string.IsNullOrEmpty(request.AssigneeId))
                {
                    dto.Relationships.Assignee = new RelationshipData
                    {
                        Type = "Person",
                        Id = request.AssigneeId
                    };
                }
                
                // Add step relationship if provided
                if (!string.IsNullOrEmpty(request.StepId))
                {
                    dto.Relationships.Step = new RelationshipData
                    {
                        Type = "WorkflowStep",
                        Id = request.StepId
                    };
                }
            }
            
            return dto;
        }
    }
}
