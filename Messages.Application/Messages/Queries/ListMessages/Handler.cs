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

        var messagesQuery = _dbContext
            .Set<Message>()
            .AsQueryable()
            .IncludeAllReferences(addressOfActiveIdentity);

        if (request.Ids.Any())
            messagesQuery = messagesQuery.WithIdsIn(request.Ids);

        if (request.OnlyIncoming)
            messagesQuery = messagesQuery.WithRecipient(addressOfActiveIdentity);
        else
            messagesQuery = messagesQuery.WithSenderOrRecipient(identityAddress);

        messagesQuery = messagesQuery.DoNotSendBeforePropertyIsNotInTheFuture();

        if (request.Unreceived)
        {
            if (request.Recipient != null)
                messagesQuery = messagesQuery.UnreceivedOfSpecificRecipient(request.Recipient);
            else
                messagesQuery = messagesQuery.Unreceived();
        }
        else
        {
            if (request.Recipient != null)
                messagesQuery = messagesQuery.ToASpecificRecipient(request.Recipient);
        }

        if (request.CreatedBy != null)
            messagesQuery = messagesQuery.FromASpecificSender(request.CreatedBy);

        if (request.Relationship != null)
            messagesQuery = messagesQuery.OfASpecificRelationship(request.Relationship);

        if (request.CreatedAt != null)
            messagesQuery = messagesQuery.CreatedAt(request.CreatedAt);

        var totalRecords = await messagesQuery.CountAsync();

        var messages = await messagesQuery
            .OrderBy(m => m.CreatedAt)
            .Paged(request.PaginationFilter)
            .ToListAsync();

        return (totalRecords, messages);
    }
}
