namespace Moeen.Dashboard.Components.Shared
{
    public sealed class MemberDetailsDialogModel
    {
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public List<MemberDetailsSectionModel> Sections { get; set; } = new();
    }

    public sealed class MemberDetailsSectionModel
    {
        public string Title { get; set; } = string.Empty;
        public List<MemberDetailsItemModel> Items { get; set; } = new();
    }

    public sealed class MemberDetailsItemModel
    {
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public bool Wide { get; set; }
    }
}
