using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;
using Messages.Common;
using Messages.Domain.Ids;

namespace Messages.Application.Messages.Queries.ListMessages;

public class ListMessagesCommand : IRequest<ListMessagesResponse>
{
    public ListMessagesCommand(PaginationFilter paginationFilter, IEnumerable<MessageId> ids, RelationshipId relationship, IdentityAddress createdBy, IdentityAddress recipient, OptionalDateRange createdAt, bool unreceived, bool onlyIncoming, bool noBody)
    {
        PaginationFilter = paginationFilter;
        Ids = ids;
        Relationship = relationship;
        CreatedBy = createdBy;
        Recipient = recipient;
        CreatedAt = createdAt;
        Unreceived = unreceived;
        OnlyIncoming = onlyIncoming;
        NoBody = noBody;
    }

    public PaginationFilter PaginationFilter { get; set; }
    public IEnumerable<MessageId> Ids { get; set; }
    public RelationshipId Relationship { get; set; }
    public IdentityAddress CreatedBy { get; set; }
    public IdentityAddress Recipient { get; set; }
    public OptionalDateRange CreatedAt { get; set; }
    public bool Unreceived { get; set; }
    public bool OnlyIncoming { get; set; }
    public bool NoBody { get; set; }
}
