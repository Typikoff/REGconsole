// Random Event Generator
// This app was created to automatize some aspects for our (myself and my sister) modification of Minecraft
// The modification contains zero lines of code, and based on our conciosness rules
// The rules are basicly turning Minecraft into a strategy game, Civilization-like game
// With the Project growing larger, it's getting more and more harder to maintain some random events, which occured in every state every turn
// The Events are based on goverment type, amount of population and some laws
// Paper card system cann't be normally used at this point: it takes around one hour just to start a  turn
// So I put on my bravary and dedication to create this ambitious (for me) project.

using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Reflection.Metadata.Ecma335;

namespace REG
{
    class Programm
    {
        static void Main(string[] args)
        {
            while (true)
            {

                string[] deckName = GetDeckName();
                if (deckName[0] == "quit")
                {
                    Environment.Exit(0);
                }

                switch (deckName.Length)
                {
                    case 1:
                        try
                        {
                            Deck deck1 = new Deck(deckName[0]);
                            deck1.GetRandomCard();
                        }
                        catch (DirectoryNotFoundException)
                        {
                            Console.WriteLine($"Was unable to find a Deck called \"{deckName[0]}\"");
                        }
                        break;
                    case 2:
                        try
                        {
                            int amnt = Convert.ToInt32(deckName[1]);
                            Deck deck2 = new Deck(deckName[0], amnt);
                            deck2.GetRandomCard(amnt);
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Second parameter must be an integer number that is greater than zero");
                        }
                        catch (DirectoryNotFoundException)
                        {
                            Console.WriteLine($"Was unable to find a Deck called \"{deckName[0]}\"");
                        }

                        break;
                    default:
                        {
                            Console.WriteLine("Please follow the correct sintax \"<DeckName> <Integer>\" or \"<DeckName>\"");
                            break;
                        }
                }
            }
        }

        static string[] GetDeckName()
        {
            Console.WriteLine("Please type deck name (and number of cards, if wanted):");
            string? name = Console.ReadLine();
            if (name == null) { throw new ArgumentNullException("name"); }

            if (name.Contains(' ')) // for multiple cards pick
            {
                try
                {
                    string[] parts = name.Split(' ');
                    return parts;
                }
                catch
                {
                    throw new Exception("Please follow the correct sintax \"<DeckName> <Integer>\"");
                }
            }
            else // for one-card pick
            {
                string[] part = new string[1];
                part[0] = name;
                return part;
            }
        }
    }

    class Deck : DeckInteractions
    {
        private int cardIndex;
        public Deck(string pth) // constructor for StateTypeDecks
        {
            Path = stateDirPath + pth;
            LastCardPickUpRaw = "";
            AmountOfCards = 1;
            Cards = Directory.GetFileSystemEntries(Path);
        }
        public Deck(string pth, int amnt) // constructor for PeoplePerRealmDecks
        {
            Path = pPRDirPath + pth;
            LastCardPickUpRaw = "";
            AmountOfCards = amnt;
            Cards = Directory.GetFileSystemEntries(Path);
        }

        public string Path
        { get; }
        public string[] Cards
        { get; }
        public int AmountOfCards
        { get; }
        public string LastCardPickUpRaw
        { get; set; }
        public override void GetRandomCard()
        {
            Random rnd = new Random();
            cardIndex = rnd.Next(Cards.Length);

            CardInteractions(cardIndex);
        }
        public override void GetRandomCard(int amount)
        {
            if (amount < 1 || amount >= Cards.Length)
            {
                Console.WriteLine("Please take numbers no less than one and less than the deck can maximum offer");
            }
            else
            {
                Random rnd = new Random();
                int[] indexes = new int[amount];

                while (amount != 0)
                {
                    cardIndex = rnd.Next(Cards.Length);
                    if (indexes.Contains(cardIndex)) // this turns the possibility of peeking one card twice to zero
                    {
                        continue;
                    }
                    indexes[--amount] = cardIndex;
                }

                foreach (int index in indexes)
                {
                    CardInteractions(index);
                }
            }
        }

        private void CardInteractions(int index)
        {
            string card = Cards[index];
            LastCardPickUpRaw = Cards[index];

            card = card.Split('-')[0]; // get rid of .txt extention and - Copy (number) postfix
            card = card.Split('\\')[filePathLength]; // get rid of file path
            Console.WriteLine(card);

            GetUserReaction(LastCardPickUpRaw);
        }

        private void GetUserReaction(string fullPath)
        {
            string? reaction = Console.ReadLine();
            switch (reaction)
            {
                case "quit":
                    Environment.Exit(0);
                    break;
                case "what":
                    // card description
                    Console.WriteLine(GetCardInfo(fullPath)[0]);
                    break;
                case "why":
                    // card arrival note
                    Console.WriteLine(GetCardInfo(fullPath)[1]);
                    break;
                case "what why":
                    // description + arrival note
                    Console.WriteLine(GetCardInfo(fullPath)[0]);
                    Console.WriteLine(GetCardInfo(fullPath)[1]);
                    break;
            }


        }

        private string[] GetCardInfo(string fullPath) // returns card's description and arrival note packed in a string array
        {
            string[] cardInfo = new string[2];
            if (File.Exists(fullPath))
            {
                try
                {
                    cardInfo = File.ReadAllLines(fullPath);
                }
                catch (ArgumentOutOfRangeException) // Arises when chosen file has  more ar less than two lines
                {
                    Console.WriteLine($"Flie called {fullPath} has invalid sintax!");
                    cardInfo[0] = File.ReadAllLines(fullPath)[0];
                    cardInfo[1] = File.ReadAllLines(fullPath)[0];
                }
            }
            else // basicly mustn't happen
            {
                cardInfo[0] = "Invalid description";
                cardInfo[1] = "No arrival note";
                throw new FileNotFoundException();
            }
            return cardInfo;
        }
    }

    abstract class DeckInteractions
    {
        public int filePathLength = 8;
        public string stateDirPath = @"C:\Users\Typik\REG\REG\Decks\StateTypeDecks\";
        public string pPRDirPath = @"C:\Users\Typik\REG\REG\Decks\PeoplePerRealmDecks\";

        public abstract void GetRandomCard();
        public abstract void GetRandomCard(int amount);
    }
}