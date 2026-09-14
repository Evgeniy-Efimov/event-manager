using EventManager.Application.Services.Interfaces;
using EventManager.UnitTests.Fixtures;

namespace EventManager.UnitTests;

public class EventServiceTests : IClassFixture<EventServiceFixture>
{
    private readonly IEventService _eventService;

    public EventServiceTests(EventServiceFixture fixture)
    {
        _eventService = fixture.EventService;
    }

    // TODO: add tests
    // создание события (Create_NewEvent_Success)
    // получение события по ID (GetByID_ExistedEvent_Success)
    // попытка получить событие с несуществующим ID (GetByID_NotExistedEvent_NotFoundException)
    // обновление существующего события (Update_ExistedEvent_Success)
    // попытка обновить событие с несуществующим ID (Update_NotExistedEvent_NotFoundException)
    // удаление существующего события (Delete_ExistedEvent_Success)
    // удаление существующего события (Delete_NotExistedEvent_NotFoundException)
    // получение всех событий (GetList_DefaultParameters_Success)
    // фильтрация по названию (GetList_FilterByName_ReturnsExpected)
    // фильтрация по датам (GetList_FilterByDates_ReturnsExpected)
    // пагинация событий (GetList_Pagination_ReturnsExpected)
    // комбинированная фильтрация (GetList_FilterWithPagination_ReturnsExpected)
}
