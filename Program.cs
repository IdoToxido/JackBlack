using System.Formats.Asn1;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Transactions;
// Fix and/or complete SPLIT option
// search SPLIT RELATED 
class User()
{
    public bool HasHit = false;
    public bool HasDoubledDown = false;
    public bool lost = false;
    public bool HasSplit = false;
    public int bet = 0;
    public int Money = 100;
    public List<int> Deck = [];
    public List<int> SplitHandSums = [];
    public int TimesSplit = 0;
}
class PlayingCards()
{
    // UNTOUCHED
    public List<int> ShuffledDeck = [ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13,
                                      1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13,
                                      1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13,
                                      1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 ];
    // Current playing deck
    public List<int> Deck = [ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13,
                              1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13,
                              1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13,
                              1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 ];

    public void Shuffle()
    {
        ShuffledDeck = new(Deck);
    }
    public void DrawCard(List<int> Player)
    {
        Random rnd = new();
        while (true)
        {
            int Num = rnd.Next(0, Deck.Count);
            Player.Add(Deck[Num]);
            Deck.RemoveAt(Num);
            break;
        }
    }
    // Compact name cuz I want to
    // Draw Wanted Card
    public void DWC(List<int> Player, int card)
    {
        if (!Deck.Contains(card))
            DrawCard(Player);
        else
        {
            Player.Add(card);
            Deck.Remove(card);
        }
    }
}

