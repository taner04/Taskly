namespace Api.Features.Shared.Extensions;

internal static class StringExtensions
{
    extension(
        string str)
    {
        public void EnsureLengthInRange<T>(
            int minLength,
            int maxLength,
            string propertyName,
            bool allowNullOrEmpty = true)
        {
            if (string.IsNullOrEmpty(str))
            {
                if (allowNullOrEmpty)
                {
                    return;
                }

                throw new ModelInvalidStringException<T>(propertyName, 0, minLength, maxLength);
            }

            var length = str.Length;
            if (length < minLength || length > maxLength)
            {
                throw new ModelInvalidStringException<T>(propertyName, length, minLength, maxLength);
            }
        }
    }
}