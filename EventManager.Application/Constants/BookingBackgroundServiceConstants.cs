namespace EventManager.Application.Constants;

public static class BookingBackgroundServiceConstants
{
    public const int BatchSize = 10;
    public const int MaxParallelTasksCount = 4;
    public const int PollingDelaySeconds = 1;
    public const int ProcessingDelaySeconds = 2;
}
