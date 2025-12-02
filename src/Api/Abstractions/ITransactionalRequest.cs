namespace Api.Abstractions;

/// <summary>
/// Marker interface indicating that a request should be executed within a transaction.
/// Middlwares or behaviors can check for this interface to enable automatic
/// transaction handling.
/// </summary>
public interface ITransactionalRequest;