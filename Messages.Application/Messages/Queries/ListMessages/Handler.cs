using AutoMapper;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.BuildingBlocks.Application.Extensions;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;
using Messages.Application.Extensions;
using Messages.Application.Messages.DTOs;
using Messages.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Messages.Application.Messages.Queries.ListMessages;

public class Handler : IRequestHandler<ListMessagesCommand, ListMessagesResponse>
{
    private readonly IDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly MessageService _messageService;
    private readonly IUserContext _userContext;

    public Handler(IDbContext dbContext, IUserContext userContext, IMapper mapper, MessageService messageService)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _mapper = mapper;
        _messageService = messageService;
    }

    public async Task<ListMessagesResponse> Handle(ListMessagesCommand request, CancellationToken cancellationToken)
    {
        var (totalRecords, items) = await FindMessagesOfIdentity(_userContext.GetAddress(), request);

        var response = new ListMessagesResponse(_mapper.Map<IEnumerable<MessageDTO>>(items), request.PaginationFilter, totalRecords);

        if (!request.NoBody)
            await _messageService.FillBodies(response);

        await _messageService.MarkMessagesAsReceived(items, cancellationToken);

        response.PrepareForActiveIdentity(_userContext.GetAddress());

        return response;
    }

    private async Task<(int TotalRecords, IEnumerable<Message> Items)> FindMessagesOfIdentity(IdentityAddress identityAddress, ListMessagesCommand request)
    {
        var addressOfActiveIdentity = _userContext.GetAddress();

        var query = _dbContext
            .Set<Message>()
            .AsQueryable()
            .IncludeAllReferences(addressOfActiveIdentity);

        if (request.Ids.Any())
            query = query.WithIdsIn(request.Ids);

        if (request.OnlyIncoming)
            query = query.WithRecipient(addressOfActiveIdentity);
        else
            query = query.WithSenderOrRecipient(identityAddress);

        query = query.DoNotSendBeforePropertyIsNotInTheFuture();

        if (request.Unreceived)
        {
            if (request.Recipient != null)
                query = query.UnreceivedOfSpecificRecipient(request.Recipient);
            else
                query = query.Unreceived();
        }
        else
        {
            if (request.Recipient != null)
                query = query.ToASpecificRecipient(request.Recipient);
        }

        if (request.CreatedBy != null)
            query = query.FromASpecificSender(request.CreatedBy);

        if (request.Relationship != null)
            query = query.OfASpecificRelationship(request.Relationship);

        if (request.CreatedAt != null)
            query = query.CreatedAt(request.CreatedAt);

        var items = await query
            .OrderBy(m => m.CreatedAt)
            .Paged(request.PaginationFilter)
            .ToListAsync();

        var totalNumberOfItems = items.Count < request.PaginationFilter.PageSize ? items.Count : await query.CountAsync();

        return (totalNumberOfItems, items);
    }
}
