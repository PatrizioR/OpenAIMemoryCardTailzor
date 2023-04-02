using Microsoft.AspNetCore.Components;
using OpenAIMemoryCardTailzor.Models;

namespace OpenAIMemoryCardTailzor.Components
{
    public partial class MemoryManagementComponent
    {
        public string? FilterText { get; set; }
        [Inject]
        public List<MemoryCard> Cards { get; set; } = null!;

        public IEnumerable<MemoryCard> FilteredCards => Cards.Where(card => string.IsNullOrEmpty(FilterText) ||  card.Name?.Contains(FilterText) == true || card.Description?.Contains(FilterText) == true || card.Category?.Contains(FilterText) == true);
    }
}
