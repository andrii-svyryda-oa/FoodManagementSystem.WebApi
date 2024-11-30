using Application.OrderItems.Exceptions;
using Domain.BalanceHistories;
using Domain.OrderItems;
using Domain.Users;
using Optional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.BalanceHistories.Exceptions;

public abstract class BalanceHistoryException(BalanceHistoryId id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public BalanceHistoryId BalanceHistoryId { get; } = id;
}

public class BalanceHistoryUserNotFoundException(UserId id)
    : BalanceHistoryException(BalanceHistoryId.Empty(), $"User with id: {id} not found");

public class BalanceHistoryUnknownException(BalanceHistoryId id, Exception innerException)
    : BalanceHistoryException(id, $"Unknown exception for the BalanceHistory with id: {id}", innerException);
