using PlanningCenter.Api.Client.Models.JsonApi.Core;
using PlanningCenter.Api.Client.Models.JsonApi.People;
using PlanningCenter.Api.Client.Models.People;
using PlanningCenter.Api.Client.Models.Requests;
using System.Collections.Generic;

namespace PlanningCenter.Api.Client.Mapping.People
{
    /// <summary>
    /// Mapper for PeopleList entities between domain models and DTOs.
    /// </summary>
    public static class PeopleListMapper
    {
        /// <summary>
        /// Maps a PeopleListDto to a PeopleList domain model.
        /// </summary>
        public static PeopleList MapToDomain(PeopleListDto dto)
        {
            return new PeopleList
            {
                Id = dto.Id,
                Name = dto.Attributes.Name ?? string.Empty,
                Description = dto.Attributes.Description,
                PeopleCount = dto.Attributes.PeopleCount,
                Status = dto.Attributes.Status ?? string.Empty,
                IsPublic = dto.Attributes.IsPublic,
                CreatedAt = dto.Attributes.CreatedAt,
                UpdatedAt = dto.Attributes.UpdatedAt,
                DataSource = "People"
            };
        }

        /// <summary>
        /// Maps a ListMemberDto to a ListMember domain model.
        /// </summary>
        public static ListMember MapToDomain(ListMemberDto dto)
        {
            return new ListMember
            {
                Id = dto.Id,
                PersonId = dto.Relationships?.Person?.Id ?? string.Empty,
                ListId = dto.Relationships?.List?.Id ?? string.Empty,
                CreatedAt = dto.Attributes.CreatedAt,
                UpdatedAt = dto.Attributes.UpdatedAt,
                DataSource = "People"
            };
        }

        /// <summary>
        /// Maps a PeopleListCreateRequest to a JSON:API request DTO.
        /// </summary>
        public static Dictionary<string, object> MapToCreateRequest(PeopleListCreateRequest request)
        {
            var jsonApiRequest = new Dictionary<string, object>
            {
                ["data"] = new Dictionary<string, object>
                {
                    ["type"] = "List",
                    ["attributes"] = new Dictionary<string, object>
                    {
                        ["name"] = request.Name,
                        ["is_public"] = request.IsPublic
                    }
                }
            };

            // Add description if provided
            if (!string.IsNullOrEmpty(request.Description))
            {
                var attributes = (Dictionary<string, object>)((Dictionary<string, object>)jsonApiRequest["data"])["attributes"];
                attributes["description"] = request.Description;
            }

            return jsonApiRequest;
        }

        /// <summary>
        /// Maps a PeopleListUpdateRequest to a JSON:API request DTO.
        /// </summary>
        public static Dictionary<string, object> MapToUpdateRequest(string id, PeopleListUpdateRequest request)
        {
            var jsonApiRequest = new Dictionary<string, object>
            {
                ["data"] = new Dictionary<string, object>
                {
                    ["type"] = "List",
                    ["id"] = id,
                    ["attributes"] = new Dictionary<string, object>()
                }
            };

            var attributes = (Dictionary<string, object>)((Dictionary<string, object>)jsonApiRequest["data"])["attributes"];

            // Add fields that are being updated
            if (!string.IsNullOrEmpty(request.Name))
            {
                attributes["name"] = request.Name;
            }

            if (!string.IsNullOrEmpty(request.Description))
            {
                attributes["description"] = request.Description;
            }

            if (request.IsPublic.HasValue)
            {
                attributes["is_public"] = request.IsPublic.Value;
            }

            return jsonApiRequest;
        }

        /// <summary>
        /// Maps a ListMemberCreateRequest to a JSON:API request DTO.
        /// </summary>
        public static Dictionary<string, object> MapToListMemberCreateRequest(ListMemberCreateRequest request)
        {
            var jsonApiRequest = new Dictionary<string, object>
            {
                ["data"] = new Dictionary<string, object>
                {
                    ["type"] = "ListMember",
                    ["relationships"] = new Dictionary<string, object>
                    {
                        ["person"] = new Dictionary<string, object>
                        {
                            ["data"] = new Dictionary<string, string>
                            {
                                ["type"] = "Person",
                                ["id"] = request.PersonId
                            }
                        }
                    }
                }
            };

            return jsonApiRequest;
        }
    }
}
