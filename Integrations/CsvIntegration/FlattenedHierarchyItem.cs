namespace StoryBuckets.Integrations.CsvIntegration
{
    public class FlattenedHierarchyItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int? ParentId { get; set; }
    }
}
