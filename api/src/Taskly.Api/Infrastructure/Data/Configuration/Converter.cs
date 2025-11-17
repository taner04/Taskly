using Taskly.Domain.TodoAggregate;
using Vogen;

namespace Taskly.Api.Infrastructure.Data.Configuration;

[EfCoreConverter<TodoId>]
public sealed partial class EfcVogenIdConverter;