using PlanningCenter.Api.Client.Models.JsonApi.People;
using PlanningCenter.Api.Client.Models.People;

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
        public static Workflow MapToDomain(WorkflowDto dto)
        {
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
    }
}
