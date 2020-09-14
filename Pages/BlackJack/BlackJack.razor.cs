using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Blazor_CardsGame.Pages.BlackJack
{
    public partial class BlackJack : ComponentBase
    {
        private Cards[] cards;
        private readonly List<string> cardList = new List<string>();
        private readonly List<int> values = new List<int>();
        private readonly Random rng = new Random();
        private readonly List<string> PlayerCards = new List<string>();
        private readonly List<string> DealerCards = new List<string>();

        private int PlayerScore;
        private int DealerScore;
        private string message;
        private int DealerHit;
        private int playerHit;
        private bool hold = false;
        private int PlayerCardValue1 = 0;
        private int PlayerCardValue2 = 0;
        private int DealerCardValue1;
        private int DealerCardValue2;

        private bool PlayerCheckAces = true;
        private bool DealerCheckAces = true;

        protected override async Task OnInitializedAsync()
        {
            cards = await Http.GetFromJsonAsync<Cards[]>("sample-data/cards.json");

            foreach (var card in cards)
            {
                cardList.Add(card.Url);
                values.Add(card.Value);
            }
        }

        private void StartGame()
        {
            PlayerHit();
            DealerHand();
        }

        private void PlayerHit()
        {
            int index1 = rng.Next(0, cardList.Count() - 1);
            int index2 = rng.Next(0, cardList.Count() - 1);
            playerHit++;

            if (playerHit == 1)
            {
                OpenFirstTwoCards(index1, index2);
            }
            if (playerHit >= 2 && PlayerScore < 21)
            {
                OtherPlayerHits(index1);
            }
            if (PlayerScore > 21)
            {
                message = "YOU BUSTED!!";
            }
            if (PlayerScore == 21)
            {
                message = "BLACK JACK!!";
            }
        }

        private void OpenFirstTwoCards(int index1, int index2)
        {
            PlayerCardValue1 = values[index1];
            PlayerCardValue2 = values[index2];
            if (PlayerCardValue1 == 11 && PlayerCardValue2 == 11)
            {
                PlayerCardValue1 = 1;
                PlayerCardValue2 = 1;
            }

            PlayerCards.Add(cardList[index1]);
            PlayerCards.Add(cardList[index2]);

            cardList.RemoveAt(index1);
            cardList.RemoveAt(index2);

            values.RemoveAt(index1);
            values.RemoveAt(index2);

            PlayerScore = PlayerCardValue1 + PlayerCardValue2;
        }

        private void OtherPlayerHits(int index1)
        {
            int CardValue3 = values[index1];

            if (CardValue3 == 11 && PlayerScore > 10)
            {
                CardValue3 = 1;
            }

            PlayerCards.Add(cardList[index1]);

            cardList.RemoveAt(index1);

            values.RemoveAt(index1);

            PlayerScore += CardValue3;

            if (PlayerCheckAces == true)
            {
                if (PlayerCardValue1 == 11 || PlayerCardValue2 == 11)
                {
                    if (PlayerScore > 21)
                    {
                        PlayerScore -= 10;
                        PlayerCheckAces = false;
                    }
                }
            }
        }

        private void DealerHand()
        {
            DealerHit++;
            int index = rng.Next(0, cardList.Count() - 1);

            DealerCardValue1 = values[index];

            DealerCards.Add(cardList[index]);

            cardList.RemoveAt(index);

            values.RemoveAt(index);

            DealerScore = DealerCardValue1;
        }

        private void Hold()
        {
            DealerHit++;
            hold = true;
            while (DealerScore < 21 && DealerScore < PlayerScore)
            {
                if (DealerHit == 1)
                {
                    SecondDealerCard();
                }
                if (DealerHit == 2)
                {
                    OtherDealerCards();
                }
            }
        }

        private void SecondDealerCard()
        {
            int index = rng.Next(0, cardList.Count() - 1);

            DealerCardValue2 = values[index];

            if (DealerCardValue2 == 11 && DealerCardValue1 == 11)
            {
                DealerCardValue2 = 1;
                DealerCardValue1 = 1;
            }
            DealerCards.Add(cardList[index]);

            cardList.RemoveAt(index);

            values.RemoveAt(index);

            DealerScore = DealerCardValue1 + DealerCardValue2;

            DealerHit++;

            GameMessage();
        }

        private void OtherDealerCards()
        {
            int index = rng.Next(0, cardList.Count() - 1);

            int CardValue = values[index];

            DealerCards.Add(cardList[index]);

            cardList.RemoveAt(index);

            values.RemoveAt(index);

            DealerScore += CardValue;

            if (CardValue == 11 && DealerScore > 21)
            {
                DealerScore -= 10;
            }
            if (DealerCheckAces == true)
            {
                if (DealerCardValue1 == 11 || DealerCardValue2 == 11)
                {
                    if (DealerScore > 21)
                    {
                        DealerScore -= 10;
                        DealerCheckAces = false;
                    }
                }
            }
            GameMessage();
        }

        private void NewGame()
        {
            NavigationManager.NavigateTo("refresh");
        }

        private void GameMessage()
        {
            if (PlayerScore > DealerScore)
            {
                message = "YOU WON!!";
            }
            if (DealerScore == 21 || DealerScore > PlayerScore)
            {
                message = "Dealer WON!!";
            }
            if (DealerScore == PlayerScore)
            {
                message = "TIE!!";
            }
            if (DealerScore > 21)
            {
                message = "Dealer BUSTED!!";
            }
        }
    }
}