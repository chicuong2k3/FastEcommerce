using MassTransit;
using Ordering.Contracts;
using Ordering.Core.Entities;
using Ordering.Core.Events;

namespace Ordering.Api.Saga;

public class CheckoutSagaState : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public int Version { get; set; }
    public string CurrentState { get; set; }

    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; }
    public IEnumerable<OrderItem> OrderItems { get; set; }
}

public class CheckoutStateMachine : MassTransitStateMachine<CheckoutSagaState>
{

    public State ProcessingPayment { get; private set; }
    public State ReservingStock { get; private set; }
    public State Completed { get; private set; }
    public State Failed { get; private set; }

    public Event<OrderCreatedIntegrationEvent> OrderCreated { get; set; }

    public CheckoutStateMachine()
    {
        Event(() => OrderCreated, x => x.CorrelateById(m => m.Message.OrderId));


        InstanceState(s => s.CurrentState);

        Initially(When(OrderCreated).Publish(context => new OrderCreatedIntegrationEvent(
            context.Message.OrderId,
            context.Message.CustomerId,
            context.Message.TotalAmount)));
    }

}