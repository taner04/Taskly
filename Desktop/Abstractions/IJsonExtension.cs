namespace Desktop.Abstractions;

/// <summary>
/// Represents a contract for JSON-related extension functionality.
/// </summary>
/// <remarks>
/// This is a marker/abstraction interface intended to identify services or
/// components that provide JSON serialization, deserialization, or related
/// helper utilities. Implementations are expected to encapsulate JSON concerns
/// such as converting objects to and from JSON strings or configuring JSON
/// serializers. Keep the intstaterface minimal to allow flexible implementations.
/// </remarks>
public interface IJsonExtension;