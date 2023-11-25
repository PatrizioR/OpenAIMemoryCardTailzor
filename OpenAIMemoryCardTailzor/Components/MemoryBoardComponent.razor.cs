using Microsoft.AspNetCore.Components;
using OpenAIMemoryCardTailzor.Models;
using System.Runtime.CompilerServices;
using System.Linq;
namespace OpenAIMemoryCardTailzor.Components
{
    public partial class MemoryBoardComponent
    {
        [Inject] HttpClient Client { get; set; } = null!;
        [Inject]
        public List<MemoryCard> Cards { get; set; } = null!;
        public List<MemoryCard>? PlayCards { get; set; }
        private int _cardCount = 16;
        private string? _cardCategory = null;
        private bool isProcessing = false;
        private Player? _player1;
        private Player? _player2;
        private Player? _currentPlayer;
        private MemoryCard? _selectedCard;
        private MemoryCard? _selectedSecondCard;
        private bool _isGameOver;
        private bool _isNameEntered;
        protected override async Task OnInitializedAsync()
        {
            InitializeGame();
            await base.OnInitializedAsync();
        }
        private void InitializeGame(bool keepNames = false)
        {
            string? player1Name = _player1?.Name;
            string? player2Name = _player2?.Name;

            _player1 = new Player();
            _player2 = new Player();
            if (keepNames)
            {
                _player1.Name = player1Name ?? "";
                _player2.Name = player2Name ?? "";
            }
            _currentPlayer = _player1;
            _isGameOver = false;
            _isNameEntered = false;
            _selectedCard = null;
            _selectedSecondCard = null;
        }
        private void RestartGame()
        {
            InitializeGame(true);
        }

        private async Task SetPlayerNamesAsync(string player1Name, string player2Name)
        {
            await InitializeCardsAsync();
            _player1.Name = player1Name;
            _player2.Name = player2Name;
            _isNameEntered = true;
        }

        private async Task InitializeCardsAsync()
        {
            Random random = new();
            int cardPairs = _cardCount / 2;
            var cardsWithCategories = Cards.Where((item) => string.IsNullOrEmpty(_cardCategory) || item.Category == _cardCategory);
            var validCardIds = new List<Guid>();
            var cardsToPlay = cardsWithCategories.Where(card => !card.Disabled).OrderBy(x => random.Next()).Take(cardPairs).Select((item)=> item.Clone()).ToList();
            var cardsIdToContent = cardsToPlay.Select((item) => new KeyValuePair<Guid, string?>(item.Id, item.Content)).ToList();

            foreach (var cardIdToContent  in cardsIdToContent)
            {
                if(cardIdToContent.Value == null)
                {
                    continue;
                }
                if (cardIdToContent.Value.StartsWith("<")){
                    validCardIds.Add(cardIdToContent.Key);
                }else if(!cardIdToContent.Value.Contains("/") && !cardIdToContent.Value.Contains("."))
                {
                    validCardIds.Add(cardIdToContent.Key);
                }
                else
                {
                    try
                    {
                        var result = await Client.GetByteArrayAsync($"images/memory_card/{cardIdToContent.Value}");
                        if(result?.Length  > 0)
                        {
                            validCardIds.Add(cardIdToContent.Key);
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            var validCardsToPlay = cardsToPlay.Where((item) => validCardIds.Contains(item.Id)).ToList();
            PlayCards = validCardsToPlay.Concat(validCardsToPlay.Select(item => item.Clone())).OrderBy(x => random.Next()).ToList();
        }
        private async Task OnCardClick(MemoryCard card)
        {
            if (isProcessing)
            {
                return;
            }
            isProcessing = true;
            void FlipAllAndReset()
            {
                if (_selectedCard != null)
                {
                    _selectedCard.IsFlipped = false;
                    _selectedCard = null;
                }
                if (_selectedSecondCard != null)
                {
                    _selectedSecondCard.IsFlipped = false;
                    _selectedSecondCard = null;
                }
            }
            if (_selectedCard != null && _selectedSecondCard != null)
            {
                FlipAllAndReset();
                StateHasChanged();
                await Task.Delay(200);
            }

            if (_selectedCard == null ^ _selectedSecondCard == null
                && (_selectedSecondCard == card || _selectedCard == card))
            {
                if(_selectedCard != null)
                {
                    _selectedCard.IsFlipped = true;
                }
                if(_selectedSecondCard != null)
                {
                    _selectedSecondCard.IsFlipped = true;
                }
                isProcessing = false;
                StateHasChanged();
                return;
            }
           
            if (_selectedCard == null)
            {
                _selectedCard = card;
                _selectedCard.IsFlipped = true;
                isProcessing = false;
                StateHasChanged();
                return;
            }
            _selectedSecondCard = card;
            _selectedSecondCard.IsFlipped = true;
            
            if (_selectedCard.Id == card.Id)
            {
                _currentPlayer!.Score++;
                await Task.Delay(2000);
                _selectedCard.IsCollected = true;
                _selectedCard.IsFlipped = false;
                card.IsCollected = true;
                card.IsFlipped = false;
                if (PlayCards?.Where(pc => !pc.IsCollected)?.Count() <= 2)
                {
                    _isGameOver = true;

                    // select the player with the most points
                    _currentPlayer = _player1!.Score > _player2!.Score ? _player1 : _player2;
                }
            }
            if (!_isGameOver)
            {
                _currentPlayer = _currentPlayer == _player1 ? _player2 : _player1;
            }
            isProcessing = false;

            StateHasChanged();
        }
    }
}
