using MediatR;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartCastle.Services.Core.Common.EventHandlers
{
    public class RetryDecorator<TNotification> : INotificationHandler<TNotification> where TNotification : INotification
    {
        private readonly INotificationHandler<TNotification> _inner;
        private readonly IAsyncPolicy _retryPolicy;

        public RetryDecorator(INotificationHandler<TNotification> inner) 
        {
            _inner = inner;
            _retryPolicy = Policy.Handle<ArgumentOutOfRangeException>()
                .WaitAndRetryAsync(3,
                    i => TimeSpan.FromSeconds(i));
        }
        public Task Handle(TNotification notification, CancellationToken cancellationToken)
        {
            return _retryPolicy.ExecuteAsync(() => _inner.Handle(notification, cancellationToken));
        }
    }
}
