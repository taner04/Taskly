using System.Runtime.CompilerServices;
using Taskly.WebApi.Common.Behaviors;

[assembly: Behaviors(
    typeof(LoggingBehavior<,>),
    typeof(ValidationBehavior<,>),
    typeof(UserProvisioningBehavior<,>)
)]

[assembly: VogenDefaults(
    conversions: Conversions.Default | Conversions.EfCoreValueConverter)]

[assembly: InternalsVisibleTo("Taskly.WebApi.Client")]