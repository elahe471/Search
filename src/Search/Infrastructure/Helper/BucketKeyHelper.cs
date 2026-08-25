namespace Search.Infrastructure.Helper
{
    public static class BucketKeyHelper
    {
        public static string Get(FieldValue key)
        {
            return key.TryGetString(out var value)
                ? value ?? string.Empty
                : string.Empty;
        }
    }
}
