using TagId = Taskly.WebApi.Features.Tags.Models.TagId;
using TodoId = Taskly.WebApi.Features.Todos.Models.TodoId;
using UserId = Taskly.WebApi.Features.Users.Models.UserId;

namespace Taskly.WebApi.Common.Infrastructure.Persistence.Configuration;

[EfCoreConverter<TodoId>]
[EfCoreConverter<TagId>]
[EfCoreConverter<AttachmentId>]
[EfCoreConverter<UserId>]
internal sealed partial class EfcVogenIdConverter;