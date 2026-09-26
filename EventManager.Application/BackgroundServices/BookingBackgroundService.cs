using EventManager.Application.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventManager.Application.BackgroundServices;

public class BookingBackgroundService(
    ILogger<BookingBackgroundService> logger,
    IBookingService bookingService) : BackgroundService
{
    private static readonly TimeSpan ProcessingDelay = TimeSpan.FromSeconds(1);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("BookingBackgroundService is running");

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await ProcessPendingBooking(cancellationToken);
                await Task.Delay(ProcessingDelay, cancellationToken);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        finally
        {
            logger.LogInformation("BookingBackgroundService stopped");
        }
    }

    private async Task ProcessPendingBooking(CancellationToken cancellationToken)
    {
        try
        {
            var pendingBooking = await bookingService.GetPending(cancellationToken);

            if (pendingBooking is null)
                return;

            logger.LogInformation("Start processing booking '{BookingId}'", pendingBooking.Id);

            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            await bookingService.Confirm(pendingBooking.Id, cancellationToken);

            logger.LogInformation("Booking '{BookingId}' confirmed", pendingBooking.Id);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "BookingBackgroundService processing error");
        }
    }
}
