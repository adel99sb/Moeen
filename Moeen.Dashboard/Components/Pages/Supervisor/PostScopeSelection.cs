namespace Moeen.Dashboard.Components.Pages.Supervisor
{
    public sealed record PostScopeOption(string Value, string Label);

    public static class PostScopeSelection
    {
        public const string Public = "public";
        public const string Halqa = "halqa";

        public static IReadOnlyList<PostScopeOption> Options { get; } = new[]
        {
            new PostScopeOption(Public, "عام"),
            new PostScopeOption(Halqa, "خاص بحلقة")
        };

        public static bool IsPublic(string? scope)
            => !string.Equals(scope, Halqa, StringComparison.OrdinalIgnoreCase);

        public static string FromIsPublic(bool isPublic)
            => isPublic ? Public : Halqa;
    }
}
