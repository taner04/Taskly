using Api.Behaviors.Logger;

[assembly: Behaviors(
    typeof(LoggingBehavior<,>),
    typeof(ValidationBehavior<,>)
)]

[assembly: VogenDefaults(
    conversions: Conversions.Default | Conversions.EfCoreValueConverter)]