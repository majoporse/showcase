using System.ComponentModel.DataAnnotations.Schema;
using SharedKernel.Interfaces;

namespace SharedKernel;

public class BaseEntity
{
    private readonly List<IDomainEvent> _domainEvents = [];
    
    [NotMapped]
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.ToList();
    
    protected void RaiseEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearEvents() => _domainEvents.Clear();
}