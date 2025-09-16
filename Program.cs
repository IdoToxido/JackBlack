using System.Security.Cryptography.X509Certificates;

class PlayingCards()
{
    public int?[,] ShuffledDeck = { { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 },
                                    { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 },
                                    { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 },
                                    { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 } };
    public int?[,] Deck = { { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 },
                            { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 },
                            { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 },
                            { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 } };

    public void Shuffle()
    {
        for (int i = 0; i < ShuffledDeck.GetLength(0); i++)
        {
            for (int j = 0; j < ShuffledDeck.GetLength(1); j++)
            {
                Deck[i, j] = ShuffledDeck[i, j];
            }
        }
    }
    public void DrawCard(List<int?> Player)
    {
        Random rnd = new();
        while (true)
        {
            int Suit = rnd.Next(0, 4);
            int Num = rnd.Next(0, 13);
            if (Deck[Suit, Num] != null)
            {
                Player.Add(Deck[Suit, Num]);
                Deck[Suit, Num] = null;
                break;
            }
        }
    }
}

class Program
{
    static bool hit = false;
    static bool DD = false;
    static bool lost = false;
    static List<int?> Dealer = [];
    static List<int?> Player = [];
    static PlayingCards PD = new();

    public static void Main()
    {
        string input = "";
        int bet = 0;
        int Money = 100;
        int DealerMoney = 1000;
        while (input != "end")
        {
            PD.Shuffle();
            bet = 0;
            while (true) //Checking that the bet is alright
            {
                Console.WriteLine($"Dealer's Currency: {DealerMoney}$");
                Console.WriteLine($"Currency: {Money}$, How much would you like to bet?");
                input = Console.ReadLine()!;
                if (input == "end")
                    break;
                if (CheckInput(input))
                {
                    bet = int.Parse(input);
                    if (bet > Money)
                    {
                        Console.WriteLine("Insufficient Funds! Enter a new amount to bet."); continue;
                    }
                    if (bet * 2.5 > DealerMoney)
                    {
                        Console.WriteLine("Dealer Has Insufficient Funds! Enter a new amount to bet."); continue;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                    Console.WriteLine("Input Error! Please Try Again."); continue;
            }
            Money -= bet;
            DealerMoney += bet;

            // Card Shuffle

            for (int i = 0; i < 2; i++)
            {
                PD.DrawCard(Player);
                PD.DrawCard(Dealer);
            }
            List<string> StrPlayer = [];
            StrPlayer = List2Str(Player);
            List<string> StrDealer = List2Str(Dealer);
            Print(StrDealer, StrPlayer, false);

            // Options
            while (true)
            {
                Console.WriteLine("H - hit,  S - stand,  Y - split,  D - double down");
                Console.WriteLine();
                if (HandSum(Player) == 21)
                {
                    if (21 == HandSum(Dealer))
                    {
                        Money += bet;
                    }
                    Money += (int)(bet * 2.5);
                    break;
                }
                ConsoleKey Act = Console.ReadKey(true).Key;
                if (Act == ConsoleKey.Y && !hit)
                {

                }
                if (Act == ConsoleKey.H && !DD)
                {
                    if (LostHit())
                        break;
                }
                else if (Act == ConsoleKey.D && !hit)
                {
                    if (LostHit())
                        break;
                    DD = true;
                }
                else if (Act == ConsoleKey.S)
                {
                    Console.WriteLine();
                    break;
                }
                else
                    Console.WriteLine("Invalid Input! Please Try Again.");
            }

            // checking sums if win or lose
            while (HandSum(Dealer) <= 16 && !lost)
            {
                PD.DrawCard(Dealer);

                Print(List2Str(Dealer), List2Str(Player), true);

                if (HandSum(Player) == HandSum(Dealer) && HandSum(Dealer) >= 17)
                {
                    Console.WriteLine("Push!");
                    Money += bet;
                    DealerMoney -= bet;
                    break;
                }
                else if (HandSum(Dealer) >= 22)
                {
                    Console.WriteLine("Dealer Busted! You Won!");
                    Money += bet * 2;
                    DealerMoney -= bet * 2;
                    break;
                }
                else if (HandSum(Player) == 21 && HandSum(Dealer) >= 17)
                {
                    Console.WriteLine("BlackJack!");
                    Money += bet * (6 / 2);
                    DealerMoney -= bet * (6 / 2);
                    break;
                }
                else if (HandSum(Player) > HandSum(Dealer) && HandSum(Dealer) >= 17)
                {
                    Console.WriteLine("You Won!");
                    Money += bet * 2;
                    DealerMoney -= bet * 2;
                    break;
                }
                else if (HandSum(Player) < HandSum(Dealer))
                {
                    Console.WriteLine("You lost! Better luck next time!");
                }
            }
        }
    }
    public static bool LostHit()
    {
        PD.DrawCard(Player);
        hit = true;
        Print(List2Str(Dealer), List2Str(Player), false);
        if (HandSum(Player) >= 22)
        {
            lost = true;
            Console.WriteLine("You Busted! Better luck next time!");
        }
        return lost;
    }

    public static bool CheckInput(string input)
    {
        foreach (char c in input)
        {
            if (!char.IsDigit(c))
                return false;
        }
        return true;
    }
    public static List<string> List2Str(List<int?> Cards)
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
    public static void Print(List<string> DHand, List<string> PHand, bool Dcont)
    {
        for (int j = 0; j < 120; j++)
        {
            Console.Write('_');
        }
        for (int i = 0; i < 2; i++)
            Console.WriteLine();
        // start card print
        PrintCard(DHand, true, Dcont);
        for (int i = 0; i < 4; i++)
            Console.WriteLine();
        PrintCard(PHand, false, true);
        // end card print
        for (int j = 0; j < 120; j++)
        {
            Console.Write('_');
        }
        for (int i = 0; i < 2; i++)
            Console.WriteLine();
    }
    // C - Card, D - Dealer's Card, Cont - Continue
    public static void PrintCard(List<string> C, bool D, bool Cont)
    {
        // sets dealer's cards to red and player's to blue
        if (D)
            Console.ForegroundColor = ConsoleColor.Red;
        else
            Console.ForegroundColor = ConsoleColor.Blue;
        // print card
        if (!Cont)
        {
            CardTop();
            Console.WriteLine();
            CardMid(C, 0);
            Console.WriteLine();
            CardBot();
        }
        else
        {
            for (int i = 0; i < C.Count; i++)
            {
                CardTop();
                Console.Write(' ');
            }
            Console.WriteLine();
            for (int i = 0; i < C.Count; i++)
            {
                CardMid(C, i);
                Console.Write(' ');
            }
            Console.WriteLine();
            for (int i = 0; i < C.Count; i++)
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
        {
            Console.Write('-');
        }
        Console.Write('|');

    }
    public static void CardMid(List<string> C, int i)
    {
        Console.Write('|');
        if (C[i].Length == 2)
        {
            Console.Write(' ');
            Console.Write(C[i]);
        }
        else
        {
            Console.Write(' ');
            Console.Write(C[i]);
            Console.Write(' ');
        }
        Console.Write('|');
    }
    public static void CardBot()
    {
        Console.Write('|');
        for (int i = 0; i < 3; i++)
        {
            Console.Write('-');
        }
        Console.Write('|');
    }
    public static int? HandSum(List<int?> Hand)
    {
        int? rtn = 0;
        int aceCount = 0;
        for (int i = 0; i < Hand.Count; i++)
        {

            if (Hand[i] == 1)
            {
                rtn += 11;
                aceCount++;
            }
            if (Hand[i] != 1)
            {
                if (Hand[i] >= 10)
                {
                    rtn += 10;
                }
                else
                    rtn += Hand[i];
            }
        }
        while (aceCount >= 1 && rtn >= 22)
        {
            rtn -= 10;
            aceCount--;
        }
        return rtn;
    }
    public static int?[] CopyArr(int?[] Arr)
    {
        int?[] rtn = [];
        for (int i = 0; i < Arr.Length; i++)
        {
            if (rtn[i] != null)
                rtn = [.. rtn, Arr[i]];
        }
        return rtn;
    }
    public static bool Split()
    {

    }
}