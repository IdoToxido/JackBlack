using System.Diagnostics.Contracts;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

// CHANGE ALL ARRAYS TO LISTS
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
    public static void Main()
    {
        PlayingCards PD = new();
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
            List<int?> Dealer = [];
            List<int?> Player = [];
            for (int i = 0; i < 2; i++)
            {
                PD.DrawCard(Player);
                PD.DrawCard(Dealer);
            }
            List<string> StrPlayer = [];
            List2Str(Player);
            List<string> StrDealer = Arr2Str(Dealer);
            Print(StrDealer, StrPlayer, false);

            // Options
            bool hit = false;
            bool DD = false;
            List<int?> PlayerHand2 = [];
            List<int?> PlayerHand3 = [];
            List<int?> PlayerHand4 = [];
            List<int?> TempArr = [];
            List<int?> TempArr2 = [];
            bool lost = false;
            while (true)
            {
                Console.WriteLine("H - hit,  S - stand,  Y - split,  D - double down");
                Console.WriteLine();
                if (HandSum(Player) == 21)
                    break;

                ConsoleKey Act2 = Console.ReadKey(true).Key;
                if (Act2 == ConsoleKey.H && !DD)
                {
                    Player = PD.DrawCard(Player);
                    hit = true;
                    Print(Arr2Str(Dealer), Arr2Str(Player), false);
                    if (HandSum(Player) >= 22)
                    {
                        lost = true;
                        Console.WriteLine("You lost! Better luck next time!");
                        break;
                    }
                }
                else if (Act2 == ConsoleKey.D && !hit)
                {
                    Player = PD.DrawCard(Player);
                    hit = true;
                    DD = true;
                    if (HandSum(Player) >= 22)
                    {
                        lost = true;
                        Console.WriteLine("You Busted! Better luck next time!");
                        break;
                    }
                }
                else
                    Console.WriteLine("Invalid Input! Please Try Again.");

                if ((Act2 == ConsoleKey.D && !hit) || Act2 == ConsoleKey.S)
                {
                    Console.WriteLine();
                    break;
                }
            }

        
            while (HandSum(Dealer) <= 16 && !lost)
            {
                Dealer = PD.DrawCard(Dealer);

                Print(Arr2Str(Dealer), Arr2Str(Player), true);

                if (HandSum(Player) == HandSum(Dealer) && HandSum(Dealer) >= 17)
                {
                    Console.WriteLine("Push!");
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
                }
            }
        }
        return rtn;
    }
    public static void Print(string[] DHand, string[] PHand, bool Dcont)
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
    public static void PrintCard(string[] C, bool D, bool Cont)
    {
        if (D)
            Console.ForegroundColor = ConsoleColor.Red;
        else
            Console.ForegroundColor = ConsoleColor.Blue;

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
            for (int i = 0; i < C.Length; i++)
            {
                CardTop();
                Console.Write(' ');
            }
            Console.WriteLine();
            for (int i = 0; i < C.Length; i++)
            {
                CardMid(C, i);
                Console.Write(' ');
            }
            Console.WriteLine();
            for (int i = 0; i < C.Length; i++)
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
    public static void CardMid(string[] C, int i)
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
    public static int? HandSum(int?[] PHand)
    {
        int? rtn = 0;
        int aceCount = 0;
        for (int i = 0; i < PHand.Length; i++)
        {
            Console.WriteLine("CurrentSum " + rtn);
            if (PHand[i] == 1)
            {
                rtn += 11;
                aceCount++;
            }
            if (PHand[i] != 1)
                rtn += PHand[i];
        }
        if (aceCount == 1 && rtn >= 22)
        {
            Console.WriteLine(rtn);
            rtn -= 10;
            Console.WriteLine(rtn);
            aceCount--;
        }
        if (aceCount == 2 && rtn >= 22)
        {
            rtn -= 20;
            aceCount -= 2;
        }
        if (aceCount == 3 && rtn >= 22)
        {
            rtn -= 30;
            aceCount -= 3;
        }
        if (aceCount == 4 && rtn >= 22)
        {
            rtn -= 40;
            aceCount -= 4;
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
}