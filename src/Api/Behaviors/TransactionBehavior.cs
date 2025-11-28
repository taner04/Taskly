namespace Api.Behaviors;

public sealed partial class TransactionBehavior<TRequest, TResponse>(
    ApplicationDbContext context
) : Behavior<TRequest, TResponse>
{
    public override async ValueTask<TResponse> HandleAsync(
        TRequest request,
        CancellationToken cancellationToken)
    {
        if (request is not ITransactionalRequest)
        {
            return await Next(request, cancellationToken);
        }

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var response = await Next(request, cancellationToken);
            
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return response;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}