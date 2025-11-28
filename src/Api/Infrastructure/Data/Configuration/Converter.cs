using Api.Features.Tags.Domain;

namespace Api.Infrastructure.Data.Configuration;

[EfCoreConverter<TodoId>]
[EfCoreConverter<TagId>]
internal sealed partial class EfcVogenIdConverter;