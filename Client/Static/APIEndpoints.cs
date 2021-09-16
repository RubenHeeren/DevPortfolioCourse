namespace Client.Static
{
    internal static class APIEndpoints
    {
#if DEBUG
        internal const string ServerBaseUrl = "https://localhost:5003";
#else
        internal const string ServerBaseUrl = "https://johndoeserver.azurewebsites.net";
#endif

        internal readonly static string s_categories = $"{ServerBaseUrl}/api/categories";
        internal readonly static string s_categoriesWithPosts = $"{ServerBaseUrl}/api/categories/withposts";
        internal readonly static string s_posts = $"{ServerBaseUrl}/api/posts";
        internal readonly static string s_postsDTO = $"{ServerBaseUrl}/api/posts/dto";
        internal readonly static string s_imageUpload = $"{ServerBaseUrl}/api/imageupload";
        internal readonly static string s_signIn = $"{ServerBaseUrl}/api/signin";
    }
}