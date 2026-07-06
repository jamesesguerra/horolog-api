namespace horolog_api.Features.Files;

public static class R2Storage
{
    public const string BucketName = "tabled-uploads";
    public const string KeyPrefix = "horolog/";

    public static string ResolveKey(string uriOrKey, string? publicUrl)
    {
        if (!string.IsNullOrEmpty(publicUrl) && uriOrKey.StartsWith(publicUrl))
        {
            return uriOrKey[publicUrl.Length..].TrimStart('/');
        }
        return uriOrKey.TrimStart('/');
    }
}
