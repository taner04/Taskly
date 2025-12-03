using Api.Features.Attachments.Models;
using Api.Features.Tags.Model;
using Api.Features.Todos.Model;

namespace Api.Infrastructure.Data.Configuration;

[EfCoreConverter<TodoId>]
[EfCoreConverter<TagId>]
[EfCoreConverter<AttachmentId>]
internal sealed partial class EfcVogenIdConverter;