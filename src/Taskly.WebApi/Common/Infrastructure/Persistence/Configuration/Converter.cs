using Taskly.WebApi.Features.Users.Model;

namespace Taskly.WebApi.Common.Infrastructure.Persistence.Configuration;

[EfCoreConverter<TodoId>]
[EfCoreConverter<TagId>]
[EfCoreConverter<AttachmentId>]
[EfCoreConverter<UserId>]
internal sealed partial class EfcVogenIdConverter;
