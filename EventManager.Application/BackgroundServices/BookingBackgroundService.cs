using EventManager.Application.Models.DTO.Booking;
using EventManager.Application.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventManager.Application.BackgroundServices;

public class BookingBackgroundService(
    ILogger<BookingBackgroundService> logger,
    IBookingService bookingService) : BackgroundService
{
    private static readonly TimeSpan PollingDelay = TimeSpan.FromSeconds(1);
    private static readonly TimeSpan BookingProcessingDelay = TimeSpan.FromSeconds(2);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("BookingBackgroundService is running");

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var pendingBookings = await GetPendingBatchAsync(cancellationToken);

                if (pendingBookings.Count > 0)
                {
                    var tasks = pendingBookings.Select(
                        booking => ProcessPendingBookingAsync(booking, cancellationToken));

                    await Task.WhenAll(tasks);
                }

                await Task.Delay(PollingDelay, cancellationToken);
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

    private async Task<IReadOnlyList<BookingDto>> GetPendingBatchAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await bookingService.GetPendingBatchAsync(cancellationToken: cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Get pending Bookings error");
            return [];
        }
    }

    private async Task ProcessPendingBookingAsync(BookingDto booking, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Start processing Booking '{BookingId}'", booking.Id);

            await Task.Delay(BookingProcessingDelay, cancellationToken);
            await bookingService.ConfirmAsync(booking.Id, cancellationToken);

            logger.LogInformation("Booking '{BookingId}' confirmed", booking.Id);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception processEx)
        {
            logger.LogError(processEx, "Process Booking '{BookingId}' error", booking.Id);

            await RejectFailedBookingAsync(booking.Id, cancellationToken);
        }
    }

    private async Task RejectFailedBookingAsync(Guid bookingId, CancellationToken cancellationToken)
    {
        try
        {
            await bookingService.RejectAsync(bookingId, cancellationToken);

            logger.LogInformation("Booking '{BookingId}' rejected after processing error", bookingId);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,"Failed to reject Booking '{BookingId}' after processing error", bookingId);
        }
    }
}
