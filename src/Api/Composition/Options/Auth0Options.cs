namespace Api.Composition.Options;

public record Auth0Options(
    string Domain,
    string Audience,
    string ClientId,
    string ClientSecret,
    bool UsePersistentStorage
);