class Program
{
    static int PlayerCount = 1;
    static List<User> AllPlayers = [];
    static List<int> Dealer = [];
    static PlayingCards PD = new();
    static int DealerMoney = 1000;
    static string input = "";
    public static void Main()
    {
        // CheckCountInput(); UNFREEZE
        for (int i = 0; i < PlayerCount; i++)
            AllPlayers.Add(new User());
        while (input != "end")
        {
            PD.Shuffle();
            Dealer = [];
            ResetPlayerDecks();
            // Dealer card draw
            for (int i = 0; i < 2; i++)
                PD.DrawCard(Dealer);
            List<string> StrDealer = List2Str(Dealer);
            // CheckBetInput(); UNFREEZE
            AllPlayers[0].bet = 10;
            //Playing sequence for all players
            for (int i = 0; i < AllPlayers.Count; i++)
            {
                // Players card draw
                // for (int j = 0; j < 2; j++) UNFREEZE
                // {
                //     PD.DrawCard(AllPlayers[i].Deck);
                // }
                PD.DWC(AllPlayers[i].Deck, 11); PD.DWC(AllPlayers[i].Deck, 11);
                // 
                List<string> StrPlayer = List2Str(AllPlayers[i].Deck);
                Print(StrDealer, StrPlayer, false, i);
                // Options
                while (true)
                {
                    if (HandSum(AllPlayers[i].Deck) == 21)
                    {
                        Console.WriteLine("BlackJack");
                        break;
                    }
                    Console.WriteLine("H - hit,  S - stand,  Y - split,  D - double down");
                    Console.WriteLine();
                    ConsoleKey Act = Console.ReadKey(true).Key;
                    // SPLIT RELATED
                    // if (Act == ConsoleKey.Y && !AllPlayers[i].HasHit)
                    // {
                    //     Split(AllPlayers[i].Deck[0], AllPlayers[i].Deck[1], i);
                    //     AllPlayers[i].HasSplit = true;
                    //     break;
                    // }
                    // ADD ELSE SPLIT RELATED
                    if (Act == ConsoleKey.H && !AllPlayers[i].HasDoubledDown)
                    {
                        if (LostHit(false, i))
                            break;
                    }
                    else if (Act == ConsoleKey.D && !AllPlayers[i].HasHit)
                    {
                        if (LostHit(false, i))
                            break;
                        AllPlayers[i].HasDoubledDown = true;
                    }
                    else if (Act == ConsoleKey.S)
                    {
                        Console.WriteLine();
                        break;
                    }
                    else
                        Console.WriteLine("Invalid Input! Please Try Again.");
                }
            }
            // checking sums if win or lose
            //
            while (HandSum(Dealer) <= 16)
                PD.DrawCard(Dealer);
            DdrawPwin();
            // SPLIT RELATED
            // SplitDdrawPwin();
        }
    }
    public static bool LostHit(bool IsInSplit, int i)
    {
        PD.DrawCard(AllPlayers[i].Deck);
        if (!IsInSplit)
            AllPlayers[i].HasHit = true;
        Print(List2Str(Dealer), List2Str(AllPlayers[i].Deck), false, i);
        if (HandSum(AllPlayers[i].Deck) >= 22)
        {
            AllPlayers[i].lost = true;
            Console.WriteLine($"Player {i + 1} busted! Better luck next time!");
        }
        return AllPlayers[i].lost;
    }
    public static List<string> List2Str(List<int> Cards)
    {
        List<string> rtn = [];
        for (int i = 0; i < Cards.Count; i++)
        {
            if (Cards[i] <= 10 && Cards[i] != 1)
                rtn.Add(Cards[i].ToString()!);
            else
            {
                switch (Cards[i].ToString())
                {
                    case "1":
                        rtn.Add("A");
                        break;
                    case "11":
                        rtn.Add("J");
                        break;
                    case "12":
                        rtn.Add("Q");
                        break;
                    case "13":
                        rtn.Add("K");
                        break;
                    default:
                        rtn.Add(Cards[i].ToString()!);
                        break;
                }
            }
        }
        return rtn;
    }
    public static void Print(List<string> DealerHand, List<string> PlayerHand, bool DealerContinue, int i)
    {
        for (int j = 0; j < 30; j++)
            Console.Write('_');
        for (int j = 0; j < 2; j++)
            Console.WriteLine();
        // start card print
        PrintCard(DealerHand, true, DealerContinue);
        for (int j = 0; j < 4; j++)
            Console.WriteLine();
        if (!DealerContinue)
            PrintCard(PlayerHand, false, true);
        // end card print
        for (int j = 0; j < 30; j++)
            Console.Write('_');
        if (!DealerContinue)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Player {i + 1}'s turn! ");
            Console.ResetColor();
        }
        for (int j = 0; j < 2; j++)
            Console.WriteLine();
    }
    // C - Card, D - Dealer's Card, Cont - Continue
    public static void PrintCard(List<string> Cards, bool IsDealer, bool Continue)
    {
        // sets dealer's cards to red and player's to blue
        if (IsDealer)
            Console.ForegroundColor = ConsoleColor.Red;
        else
            Console.ForegroundColor = ConsoleColor.Blue;
        // prints the first card if continue is false
        if (!Continue)
        {
            CardTop();
            Console.WriteLine();
            CardMid(Cards, 0);
            Console.WriteLine();
            CardBot();
        }
        //prints all cards if continue is true
        else
        {
            for (int i = 0; i < Cards.Count; i++)
            {
                CardTop();
                Console.Write(' ');
            }
            Console.WriteLine();
            for (int i = 0; i < Cards.Count; i++)
            {
                CardMid(Cards, i);
                Console.Write(' ');
            }
            Console.WriteLine();
            for (int i = 0; i < Cards.Count; i++)
            {
                CardBot();
                Console.Write(' ');
            }
            Console.WriteLine();
            Console.WriteLine();
        }
        Console.ResetColor();
    }
    public static void CardTop()
    {
        Console.Write('|');
        for (int i = 0; i < 3; i++)
            Console.Write('-');
        Console.Write('|');
    }
    public static void CardMid(List<string> Card, int i)
    {
        Console.Write('|');
        if (Card[i].Length == 2)
        {
            Console.Write(' ');
            Console.Write(Card[i]);
        }
        else
        {
            Console.Write(' ');
            Console.Write(Card[i]);
            Console.Write(' ');
        }
        Console.Write('|');
    }
    public static void CardBot()
    {
        Console.Write('|');
        for (int i = 0; i < 3; i++)
            Console.Write('-');
        Console.Write('|');
    }
    static int HandSum(List<int> cards)
    {
        int sum = 0;
        int aces = 0;
        foreach (int card in cards)
            if (card == 1)
            {
                sum += 11;
                aces++;
            }
            else if (card >= 10)
                sum += 10;
            else
                sum += card;
        while (sum > 21 && aces > 0)
        {
            sum -= 10;
            aces--;
        }
        return sum;
    }
    //  SPLIT RELATED
    // public static void Split(int i)
    // {
    //     AllPlayers[i].TimesSplit += 1;
    //     Console.ForegroundColor = ConsoleColor.Green;
    //     Console.WriteLine($"Cards split! Player {i + 1}'s new bet is: {AllPlayers[i].bet * 2}");
    //     Console.ForegroundColor = ConsoleColor.Red;
    //     Console.WriteLine($"Player {i + 1} press 'S' or bust in order to get to the next hand.");
    //     Console.ResetColor();
    //     AllPlayers[i].Deck = [AllPlayers[i].Deck[0]];
    //     PD.DrawCard(AllPlayers[i].Deck);
    //     PD.DrawCard(Ydeck2);
    //     SplitActions(i);
    //     SplitActions(i);
    // }
    // SPLIT RELATED
    // public static void SplitDdrawPwin()
    // {
    //     int DealerSum = HandSum(Dealer);
    //     for (int i = 0; i < PlayerCount; i++)
    //         if (AllPlayers[i].HasSplit)
    //             foreach (int SplitPlayerSum in AllPlayers[i].SplitHandSums)
    //                 if (SplitPlayerSum == DealerSum)
    //                 {
    //                     Console.WriteLine($"Player {i + 1} pushed!");
    //                     AllPlayers[i].Money += AllPlayers[i].bet;
    //                     DealerMoney -= AllPlayers[i].bet;
    //                 }
    //                 else if (DealerSum >= 22)
    //                 {
    //                     Console.WriteLine($"Dealer busted! Player {i + 1} Won!");
    //                     AllPlayers[i].Money += AllPlayers[i].bet * 2;
    //                     DealerMoney -= AllPlayers[i].bet * 2;
    //                 }
    //                 else if (SplitPlayerSum > DealerSum)
    //                 {
    //                     Console.WriteLine($"Player{i + 1} won!");
    //                     AllPlayers[i].Money += AllPlayers[i].bet * 2;
    //                     DealerMoney -= AllPlayers[i].bet * 2;
    //                 }
    //                 else if (SplitPlayerSum < DealerSum)
    //                     Console.WriteLine($"Player {i + 1} lost! Better luck next time!");
    // }
    public static void DdrawPwin()
    {
        Print(List2Str(Dealer), List2Str(AllPlayers[0].Deck), true, 0);
        for (int i = 0; i < PlayerCount; i++)
        {
            if (AllPlayers[i].HasSplit)
                continue;
            int Psum = HandSum(AllPlayers[i].Deck);
            int Dsum = HandSum(Dealer);
            if (Psum == 21)
            {
                DealerMoney -= (int)(AllPlayers[i].bet * 2.5);
                AllPlayers[i].Money += (int)(AllPlayers[i].bet * 2.5);
            }
            else if (Psum >= 22)
            {
                AllPlayers[i].lost = true;
                Console.WriteLine($"Player {i + 1} Busted! Better luck next time!");
                Console.WriteLine();
            }
            else if (Psum == Dsum && !AllPlayers[i].lost)
            {
                Console.WriteLine("Push!");
                AllPlayers[i].Money += AllPlayers[i].bet;
                DealerMoney -= AllPlayers[i].bet;
            }
            else if (Dsum >= 22 && !AllPlayers[i].lost)
            {
                Console.WriteLine($"Dealer busted! Player {i + 1} won!");
                AllPlayers[i].Money += AllPlayers[i].bet * 2;
                DealerMoney -= AllPlayers[i].bet * 2;
            }
            else if (Psum > Dsum && Psum <= 21)
            {
                Console.WriteLine($"Player {i + 1} won!");
                AllPlayers[i].Money += AllPlayers[i].bet * 2;
                DealerMoney -= AllPlayers[i].bet * 2;
            }
            else if (Psum < Dsum)
                Console.WriteLine($"Player {i + 1} lost! Better luck next time!");
        }
        PrintWinChart();
    }
    // SPLIT RELATED
    // public static void SplitActions(List<int> Ydeck, int HandNum, int i)
    // {
    //     Print(List2Str(Dealer), List2Str(AllPlayers[i].Deck), false, i);
    //     while (true)
    //     {
    //         Console.ForegroundColor = ConsoleColor.DarkMagenta;
    //         Console.WriteLine($"Hand {HandNum} ");
    //         ConsoleKey Act = Console.ReadKey(true).Key;
    //         Console.ForegroundColor = ConsoleColor.Red;
    //         Console.WriteLine("H - hit,  S - stand,  Y - split");
    //         Console.ResetColor();
    //         if (Act == ConsoleKey.Y && AllPlayers[i].Deck[0] == AllPlayers[i].Deck[1] && !AllPlayers[i].HasHit)
    //             Split(i);
    //         if (Act == ConsoleKey.H)
    //         {
    //             AllPlayers[i].HasHit = true;
    //             if (LostHit(true, i))
    //                 break;
    //         }
    //         else if (Act == ConsoleKey.S || HandSum(Ydeck) == 21)
    //         {
    //             AllPlayers[i].SplitHandSums.Add(HandSum(Ydeck));
    //             break;
    //         }
    //     }
    // }
    public static bool CheckNumber(string str)
    {
        foreach (char c in str)
            if (!char.IsDigit(c))
                return false;
        return true;
    }
    public static void CheckCountInput()
    {
        while (true)
        {
            Console.WriteLine("Enter number of players");
            input = Console.ReadLine()!;
            if (int.Parse(input) >= 1 && int.Parse(input) <= 4 && CheckNumber(input))
            {
                PlayerCount = int.Parse(input);
                break;
            }
            else
                Console.WriteLine("Invalid input! Try again.");
        }

    }
    public static void CheckBetInput()
    {
        for (int i = 0; i < PlayerCount; i++)
        {
            while (true) // Checking that the bet is alright
            {
                Console.WriteLine($"Dealer's Currency: {DealerMoney}$");
                Console.WriteLine($"Player {i + 1} Currency: {AllPlayers[i].Money}$, How much would player {i + 1} like to bet?");
                input = Console.ReadLine()!;
                if (CheckNumber(input))
                {
                    AllPlayers[i].bet = int.Parse(input);
                    if (AllPlayers[i].bet > AllPlayers[i].Money)
                    {
                        Console.WriteLine($"Insufficient funds for player number {i + 1}! Enter a new amount to bet.");
                        continue;
                    }
                    if (AllPlayers[i].bet > DealerMoney)
                        if (AllPlayers[i].bet > DealerMoney)
                        {
                            Console.WriteLine($"Dealer has insufficient funds! Player{i + 1}, Please enter a new amount to bet.");
                            continue;
                        }
                        else
                        {
                            DealerMoney += AllPlayers[i].bet;
                            AllPlayers[i].Money -= AllPlayers[i].bet;
                            DealerMoney += AllPlayers[i].bet;
                            AllPlayers[i].Money -= AllPlayers[i].bet;
                            break;
                        }
                }
                else
                    Console.WriteLine("Input Error! Please Try Again."); continue;
            }
        }
    }
    public static void PrintWinChart()
    {
        int DealerHandSum = HandSum(Dealer);
        for (int i = 0; i < 7; i++)
            Console.Write('_');
        Console.WriteLine();
        for (int i = 0; i < PlayerCount; i++)
        {
            int PlayerHandSum = HandSum(AllPlayers[i].Deck);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"{PlayerHandSum,-3}");
            Console.ResetColor();
            Console.Write("| ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"{DealerHandSum,-3}|");
            Console.ResetColor();
            Console.Write("| ");
            Console.Write("| ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Win: {!AllPlayers[i].lost}");
            Console.WriteLine();
            Console.Write($"Win: {!AllPlayers[i].lost}");
            Console.WriteLine();
            Console.ResetColor();
        }
        Console.WriteLine();
    }
    public static void ResetPlayerDecks()
    {
        for (int i = 0; i < PlayerCount; i++)
        {
            AllPlayers[i].Deck = [];
        }
    }
}