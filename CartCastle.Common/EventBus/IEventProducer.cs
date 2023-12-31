﻿namespace CartCastle.Common.EventBus
{
    public interface IEventProducer
    {
        Task DispatchAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default);
    }
}
