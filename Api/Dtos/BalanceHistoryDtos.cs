using Domain.BalanceHistories;

namespace Api.Dtos;

public record BalanceHistoryDto(
    Guid Id,
    string Details,
    decimal Difference)
{
    public static BalanceHistoryDto FromDomainModel(BalanceHistory balanceHistory)
        => new(
            Id: balanceHistory.Id.Value,
            Details: balanceHistory.Details,
            Difference: balanceHistory.Difference);
}
