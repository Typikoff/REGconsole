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
            Console.WriteLine("Greetings! I, REG the Consoleman, am ready to help your brain computer \nto handle some unpredictable events that will happen, while simulating civilizations. \nAwaiting for your orders!\n");

            while (true)
            {
                string[] deckName = GetDeckName();
                if (deckName[0] == "quit")
                {
                    Console.WriteLine("Thanks for Your interactions!");
                    Environment.Exit(0);
                }

                switch (deckName.Length)
                {
                    case 1: // one-card pick
                        try
                        {
                            Deck deck1 = new Deck(deckName[0]);
                            Console.WriteLine(); // just a separator
                            deck1.GetRandomCard();
                        }
                        catch (DirectoryNotFoundException)
                        {
                            Console.WriteLine($"Was unable to find a Deck called \"{deckName[0]}\"");
                        }
                        break;
                    case 2: // multiple card pick
                        try
                        {
                            int amnt = Convert.ToInt32(deckName[1]);
                            Deck deck2 = new Deck(deckName[0], ConvertPeopleToCards(amnt));
                            Console.WriteLine(); // just a separator
                            deck2.AmountOfPeople = amnt; // for one-card pick it will be specified later
                            deck2.GetRandomCard(ConvertPeopleToCards(amnt));
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
                            Console.WriteLine("Please follow the correct sintax \"<DeckName> <Integer>\" or \"<DeckName>\"");
                            break;
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

        static int ConvertPeopleToCards(int amount)
        {
            decimal amountDecimal = Convert.ToDecimal(amount);
            amountDecimal /= 10;
            int result = Convert.ToInt32(Math.Round(amountDecimal, 0, MidpointRounding.AwayFromZero));
            if (result == 0)
            {
                return 1;
            }
            return result;
        }
    }

    class Deck : DeckInteractions
    {
        private int cardIndex;
        public Deck(string pth) // constructor for StateTypeDecks
        {
            Path = stateDirPath + pth;
            LastCardPickUpRaw = "";
            Card = "";
            AmountOfCards = 1;  // fot one-card pick
            AmountOfPeople = 0; // will be specified by user specificly, just later
            Cards = Directory.GetFileSystemEntries(Path);
        }
        public Deck(string pth, int amnt) // constructor for PeoplePerRealmDecks
        {
            Path = pPRDirPath + pth;
            LastCardPickUpRaw = "";
            Card = "";
            AmountOfCards = amnt;
            AmountOfPeople = 0; // will be changed rigth after constructor
            Cards = Directory.GetFileSystemEntries(Path);
        }

        public string Path // fullpath
        { get; }
        public string[] Cards
        { get; }
        public string Card
        { get; set; }
        public int AmountOfCards
        { get; }
        public string LastCardPickUpRaw
        { get; set; }
        public int AmountOfPeople
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
                Console.WriteLine("Please take numbers no less than zero and less than the deck can maximum offer multiplied by ten.");
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

        private void CardInteractions(int index) // clears card name from it's path, powers GetUserReaction() and DoExpandedCardsActions()
        {
            try
            {
                Card = Cards[index];
                LastCardPickUpRaw = Cards[index];

                Card = Card.Split('-')[0]; // get rid of .txt extention and - Copy (number) postfix
                Card = Card.Split('\\')[filePathLength]; // get rid of file path
                Console.WriteLine(Card);

                DoExpandedCardsActions(Card);
                GetUserReaction(LastCardPickUpRaw);
            }
            catch (Exception) 
            { 
                Console.WriteLine("Invalid input. Try something else.");
            }
        }

        private void DoExpandedCardsActions(string name) // fully dependant on current cards, lacks reusability
        {
            Random rnd = new Random();
            string? userReaction = "foo"; // random value, not empty string ""
            bool showEndMessage = true;   // for speeding process with cards without Expanded action

            do
            {
                switch (name) // after the actual name of card must be one space, because of the way it clears name from path
                {
                    case "Accident ": // strange bug, does not pick an Accident card. Perhaps, we live in a safe society
                        Console.WriteLine($"Accident happend with a person number {rnd.Next(AmountOfPeople) + 1}");
                        Console.WriteLine("If this is possible, press Enter");
                        break;
                    case "Child ": 
                        Console.WriteLine(rnd.Next(2) == 0 ? "A boy was born" : "A girl was born");
                        Console.WriteLine("If you like to choose this child's parents, write down an amount of families, capable of having children.");
                        userReaction = Console.ReadLine();
                        if (userReaction != "")
                        {
                            try
                            {
                                int numOfCouples = Convert.ToInt32(userReaction);
                                if (numOfCouples == 0) // just trolling-proof
                                {
                                    Console.WriteLine($"Since there is no families, the child was born from a person number {rnd.Next(AmountOfPeople) + 1}.");
                                }
                                else if (numOfCouples > 0 && numOfCouples <= (AmountOfPeople / 2 - 2)) // main case
                                {
                                    int theRandomNumber = rnd.Next(AmountOfPeople) + 1;
                                    if (theRandomNumber < numOfCouples * 4)
                                    {
                                        Console.WriteLine($"The child was born in the family number {rnd.Next(numOfCouples) + 1}.");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"The child was born from a person number {theRandomNumber}.");
                                        Console.WriteLine("If this is impossible, use 'super dice' command after Expanded actions.");
                                    }
                                    userReaction = "";
                                }
                                else
                                {
                                    Console.WriteLine("Please type next time a possible amount of families.");
                                    userReaction = "";
                                }
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("You got great sence of humor!");
                                userReaction = "";
                            }
                        }
                        break;
                    case "Twins ": // the following monster-string generates sexes for two babies. Just a shortcut.
                        Console.WriteLine(rnd.Next(2) == 0 ? $"A boy and a{(rnd.Next(2) == 0 ? "nother boy" : " girl")} were born" : $"A girl and a{(rnd.Next(2) == 0 ? "nother girl" : " boy")} were born");
                        Console.WriteLine("If you like to choose their parents, write down an amount of families, capable of having children.");
                        userReaction = Console.ReadLine();
                        if (userReaction != "")
                        {
                            try
                            {
                                int numOfCouples = Convert.ToInt32(userReaction);
                                if (numOfCouples == 0) // just trolling-proof
                                {
                                    Console.WriteLine($"Since there is no families, the child was born from a person number {rnd.Next(AmountOfPeople) + 1}.");
                                }
                                else if (numOfCouples > 0 && numOfCouples <= (AmountOfPeople / 2 - 2))
                                {
                                    int theRandomNumber = rnd.Next(AmountOfPeople) + 1; // main case
                                    if (theRandomNumber < numOfCouples * 4)
                                    {
                                        Console.WriteLine($"They were was born in the family number {rnd.Next(numOfCouples) + 1}.");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"They were born from a person number {theRandomNumber}.");
                                        Console.WriteLine("If this is impossible, use 'super dice' command after Expanded actions.");
                                    }
                                    userReaction = "";
                                }
                                else
                                {
                                    Console.WriteLine("Please type next time a possible amount of families.");
                                    userReaction = "";
                                }
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("You got great sence of humor!");
                                userReaction = "";
                            }
                        }
                        break;
                    case "Divorce ":
                        if (AmountOfPeople == 1) // precaution for trolls
                        {
                            Console.WriteLine("Looks like the last one is single. No need to betray this heavy populated state. :(");
                            userReaction = "";
                            break;
                        }

                        Console.WriteLine("Please type an amount of existing families");
                        userReaction = Console.ReadLine();
                        if (userReaction != "")
                        {
                            try
                            {
                                int numOfCouples = Convert.ToInt32(userReaction);
                                if (numOfCouples == 0)
                                {
                                    Console.WriteLine("Than there is no need in divorce.");
                                }
                                else if (numOfCouples > 0 && numOfCouples <= (AmountOfPeople / 2 - 2))
                                {
                                    Console.WriteLine($"The fammily number {rnd.Next(numOfCouples) + 1} had dissolved.");
                                }
                                else
                                {
                                    Console.WriteLine("Please type next time a possible amount of families.");
                                }
                                userReaction = "";
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("You got great sence of humor!");
                                userReaction = "";
                            }
                        }
                        break;
                    case "Genius ":
                        Console.WriteLine($"A person number {rnd.Next(AmountOfPeople) + 1} become a genius!");
                        Console.WriteLine("If this is impossible, press Enter. Otherwise select 5 most effortfull branches for your state in resent years, and ordder them.");
                        userReaction = Console.ReadLine();

                        if (userReaction != "")
                        {
                            int randomInt = rnd.Next(6) + 1;
                            Console.WriteLine(randomInt == 6 ? "It is a genious Agent" : $"It is a genious type {randomInt}");
                            userReaction = "";
                        }
                        break;
                    case "Ilness ":
                        Console.WriteLine($"A person number {rnd.Next(AmountOfPeople) + 1} fell ill.");
                        int theCase = rnd.Next(6) + 1;
                        switch (theCase)
                        {
                            case 4: // traditional medicine
                                Console.WriteLine("That person's life is in danger, but traditional medicine, such as herbs, will save it.");
                                switch (rnd.Next(3)) // to dicide, how rare need herbs be.
                                {
                                    case 0:
                                        Console.WriteLine("Ordinary herbset will do.");
                                        break;
                                    case 1:
                                        Console.WriteLine("Required some rare ingredients.");
                                        break;
                                    case 2:
                                        Console.WriteLine("Only the rarest herbs will do.");
                                        break;
                                }
                                break;
                            case 5: // potions
                                Console.WriteLine("That person's life is in danger, but curing potion of some kind will save it.");
                                Console.WriteLine(rnd.Next(2) == 1 ? "Ordinary potion is enougth." : "The situation is getting worse. Adwanced potion required.");
                                break;
                            case 6: // uncureable
                                Console.WriteLine("After many tryes and methods, said person died. Looks like, that was an uncureable ilness.");
                                break;
                            default: // illness was not that strong, said person just lost it's turn.
                                Console.WriteLine("But luckily that person got throuth it with some rest.");
                                break;
                        }
                        break;
                    case "Kriminal ":
                        Console.WriteLine("One of your people became a kriminal. Carefully procseed to find out some details.");
                        Console.ReadLine(); // kriminals may be unknown to the person in charge of a state. This allow said person to not to accidently see it.
                        userReaction = "";
                        string[] KriminalTypes = {"Murderer", "Thief", "Vandal", "Traitor", "Kidnapper", "Terrorist"};
                        Console.WriteLine($"A person number {rnd.Next(AmountOfPeople)+1} became a {KriminalTypes[rnd.Next(KriminalTypes.Length)]}. Said person's capability of it is {rnd.Next(6)+1}.");
                        break;
                    case "Marriage ":
                        if (AmountOfPeople == 1) // precaution for trolls
                        {
                            Console.WriteLine("Somehow, it is impossible to marry itself :(");
                            userReaction = "";
                            break;
                        }

                        while (userReaction != "")
                        {
                            int randomIndex1 = rnd.Next(AmountOfPeople) + 1;
                            int randomIndex2 = rnd.Next(AmountOfPeople) + 1;
                            while (randomIndex1 == randomIndex2) // for excluding a possibility of picking the same person
                            {
                                randomIndex2 = rnd.Next(AmountOfPeople) + 1;
                            }
                            Console.WriteLine($"Person number {randomIndex1} married person number {randomIndex2}. If this is unlikely to happen, type something exept empty message.");
                            userReaction = Console.ReadLine(); // since chances of picking couples with same gender, kids, elderly or married ones are rather high, it is up to user to determine that happening
                        }
                        break;
                    case "Relatives ":
                        int randomAmount = rnd.Next(6)+1; // in one card 1-6 people
                        Console.WriteLine($"A group with a total number of {randomAmount} people of your states dominant nationality and religy arrived.");
                        Console.WriteLine("Be ready to properly register them.");
                        while (randomAmount != 0) // loop for each person
                        {
                            int randomIndex1 = rnd.Next(6); // for person's education and fighting capacity 0-2 nothing, 3 only education, 4 only fighting capacity, 5 for both
                            switch (randomIndex1) // each person's characteristics
                            {                     // this strange formula for age produces average 30-35 result. Hovever, people with age of 20 or 50 are also possible.
                                case 3: // only education
                                    int educationIndex = rnd.Next(6); // for deciding optimal chances: 1 is common (0-2), 2 is rare (3-4), 3 is the rarets (5)
                                    if (educationIndex == 5) educationIndex = 3;
                                    else if (educationIndex > 2) educationIndex = 2;
                                    else educationIndex = 1;
                                    Console.WriteLine($"Age: {rnd.Next(10, 25) + rnd.Next(10, 25)} ||| Gender: {(rnd.Next(2) == 0 ? "Male" : "Female")} ||| Education: {educationIndex} ||| Fichting capacity: 0.");
                                    break;
                                case 4: // only figthing cap
                                    int fightingCapacity = rnd.Next(6); // for deciding optimal chances: 1 is common (0-2), 2 is rare (3-4), 3 is the rarets (5)
                                    if (fightingCapacity == 5) fightingCapacity = 3;
                                    else if (fightingCapacity > 2) fightingCapacity = 2;
                                    else fightingCapacity = 1;
                                    Console.WriteLine($"Age: {rnd.Next(10, 25) + rnd.Next(10, 25)} ||| Gender: {(rnd.Next(2) == 0 ? "Male" : "Female")} ||| Education: 0 ||| Fichting capacity: {fightingCapacity}.");
                                    break; 
                                case 5: //both
                                    int educationIndex1 = rnd.Next(6); // for deciding optimal chances: 1 is common (0-2), 2 is rare (3-4), 3 is the rarets (5)
                                    if (educationIndex1 == 5) educationIndex1 = 3;
                                    else if (educationIndex1 > 2) educationIndex1 = 2;
                                    else educationIndex1 = 1;

                                    int fightingCapacity1 = rnd.Next(6); // for deciding optimal chances: 1 is common (0-2), 2 is rare (3-4), 3 is the rarets (5)
                                    if (fightingCapacity1 == 5) fightingCapacity1 = 3;
                                    else if (fightingCapacity1 > 2) fightingCapacity1 = 2;
                                    else fightingCapacity1 = 1;

                                    Console.WriteLine($"Age: {rnd.Next(10, 25) + rnd.Next(10, 25)} ||| Gender: {(rnd.Next(2) == 0 ? "Male" : "Female")} ||| Education: {educationIndex1} ||| Fichting capacity: {fightingCapacity1}.");
                                    break;
                                default: // none
                                    Console.WriteLine($"Age: {rnd.Next(10, 25) + rnd.Next(10, 25)} ||| Gender: {(rnd.Next(2) == 0 ? "Male" : "Female")} ||| Education: 0 ||| Fichting capacity: 0.");
                                    break;
                            }
                            Console.ReadLine();
                            randomAmount -= 1;
                        }
                        userReaction = "";
                        break;
                    case "Migrants ":
                        int randomAmount1 = rnd.Next(6) + 1; // in one card 1-6 people
                        Console.WriteLine($"A group with a total number of {randomAmount1} people of random nationality and religy arrived.");
                        Console.WriteLine("In case you will accept them, be ready to properly register them.");
                        while (randomAmount1 != 0) // loop for each person
                        {
                            int randomIndex1 = rnd.Next(6); // for person's education and fighting capacity 0-2 nothing, 3 only education, 4 only fighting capacity, 5 for both
                            switch (randomIndex1) // each person's characteristics
                            {                     // this strange formula for age produces average 30-35 result. Hovever, people with age of 20 or 50 are also possible.
                                case 3: // only education
                                    int educationIndex = rnd.Next(6); // for deciding optimal chances: 1 is common (0-2), 2 is rare (3-4), 3 is the rarets (5)
                                    if (educationIndex == 5) educationIndex = 3;
                                    else if (educationIndex > 2) educationIndex = 2;
                                    else educationIndex = 1;
                                    Console.WriteLine($"Age: {rnd.Next(10, 25) + rnd.Next(10, 25)} ||| Gender: {(rnd.Next(2) == 0 ? "Male" : "Female")} ||| Education: {educationIndex} ||| Fichting capacity: 0.");
                                    break;
                                case 4: // only figthing cap
                                    int fightingCapacity = rnd.Next(6); // for deciding optimal chances: 1 is common (0-2), 2 is rare (3-4), 3 is the rarets (5)
                                    if (fightingCapacity == 5) fightingCapacity = 3;
                                    else if (fightingCapacity > 2) fightingCapacity = 2;
                                    else fightingCapacity = 1;
                                    Console.WriteLine($"Age: {rnd.Next(10, 25) + rnd.Next(10, 25)} ||| Gender: {(rnd.Next(2) == 0 ? "Male" : "Female")} ||| Education: 0 ||| Fichting capacity: {fightingCapacity}.");
                                    break;
                                case 5: //both
                                    int educationIndex1 = rnd.Next(6); // for deciding optimal chances: 1 is common (0-2), 2 is rare (3-4), 3 is the rarets (5)
                                    if (educationIndex1 == 5) educationIndex1 = 3;
                                    else if (educationIndex1 > 2) educationIndex1 = 2;
                                    else educationIndex1 = 1;

                                    int fightingCapacity1 = rnd.Next(6); // for deciding optimal chances: 1 is common (0-2), 2 is rare (3-4), 3 is the rarets (5)
                                    if (fightingCapacity1 == 5) fightingCapacity1 = 3;
                                    else if (fightingCapacity1 > 2) fightingCapacity1 = 2;
                                    else fightingCapacity1 = 1;

                                    Console.WriteLine($"Age: {rnd.Next(10, 25) + rnd.Next(10, 25)} ||| Gender: {(rnd.Next(2) == 0 ? "Male" : "Female")} ||| Education: {educationIndex1} ||| Fichting capacity: {fightingCapacity1}.");
                                    break;
                                default: // none
                                    Console.WriteLine($"Age: {rnd.Next(10, 25) + rnd.Next(10, 25)} ||| Gender: {(rnd.Next(2) == 0 ? "Male" : "Female")} ||| Education: 0 ||| Fichting capacity: 0.");
                                    break;
                            }
                            Console.ReadLine();
                            randomAmount1 -= 1;
                        }
                        userReaction = "";
                        break;
                    case "Resources ":
                        Console.WriteLine("Through some trade and internal taxing, you managed to save up on some resources, common to your state's area.");
                        Console.WriteLine("You are free to decide on resource type you want to get.");
                        break;
                    case "Treasure ":
                        string[] listOfTreasures = {"Money", "Rare metals", "Secret knowlage", "Shiny gems", "Sad emptyness", "Poverfull tools", "Art masterpiece", "Wargear" };
                        Console.WriteLine("Your people accidently found a buried treasure. You hold your breath donw, anticipating the contents.");
                        Console.ReadLine(); // small dose of intrigue
                        // these two additional action are for a possibility of same categories being selected
                        string firstContent = listOfTreasures[rnd.Next(listOfTreasures.Length)];
                        string secondContent = listOfTreasures[rnd.Next(listOfTreasures.Length)];
                        Console.WriteLine(firstContent != secondContent ? $"The chest contained {firstContent}. But after carefully looking, you found a hidden part, which contained {secondContent}" : $"You got your hands on a really large amount of {firstContent}");
                        userReaction = "";
                        break;
                    default:
                        userReaction = "";
                        showEndMessage = false; // for speeding process with cards without Expanded action
                        break;
                }
                if (userReaction != "")
                {
                    userReaction = Console.ReadLine();
                }
            }
            while (userReaction != "");

            if (showEndMessage) // for speeding process with cards without Expanded action
            {
                Console.WriteLine("*** Expanded action ended ***");
            }
        }

        private void GetUserReaction(string fullPath)
        {
            string? reaction = Console.ReadLine();
            Random rnd = new Random();
            while (reaction != "")
            {
                switch (reaction)
                {
                    case "quit":
                        Console.WriteLine("Thanks for Your interactions!");
                        Environment.Exit(0);
                        break;
                    case "what": // card description
                        Console.WriteLine("Card description:");
                        Console.WriteLine(GetCardInfo(fullPath)[0]);
                        break;
                    case "why": // card arrival note
                        Console.WriteLine("Card arrival note:");
                        Console.WriteLine(GetCardInfo(fullPath)[1]);
                        break;
                    case "what why": // description + arrival note
                        Console.WriteLine("Card description:");
                        Console.WriteLine(GetCardInfo(fullPath)[0]);
                        Console.WriteLine("Card arrival note:");
                        Console.WriteLine(GetCardInfo(fullPath)[1]);
                        break;
                    case "coin":
                        Console.WriteLine(rnd.Next(2) == 1 ? "You got Heads" : "You got Tails");
                        break;
                    case "dice":
                        Console.WriteLine($"You roll {rnd.Next(6) + 1}");
                        break;
                    case "super dice": // allows User to set right limit to random
                        Console.WriteLine("Please set a number of sides");
                        try
                        {
                            int sides = Convert.ToInt32(Console.ReadLine());
                            Console.WriteLine($"You roll {rnd.Next(sides) + 1}");
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Please use an integer number that is more than 1.");
                        }
                        break;
                }
                reaction = Console.ReadLine();
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