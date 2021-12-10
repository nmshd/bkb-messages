using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;
using Messages.Common;
using Messages.Domain.Entities;
using Messages.Domain.Ids;
using Microsoft.EntityFrameworkCore;

namespace Messages.Application.Extensions;

public static class MessagesQueryableExtensions
{
    public static IQueryable<Message> IncludeAllReferences(this IQueryable<Message> messages, IdentityAddress addressOfActiveIdentity)
    {
        return messages
            .Include(m => m.Recipients)
            .Include(m => m.Attachments);
    }

    public static async Task<Message> FirstWithId(this IQueryable<Message> query, MessageId id, CancellationToken cancellationToken)
    {
        var message = await query.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        if (message == null)
            throw new NotFoundException(nameof(Message));

        return message;
    }

    public static IQueryable<Message> WithSenderOrRecipient(this IQueryable<Message> query, IdentityAddress identity)
    {
        return query.Where(m => m.CreatedBy == identity || m.Recipients.Any(r => r.Address == identity));
    }

    public static IQueryable<Message> WithIdsIn(this IQueryable<Message> query, IEnumerable<MessageId> ids)
    {
        return query.Where(m => ids.Contains(m.Id));
    }

    public static IQueryable<Message> WithRecipient(this IQueryable<Message> query, IdentityAddress identity)
    {
        return query.Where(m => m.Recipients.Any(r => r.Address == identity));
    }

    public static IQueryable<Message> DoNotSendBeforePropertyIsNotInTheFuture(this IQueryable<Message> query)
    {
        return query.Where(m => !m.DoNotSendBefore.HasValue || m.DoNotSendBefore <= SystemTime.UtcNow);
    }

    public static IQueryable<Message> UnreceivedOfSpecificRecipient(this IQueryable<Message> query, IdentityAddress recipient)
    {
        return query.Where(m => m.Recipients.Any(r => r.ReceivedAt == null && r.Address == recipient));
    }

    public static IQueryable<Message> Unreceived(this IQueryable<Message> query)
    {
        return query.Where(m => m.Recipients.Any(r => r.ReceivedAt == null));
    }

    public static IQueryable<Message> ToASpecificRecipient(this IQueryable<Message> query, IdentityAddress recipient)
    {
        return query.Where(m => m.Recipients.Any(r => r.Address == recipient));
    }

    public static IQueryable<Message> WithASpecificRecipientWhoDidNotReceiveTheMessage(this IQueryable<Message> query, IdentityAddress recipient)
    {
        return query.Where(m => m.Recipients.Any(r => r.Address == recipient && !r.ReceivedAt.HasValue));
    }

    public static IQueryable<Message> FromASpecificSender(this IQueryable<Message> query, IdentityAddress sender)
    {
        return query.Where(m => m.CreatedBy == sender);
    }

    public static IQueryable<Message> OfASpecificRelationship(this IQueryable<Message> query, RelationshipId relationshipId)
    {
        return query.Where(m => m.Recipients.Any(r => r.RelationshipId == relationshipId));
    }

    public static IQueryable<Message> CreatedAt(this IQueryable<Message> query, OptionalDateRange createdAt)
    {
        if (createdAt == null)
            return query;

        if (createdAt.From != default)
            query = query.Where(r => r.CreatedAt >= createdAt.From);

        if (createdAt.To != default)
            query = query.Where(r => r.CreatedAt <= createdAt.To);

        return query;
    }

    public static IQueryable<Message> CreatedAfterASpecificTime(this IQueryable<Message> query, DateTime start)
    {
        return query.Where(m => m.CreatedAt >= start);
    }

    public static IQueryable<Message> CreatedBeforeASpecificTime(this IQueryable<Message> query, DateTime end)
    {
        return query.Where(m => m.CreatedAt <= end);
    }
}
