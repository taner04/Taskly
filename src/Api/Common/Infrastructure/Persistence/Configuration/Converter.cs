using Api.Features.Users.Model;

namespace Api.Common.Infrastructure.Persistence.Configuration;

[EfCoreConverter<TodoId>]
[EfCoreConverter<TagId>]
[EfCoreConverter<AttachmentId>]
[EfCoreConverter<UserId>]
internal sealed partial class EfcVogenIdConverter;