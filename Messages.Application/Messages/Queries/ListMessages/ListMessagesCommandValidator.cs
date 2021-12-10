﻿using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using FluentValidation;
using Messages.Common;
using Messages.Common.FluentValidation;

namespace Messages.Application.Messages.Queries.ListMessages;

// ReSharper disable once UnusedMember.Global
public class ListMessagesCommandValidator : AbstractValidator<ListMessagesCommand>
{
    public ListMessagesCommandValidator()
    {
        RuleFor(command => command.CreatedAt)
            .IsValidRange<ListMessagesCommand, OptionalDateRange, DateTime?>()
            .WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);
    }
}
