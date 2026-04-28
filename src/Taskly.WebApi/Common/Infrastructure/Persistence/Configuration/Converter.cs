using AttachmentId = Taskly.WebApi.Features.Attachments.Common.Models.AttachmentId;
using TagId = Taskly.WebApi.Features.Tags.Common.Models.TagId;
using UserId = Taskly.WebApi.Features.Users.Common.Models.UserId;

namespace Taskly.WebApi.Common.Infrastructure.Persistence.Configuration;

[EfCoreConverter<TodoId>]
[EfCoreConverter<TagId>]
[EfCoreConverter<AttachmentId>]
[EfCoreConverter<UserId>]
internal sealed partial class EfcVogenIdConverter;