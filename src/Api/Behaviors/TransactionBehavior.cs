namespace Api.Behaviors;

public sealed class TransactionBehavior<TRequest, TResponse>(
    ApplicationDbContext context
) : Behavior<TRequest, TResponse>
    where TResponse : IErrorOr
{
    public override async ValueTask<TResponse> HandleAsync(
        TRequest request,
        CancellationToken cancellationToken)
    {
        if (request is not ITransactionalRequest)
        {
            return await Next(request, cancellationToken);
        }

        var strategy = context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

            var response = await Next(request, cancellationToken);

            if (!response.IsError)
            {
                await context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            else
            {
                await transaction.RollbackAsync(cancellationToken);
            }

            return response;
        });
    }
}