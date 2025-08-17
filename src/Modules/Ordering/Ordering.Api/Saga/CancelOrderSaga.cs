using MassTransit;

namespace Ordering.Api.Saga;

public class CancelOrderSagaState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }
}


public class CancelOrderSaga : MassTransitStateMachine<CancelOrderSagaState>
{
    public State CancellationStarted { get; private set; }
    public State PaymentRefunded { get; private set; }


}