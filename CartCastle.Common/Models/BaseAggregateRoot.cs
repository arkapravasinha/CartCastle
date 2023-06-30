using System.Collections.Immutable;
using System.Reflection;

namespace CartCastle.Common.Models
{
    public abstract record BaseAggregateRoot<TA, TKey> : BaseEntity<TKey>, IAggregateRoot<TKey> where TA : class, IAggregateRoot<TKey>
    {
        private readonly Queue<IDomainEvent<TKey>> _events = new Queue<IDomainEvent<TKey>>();
        protected BaseAggregateRoot()
        {

        }
        protected BaseAggregateRoot(TKey id) : base(id) { }
        public long Version { get; private set; }

        public IReadOnlyCollection<IDomainEvent<TKey>> Events => _events.ToImmutableArray();

        public void ClearEvents()
        {
            _events.Clear();
        }

        protected void Append(IDomainEvent<TKey> @event)
        {
            _events.Enqueue(@event);
            this.When(@event);
            this.Version++;
        }

        protected abstract void When(IDomainEvent<TKey> @event);

        #region Factory
        private static readonly ConstructorInfo constructor;
        static BaseAggregateRoot()
        {
            var aggregateType = typeof(TA);
            constructor = aggregateType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                        null, new Type[0], new ParameterModifier[0]);
            if (null == constructor)
                throw new InvalidOperationException($"Unable to find required private parameterless constructor for Aggregate of type '{aggregateType.Name}'");
        }

        public static TA Create(IEnumerable<IDomainEvent<TKey>> events)
        {
            if (events == null || !events.Any())
                throw new ArgumentNullException(nameof(events));
            var result = (TA)constructor.Invoke(new object[0]);
            var baseAggregate = result as BaseAggregateRoot<TA, TKey>;
            if (null != baseAggregate)
                foreach (var @event in events)
                {
                    baseAggregate.Append(@event);
                }
            result.ClearEvents();
            return result;
        }
        #endregion
    }
}
