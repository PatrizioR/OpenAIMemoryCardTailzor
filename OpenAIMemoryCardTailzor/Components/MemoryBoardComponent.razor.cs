using Microsoft.AspNetCore.Components;
using OpenAIMemoryCardTailzor.Models;

namespace OpenAIMemoryCardTailzor.Components
{
    public partial class MemoryBoardComponent
    {
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
        protected override void OnInitialized()
        {
            base.OnInitialized();
            InitializeGame();
        }
        private void InitializeGame()
        {
            _player1 = new Player();
            _player2 = new Player();
            _currentPlayer = _player1;
            _isGameOver = false;
            _isNameEntered = false;
            _selectedCard = null;
            _selectedSecondCard = null;
        }
        private void RestartGame()
        {
            InitializeGame();
        }

        private void SetPlayerNames(string player1Name, string player2Name)
        {
            _player1.Name = player1Name;
            _player2.Name = player2Name;
            _isNameEntered = true;
            InitializeCards();
        }

        private void InitializeCards()
        {
            Random random = new();
            int cardPairs = _cardCount / 2;
            var cardsToPlay = Cards.Where((item)=> string.IsNullOrEmpty(_cardCategory) || item.Category == _cardCategory).OrderBy(x => random.Next()).Take(cardPairs).Select((item)=> item.Clone()).ToList();

            PlayCards = cardsToPlay.Concat(cardsToPlay.Select(item => item.Clone())).OrderBy(x => random.Next()).ToList();
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
                if (PlayCards?.Count <= 2)
                {
                    _isGameOver = true;
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
