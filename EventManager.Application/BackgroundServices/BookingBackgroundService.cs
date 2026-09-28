using EventManager.Application.Models.DTO.Booking;
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
                var pendingBooking = await GetPendingBooking(cancellationToken);

                if (pendingBooking != null)
                    await ProcessPendingBooking(pendingBooking, cancellationToken);

                await Task.Delay(ProcessingDelay, cancellationToken);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation("BookingBackgroundService stopped gracefully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "BookingBackgroundService stopped due to error");
        }
    }

    private async Task<BookingDto?> GetPendingBooking(CancellationToken cancellationToken)
    {
        try
        {
            return await bookingService.GetPending(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Get pending Booking error");
            return null;
        }
    }

    private async Task ProcessPendingBooking(BookingDto booking, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Start processing Booking '{BookingId}'", booking.Id);

            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            await bookingService.Confirm(booking.Id, cancellationToken);

            logger.LogInformation("Booking '{BookingId}' confirmed", booking.Id);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Process Booking '{BookingId}' error", booking.Id);
        }
    }
}
