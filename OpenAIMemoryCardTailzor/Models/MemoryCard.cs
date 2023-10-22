namespace OpenAIMemoryCardTailzor.Models
{
    public class MemoryCard
    {
        public Guid Id { get; set; }
        public string? Category { get; set; }
        public string? Name { get; set; }
        public string? Content { get; set; }
        public string? Description { get; set; }
        public string? VideoUrl { get; set; }
        public bool IsFlipped { get; set; }
        public bool IsCollected { get; set; }

        public MemoryCard Clone()
        {
            return new MemoryCard
            {
                Id = this.Id,
                Name = this.Name,
                Category = this.Category,
                Content = this.Content,
                Description = this.Description,
                VideoUrl = this.VideoUrl,
                IsFlipped = this.IsFlipped
            };
        }
    }
}
