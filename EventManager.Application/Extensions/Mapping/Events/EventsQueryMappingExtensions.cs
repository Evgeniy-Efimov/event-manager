using EventManager.Application.Models.DTO.Events;
using EventManager.DataAccess.Constants;
using EventManager.DataAccess.Models.Queries;

namespace EventManager.Application.Extensions.Mapping.Events;

public static class EventsQueryMappingExtensions
{
    public static EventsQuery ToQuery(this EventsRequestDto requestDto)
    {
        return new EventsQuery(
            requestDto.Page ?? PaginationConstants.DefaultPage,
            requestDto.PageSize ?? PaginationConstants.DefaultPageSize,
            requestDto.Title,
            requestDto.From,
            requestDto.To);
    }
}
