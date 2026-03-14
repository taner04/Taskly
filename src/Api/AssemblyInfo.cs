using Api.Common.Behaviors;
using Api.Common.Behaviors.Logger;

[assembly: Behaviors(
    typeof(LoggingBehavior<,>),
    typeof(ValidationBehavior<,>),
    typeof(UserProvisioningBehavior<,>)
)]

[assembly: VogenDefaults(
    conversions: Conversions.Default | Conversions.EfCoreValueConverter)]