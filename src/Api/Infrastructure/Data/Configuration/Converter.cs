using TagId = Api.Features.Tags.Model.TagId;
using TodoId = Api.Features.Todos.Model.TodoId;

namespace Api.Infrastructure.Data.Configuration;

[EfCoreConverter<TodoId>]
[EfCoreConverter<TagId>]
internal sealed partial class EfcVogenIdConverter;