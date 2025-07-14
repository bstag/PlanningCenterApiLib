using PlanningCenter.Api.Client.Models.JsonApi.People;
using PlanningCenter.Api.Client.Models.People;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Models.JsonApi;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Mapping.People
{
    /// <summary>
    /// Mapper for Workflow entities between domain models and DTOs.
    /// </summary>
    public static class WorkflowMapper
    {
        /// <summary>
        /// Maps a WorkflowDto to a Workflow domain model.
        /// </summary>
        /// <param name="dto">The DTO to map from</param>
        /// <returns>The mapped domain model</returns>
        /// <exception cref="ArgumentNullException">Thrown when dto is null</exception>
        public static Workflow MapToDomain(WorkflowDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new Workflow
            {
                Id = dto.Id,
                Name = dto.Attributes.Name ?? string.Empty,
                MyReadyCardCount = dto.Attributes.MyReadyCardCount,
                TotalReadyCardCount = dto.Attributes.TotalReadyCardCount,
                CompletedCardCount = dto.Attributes.CompletedCardCount,
                TotalCardsCount = dto.Attributes.TotalCardsCount,
                TotalReadyAndSnoozedCardCount = dto.Attributes.TotalReadyAndSnoozedCardCount,
                TotalStepsCount = dto.Attributes.TotalStepsCount,
                TotalUnassignedStepsCount = dto.Attributes.TotalUnassignedStepsCount,
                TotalUnassignedCardCount = dto.Attributes.TotalUnassignedCardCount,
                TotalOverdueCardCount = dto.Attributes.TotalOverdueCardCount,
                MyOverdueCardCount = dto.Attributes.MyOverdueCardCount,
                MyDueSoonCardCount = dto.Attributes.MyDueSoonCardCount,
                RecentlyViewed = dto.Attributes.RecentlyViewed,
                CampusId = dto.Attributes.CampusId,
                WorkflowCategoryId = dto.Attributes.WorkflowCategoryId,
                CreatedAt = dto.Attributes.CreatedAt,
                UpdatedAt = dto.Attributes.UpdatedAt,
                DeletedAt = dto.Attributes.DeletedAt,
                ArchivedAt = dto.Attributes.ArchivedAt,
                DataSource = "People"
            };
        }

        /// <summary>
        /// Maps a WorkflowCardDto to a WorkflowCard domain model.
        /// </summary>
        /// <param name="dto">The DTO to map from</param>
        /// <returns>The mapped domain model</returns>
        /// <exception cref="ArgumentNullException">Thrown when dto is null</exception>
        public static WorkflowCard MapWorkflowCardToDomain(WorkflowCardDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

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
        /// Maps a WorkflowCardCreateRequest to a JSON:API request.
        /// </summary>
        /// <param name="request">The request to map from</param>
        /// <returns>The mapped JSON:API request</returns>
        /// <exception cref="ArgumentNullException">Thrown when request is null</exception>
        public static JsonApiRequest<WorkflowCardCreateDto> MapCreateRequestToJsonApi(WorkflowCardCreateRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            return new JsonApiRequest<WorkflowCardCreateDto>
            {
                Data = new WorkflowCardCreateDto
                {
                    Type = "WorkflowCard",
                    Attributes = new WorkflowCardCreateAttributesDto
                    {
                        Notes = request.Notes,
                        DueAt = request.DueAt
                    },
                    Relationships = new WorkflowCardCreateRelationshipsDto
                    {
                        Person = !string.IsNullOrEmpty(request.PersonId) ? new RelationshipData { Id = request.PersonId, Type = "Person" } : null,
                        Assignee = request.AssigneeId != null ? new RelationshipData { Id = request.AssigneeId, Type = "Person" } : null,
                        Step = !string.IsNullOrEmpty(request.StepId) ? new RelationshipData { Id = request.StepId, Type = "WorkflowStep" } : null
                    }
                }
            };
        }

        /// <summary>
        /// Maps a WorkflowCardUpdateRequest to a JSON:API request.
        /// </summary>
        /// <param name="request">The request to map from</param>
        /// <returns>The mapped JSON:API request</returns>
        /// <exception cref="ArgumentNullException">Thrown when request is null</exception>
        public static JsonApiRequest<WorkflowCardUpdateDto> MapUpdateRequestToJsonApi(WorkflowCardUpdateRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            return new JsonApiRequest<WorkflowCardUpdateDto>
            {
                Data = new WorkflowCardUpdateDto
                {
                    Type = "WorkflowCard",
                    Attributes = new WorkflowCardUpdateAttributesDto
                    {
                        Notes = request.Notes,
                        DueAt = request.DueAt,
                        Status = request.Status,
                        SnoozeUntil = request.SnoozeUntil
                    },
                    Relationships = new WorkflowCardUpdateRelationshipsDto
                    {
                        Assignee = request.AssigneeId != null ? new RelationshipData { Id = request.AssigneeId, Type = "Person" } : null,
                        Step = request.StepId != null ? new RelationshipData { Id = request.StepId, Type = "WorkflowStep" } : null
                    }
                }
            };
        }
    }
}
