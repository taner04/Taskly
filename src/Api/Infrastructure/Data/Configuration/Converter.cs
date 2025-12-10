using Api.Features.Users.Model;

namespace Api.Infrastructure.Data.Configuration;

[EfCoreConverter<TodoId>]
[EfCoreConverter<TagId>]
[EfCoreConverter<AttachmentId>]
[EfCoreConverter<UserId>]
internal sealed partial class EfcVogenIdConverter;