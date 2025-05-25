namespace SharedKernel.Interfaces;

public interface IDomainEvent
{
    IIntegrationEvent MapToIntegrationEvent();
}