namespace Api.Infrastructure.Data.Configuration;

[EfCoreConverter<TodoId>]
[EfCoreConverter<TagId>]
[EfCoreConverter<AttachmentId>]
internal sealed partial class EfcVogenIdConverter;