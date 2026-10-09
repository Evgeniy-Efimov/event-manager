using EventManager.Application.Constants;
using EventManager.Application.Services.Interfaces;
using EventManager.Domain.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventManager.Application.BackgroundServices;

public class BookingBackgroundService(
    ILogger<BookingBackgroundService> logger,
    IBookingService bookingService,
    IQueue<Booking> bookingQuery) : BackgroundService
{
    private static readonly TimeSpan PollingDelay = TimeSpan.FromSeconds(
        BookingBackgroundServiceConstants.PollingDelaySeconds);
    private static readonly TimeSpan ProcessingDelay = TimeSpan.FromSeconds(
        BookingBackgroundServiceConstants.ProcessingDelaySeconds);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("BookingBackgroundService is running");

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var batch = await GetBookingBatchAsync(cancellationToken);

                if (batch.Count > 0)
                    await ProcessBookingBatchAsync(batch, cancellationToken);

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

    private async ValueTask<IReadOnlyList<Booking>> GetBookingBatchAsync(CancellationToken cancellationToken)
    {
        var batch = new List<Booking>();

        while (batch.Count < BookingBackgroundServiceConstants.BatchSize)
        {
            var booking = await bookingQuery.ReadAsync(cancellationToken);

            if (booking is null)
                break;

            batch.Add(booking);
        }

        return batch;
    }

    private async Task ProcessBookingBatchAsync(IReadOnlyList<Booking> batch, CancellationToken cancellationToken)
    {
        logger.LogInformation("Start processing Bookings batch (size: {BatchSize})", batch.Count);

        await Parallel.ForEachAsync(
            batch,
            new ParallelOptions
            {
                MaxDegreeOfParallelism = BookingBackgroundServiceConstants.MaxParallelTasksCount,
                CancellationToken = cancellationToken
            },
            async (booking, token) =>
            {
                await ProcessBookingAsync(booking, token);
            });
    }

    private async Task ProcessBookingAsync(Booking booking, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Start processing Booking '{BookingId}'", booking.Id);

            await Task.Delay(ProcessingDelay, cancellationToken);
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
