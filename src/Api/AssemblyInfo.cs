using Api.Behaviors;
using Api.Behaviors.Logger;

[assembly: Behaviors(
    typeof(LoggingBehavior<,>),
    typeof(ValidationBehavior<,>),
    typeof(UserProvisioningBehavior<,>)
)]

[assembly: VogenDefaults(
    conversions: Conversions.Default | Conversions.EfCoreValueConverter)]