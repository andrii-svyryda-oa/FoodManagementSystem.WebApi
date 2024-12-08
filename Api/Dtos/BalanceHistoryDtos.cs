using Domain.BalanceHistories;

namespace Api.Dtos;

public record PaginatedData<T>(IReadOnlyList<T> Data, int Count);

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
