namespace OpenAIMemoryCardTailzor.Models
{
    public class MemoryCard
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Content { get; set; }
        public string? Description { get; set; }
        public string? VideoUrl { get; set; }
        public bool IsFlipped { get; set; }

        public MemoryCard Clone()
        {
            return new MemoryCard
            {
                Id = this.Id,
                Name = this.Name,
                Content = this.Content,
                Description = this.Description,
                VideoUrl = this.VideoUrl,
                IsFlipped = this.IsFlipped
            };
        }
    }
}
