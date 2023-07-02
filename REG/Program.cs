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
using System.Reflection;

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
                    case "Cataclysm ": // TODO finish it
                        string[] listOfCataclisms = { "Earthquake", "Meteorite", "Drougth", "Flood", "Pollution", "Wildfire", "Avalance" }; // what could happen
                        string[] clockWiseDirections9 = { "North", "North-East", "East", "South-East", "South", "South-West", "West", "North-West", "Center" }; // possible location of dissaster
                        Console.WriteLine("Your state suffered a natural disaster. If it impossible, type something (some disasters are region-specific).");
                        while (userReaction != "")
                        {
                            string chosenEvent = listOfCataclisms[rnd.Next(listOfCataclisms.Length)]; // for random disaster
                            switch (chosenEvent) // for specific reaction to each
                            {
                                case "Earthquake": //      |||            selects random part of a state               |||
                                    Console.WriteLine($"The {clockWiseDirections9[rnd.Next(clockWiseDirections9.Length)]} part of your country was struck with an Earthquake.");
                                    break;
                                case "Meteorite":
                                    Console.WriteLine($"The {clockWiseDirections9[rnd.Next(clockWiseDirections9.Length)]} part of your country was hit by a Meteorite.");
                                    break;
                                case "Drougth":
                                    Console.WriteLine($"An extreme Drougth fell upon your country. The {clockWiseDirections9[rnd.Next(clockWiseDirections9.Length)]} part suffered the most.");
                                    break;
                                case "Flood": // no need for specific location, flood is whereas river is
                                    Console.WriteLine("Due to heavy rains, water levels in nearby river have rised a lot. Unexpected Flood swept the shores.");
                                    break;
                                case "Pollution":
                                    Console.WriteLine($"Lack of nature-conserving measures with execive resource exploitation resulted in a heavy Pollution in the {clockWiseDirections9[rnd.Next(clockWiseDirections9.Length)]} part of your country.");
                                    break;
                                case "Wildfire":
                                    Console.WriteLine($"Hot climate with a lack of rains provoked a Wildfire in the {clockWiseDirections9[rnd.Next(clockWiseDirections9.Length)]} part of your country.");
                                    break;
                                case "Avalance": // no need for specific location, it happens just with most snowwy mountain
                                    Console.WriteLine("As snow melted in your mountains, fast and enourmous Avalance fell upon.");
                                    break;

                            }

                            userReaction = Console.ReadLine();
                        }
                        break;
                    case "Corruption ":
                        Console.WriteLine("Please type down an amount of current state sitizens.");
                        try
                        {
                            AmountOfPeople = Convert.ToInt32( Console.ReadLine() );
                            Console.WriteLine($"A person number {rnd.Next(AmountOfPeople)+1} was seen in some corruption sqeemes.");
                        }
                        catch (FormatException)
                        { Console.WriteLine("It is quite impossible to have none..."); }
                        userReaction = "";
                        break;
                    case "HSR ": // High Society Request or people demanding rights!
                        string[] demands = { "Social polices", "Cultural sphere", "Working conditions", "Living conditions", "State-specific uniqenesses", "all branches: Social, Cultural, Working and Living conditions, and some state-specific uniqe things" };
                        Console.WriteLine($"With society growth, requests are also rizing. Your people are demanding an improovement of {demands[rnd.Next(demands.Length)]}.");
                        userReaction = "";
                        break;
                    case "Labor Migrants ": //          5-10 migrants
                        Console.WriteLine($"A group of {rnd.Next(5, 11)} Labor Migrants came to your country for one year.");
                        userReaction = "";
                        break;
                    case "Suicide ":
                        Console.WriteLine("Please type a number of those with Happiness below 30%.");
                        try
                        {
                            int userInput = Convert.ToInt32( Console.ReadLine() );
                            if (userInput == 0) userInput = 1/userInput; // roundabout way to go to catch block, if user types '0'
                            Console.WriteLine($"Among them, a person standing {rnd.Next(userInput)+1} commits it.");
                        }
                        catch
                        {
                            Console.WriteLine($"Better off joking. One should commit it. Fate is predetermined. The person number {rnd.Next(AmountOfPeople)+1} did it.");
                        }
                        userReaction = "";
                        break;
                    // state-spesific events
                    case "Crown's Patronage ":
                        switch (rnd.Next(3))
                        {
                            case 0: 
                                Console.WriteLine("Some unnamed talent presented you with a masterpiece: An unforgettably exprescive Poem.");
                                break;
                            case 1:
                                Console.WriteLine("Some unnamed talent presented you with a masterpiece: An indifferent from reality Painting.");
                                break;
                            case 2:
                                Console.WriteLine("Some unnamed talent presented you with a masterpiece: An incredibly harmonizing Melody.");
                                break;
                        }
                        userReaction = "";
                        break;
                    case "Unification ":
                        int randomAmount2 = 7; // standart amount for new clan
                        Console.WriteLine("A new Clan had settled in your state. Be ready to register newcomes.");
                        while (randomAmount2 != 0) // loop for each person
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
                            randomAmount2 -= 1;
                        }
                        userReaction = "";
                        break;
                    case "Coins Minting ":
                        Console.WriteLine("Please type down an amount of current state sitizens.");
                        try
                        {
                            AmountOfPeople = Convert.ToInt32(Console.ReadLine());
                            if (AmountOfPeople > 0)
                            {
                                Console.WriteLine($"You got {AmountOfPeople} golden ingots, {AmountOfPeople * 2} silver ingots and {AmountOfPeople * 5} copper ingots."); // thing below makes it easyer to get them in Minecraft
                                Console.WriteLine($"In convenient format: {AmountOfPeople / 64} stacks and {AmountOfPeople % 64} golden ingots, {AmountOfPeople / 32} stacks and {AmountOfPeople * 2 % 64} silver ingots, {AmountOfPeople * 5 / 64} stacks and {AmountOfPeople * 5 % 64} copper ingots.");
                            }                                                                                                               // same as Amount * 2 / 64         but here it can change result
                            else { Console.WriteLine("Please stop joking about your followers. They will get angry."); }
                        }                                                                                                               
                        catch (FormatException)
                        { Console.WriteLine("Invalid amount of people. Try to type normal integers."); }
                        userReaction = "";

                        break;
                    case "Escapers ":
                        Console.WriteLine("Some prizoners tryed to escape! Decide, wheather or not it was successfull, based on security quality and conditions of detention.\nIf it was unsuccessfull, press Enter, if not - type an amount of prizoners.");
                        userReaction = Console.ReadLine();
                        if (userReaction != "") // empty string stands for no success
                        {
                            try
                            {
                                int numberOfPrizoners = Convert.ToInt32(userReaction);
                                if (numberOfPrizoners <= 0) // anty-trollong def
                                {
                                    Console.WriteLine("Looks like this Punishment colony does not serve it's purpose. Greatly ashamed, first 5 stuff members deserted.");
                                }
                                else if (numberOfPrizoners <= 5) // 5 is exect amount of escapers
                                {
                                    Console.WriteLine("All prizoners escaped!");
                                }
                                else // normal situation
                                {
                                    int randomIndexOfPrixoner, indexer = 5;
                                    int[] indexesOfPrizoners = new int[indexer];
                                    while (indexer > 0) // generation of random 5 numbers
                                    {
                                        randomIndexOfPrixoner = rnd.Next(numberOfPrizoners) + 1;
                                        if (!(indexesOfPrizoners.Contains(randomIndexOfPrixoner)))
                                        {
                                            indexesOfPrizoners[indexer - 1] = randomIndexOfPrixoner;
                                            indexer--;
                                        }
                                    }
                                    Array.Sort(indexesOfPrizoners);    // for order in array
                                    Array.Reverse(indexesOfPrizoners); // it is easyer to delete elements from the end, because numeration will not change
                                    Console.Write($"Kriminals with numbers");
                                    foreach (int index in indexesOfPrizoners)
                                    {
                                        Console.Write(" " + Convert.ToString(index));
                                    }
                                    Console.WriteLine(" had escaped.");
                                }
                            }
                            catch (FormatException)
                            {
                                Console.WriteLine("You are saying rather intresting things. Then first 5 prizoners escaped.");
                            }
                        }
                        else
                        {
                            userReaction = "";
                        }
                        break;
                    case "Murder! ":
                        Console.WriteLine("A deadbody was found. In order to identify the victim, type an amount of people.");
                        try
                        {
                            AmountOfPeople = Convert.ToInt32(Console.ReadLine());
                            if (AmountOfPeople < 1)
                            {   // small joke, just to enlight QA
                                Console.WriteLine("Well, everybody is just dead by now. What shall we do? We need to find a murder or commit one. But there is only one person here - You, the user. So, my proposal is like: I'll generate an event, 'User's death', You will somehow die from this, and I, REG, will be the culprit. Sounds wonderfull, isn't it?");
                            }
                            else
                            {
                                Console.WriteLine($"After some investigation, the deadbody was identifyed. It was person with number {rnd.Next(AmountOfPeople)+1}. Now execute the prizoner, who did it.");
                            }
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Please use integers for amounts. It's like ... a general rule? Anyway, dead person's number was 1. Now execute the prizoner, who did it.");
                        }
                        userReaction = "";
                        break;
                    case "Refillment of Cameras ":
                        Console.WriteLine("A new batch of kriminals arrived, in total amount of 25. Be ready to register them.");
                        int kriminalsToCreate = 25;
                        while (kriminalsToCreate > 0)
                        {
                            int randomIndex1 = rnd.Next(6); // for person's education and fighting capacity 0-2 nothing, 3 only education, 4 only fighting capacity, 5 for both
                            switch (randomIndex1) // each person's characteristics, redused max value for kriminals
                            {                     // this strange formula for age produces average 30-35 result. Hovever, people with age of 20 or 50 are also possible.
                                case 3: // only education
                                    int educationIndex = rnd.Next(6); // for deciding optimal chances: 1 is common (0-3), 2 is rare (4-5)
                                    if (educationIndex >= 4) educationIndex = 2;
                                    else educationIndex = 1;
                                    Console.WriteLine($"Age: {rnd.Next(10, 25) + rnd.Next(10, 25)} ||| Gender: {(rnd.Next(2) == 0 ? "Male" : "Female")} ||| Education: {educationIndex} ||| Fichting capacity: 0.");
                                    break;
                                case 4: // only figthing cap
                                    int fightingCapacity = rnd.Next(6); // for deciding optimal chances: 1 is common (0-3), 2 is rare (4-5)
                                    if (fightingCapacity >= 4) fightingCapacity = 2;
                                    else fightingCapacity = 1;
                                    Console.WriteLine($"Age: {rnd.Next(10, 25) + rnd.Next(10, 25)} ||| Gender: {(rnd.Next(2) == 0 ? "Male" : "Female")} ||| Education: 0 ||| Fichting capacity: {fightingCapacity}.");
                                    break;
                                case 5: //both
                                    int educationIndex1 = rnd.Next(6); // for deciding optimal chances: 1 is common (0-3), 2 is rare (4-5)
                                    if (educationIndex1 >= 4) educationIndex1 = 2;
                                    else educationIndex1 = 1;

                                    int fightingCapacity1 = rnd.Next(6); // for deciding optimal chances: 1 is common (0-3), 2 is rare (4-5)
                                    if (fightingCapacity1 >= 4) fightingCapacity1 = 2;
                                    else fightingCapacity1 = 1;

                                    Console.WriteLine($"Age: {rnd.Next(10, 25) + rnd.Next(10, 25)} ||| Gender: {(rnd.Next(2) == 0 ? "Male" : "Female")} ||| Education: {educationIndex1} ||| Fichting capacity: {fightingCapacity1}.");
                                    break;
                                default: // none
                                    Console.WriteLine($"Age: {rnd.Next(10, 25) + rnd.Next(10, 25)} ||| Gender: {(rnd.Next(2) == 0 ? "Male" : "Female")} ||| Education: 0 ||| Fichting capacity: 0.");
                                    break;
                            }
                            Console.ReadLine();
                            kriminalsToCreate--;
                        }
                        userReaction = "";
                        break;
                    case "Call of the Horizon ":
                        int numberOfColonists = 5; // constant value for this card
                        Console.WriteLine("A group of 5 people of your states dominant nationality and religy separated to establish another settlement (or colony).");
                        Console.WriteLine("Do not forget to register them, or they woun't count as your citizens.");
                        while (numberOfColonists != 0) // loop for each person
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
                            numberOfColonists -= 1;
                        }
                        userReaction = "";
                        break;
                    case "Theft ":
                        Console.WriteLine("One of your state's chests was found empty! Please, type down a total number of possible chests, and you will know which one was.");
                        try
                        {
                            int theNumber = Convert.ToInt32(Console.ReadLine());
                            if (theNumber <= 0) // it is impossible to have 0 chests, because every state has it's "main" chest. At this point user is just moking REG.
                            {
                                Console.WriteLine("You really hate normal numbers, don't you?");
                            }
                            else
                            {
                                Console.WriteLine($"A cheest with number {rnd.Next(theNumber) + 1} was emptied.");
                            }
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("So you think, that messing around is funny, huh. Than, take this: ALL of state's chests are now empty. Think twice next time.");
                        }
                        userReaction = "";
                        break;
                    case "Herecy! ":
                        Console.WriteLine("An unholy heretic was cought during his rituals. Type down an amount of people in your state, so we can identify all of theyer followers.");
                        try
                        {
                            AmountOfPeople = Convert.ToInt32(Console.ReadLine());
                            if (AmountOfPeople < 1)
                            {
                                Console.WriteLine("Ah, I see, no people, huh. So it was a conversation between mossy stones with a bald one.");
                            }
                            else
                            {
                                Console.WriteLine($"Thankfully, the herecy hadn't spread jet. Only the person with number {rnd.Next(AmountOfPeople) + 1} must be subjected to execution, and predominance of the true faith will be restored.");
                            }
                        }
                        catch (FormatException)
                        {                                                                                   // to stop user from finding bugs and crashes.
                            Console.WriteLine("Please use integers for amounts. It's like ... a general rule? But anyway, the first five persons were cought up in herecy nets. Nothing, but immidiete fire purification, can save their souls.");
                        }
                        userReaction = "";
                        break;
                    case "Religious Festival ":
                        Console.WriteLine("An annual festival had started. Each of your state's active Religious centers recieves donations from devoted people. Let them know, how many there are such places.");
                        try
                        {
                            int theNumber = Convert.ToInt32(Console.ReadLine());
                            if (theNumber <= 0) // just in case
                            {
                                Console.WriteLine("The people were angry. They organised demonstrations and strikes. It all had ended only then, when ruling party promised to build at least one.");
                            }
                            else // 16 gold ingots and 64 bread per temple
                            {
                                Console.WriteLine($"The total amount of donation was {theNumber/4} stacks {theNumber%4 * 16} gold ingots and {theNumber} stacks of bread.");
                            }
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("As a result of such pour announcment, nobody donated even a single thing.");
                        }
                        userReaction = "";
                        break;
                    case "Fresh Batch ":
                        Console.WriteLine("A new batch of slaves arrived, in total amount of 15. Take a note on them.");
                        int slavesToCreate = 15;
                        while (slavesToCreate > 0)
                        {
                            int randomIndex1 = rnd.Next(6); // for person's education and fighting capacity 0-2 nothing, 3 only education, 4 only fighting capacity, 5 for both
                            switch (randomIndex1) // each person's characteristics, redused max value for kriminals, even more redused for slaves
                            {                     // this strange formula for age produces average 30-35 result. Hovever, people with age of 20 or 50 are also possible.
                                case 3: // only education
                                    Console.WriteLine($"Age: {rnd.Next(10, 25) + rnd.Next(10, 25)} ||| Gender: {(rnd.Next(2) == 0 ? "Male" : "Female")} ||| Education: 1 ||| Fichting capacity: 0.");
                                    break;
                                case 4: // only figthing cap
                                    Console.WriteLine($"Age: {rnd.Next(10, 25) + rnd.Next(10, 25)} ||| Gender: {(rnd.Next(2) == 0 ? "Male" : "Female")} ||| Education: 0 ||| Fichting capacity: 1.");
                                    break;
                                case 5: //both
                                    Console.WriteLine($"Age: {rnd.Next(10, 25) + rnd.Next(10, 25)} ||| Gender: {(rnd.Next(2) == 0 ? "Male" : "Female")} ||| Education: 1 ||| Fichting capacity: 1.");
                                    break;
                                default: // none
                                    Console.WriteLine($"Age: {rnd.Next(10, 25) + rnd.Next(10, 25)} ||| Gender: {(rnd.Next(2) == 0 ? "Male" : "Female")} ||| Education: 0 ||| Fichting capacity: 0.");
                                    break;
                            }
                            Console.ReadLine();
                            slavesToCreate--;
                        }
                        userReaction = "";
                        break;
                    case "Mysterious Revenge ":
                        Console.WriteLine("Deadbody was found. Please type down total number of non-slave people in your state.");
                        try
                        {
                            AmountOfPeople = Convert.ToInt32(Console.ReadLine());
                            if (AmountOfPeople < 1)
                            {
                                Console.WriteLine("Everybody is a slave! What an awesome idea. So everybody just rebel and scatter across nearby countries. Yours is nonexictant now.");
                            }
                            else
                            {
                                Console.WriteLine($"After some investigation, it turns out, that the {rnd.Next(AmountOfPeople) + 1}th non-slave was killed. Hovewer, the perpetraitor wasn't found.");
                            }
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("All your state's leading representatives were found dead, not just that one.");
                        }
                        userReaction = "";
                        break;
                    case "Property Lease ": // TODO finish it
                        Console.WriteLine("It is time to take your money for property lease! Plese type down an amount of leaseable objects.");
                        bool incorrectInput = true;

                        while (incorrectInput)
                        {
                            try
                            {
                                incorrectInput = false; // Correct format: 4 34 4 5 1
                                Console.WriteLine("The input format is: number of 1) boats 2) donkeys and mulls 3) Horses and small ships 4) Battle ships 5) Trade ships. Use ' ' (space) as a separator.");
                                userReaction = Console.ReadLine();
                                if (userReaction != "" && userReaction != null)
                                {
                                    string[] complexUserInput = userReaction.Split(" ");
                                    int[] KoefficientsForProperty = {1, 2, 3, 5, 9};
                                    int thePrice = 0;
                                    for (int i = 0; i < KoefficientsForProperty.Length; i++)
                                    {
                                        thePrice += Convert.ToInt32(complexUserInput[i]) * KoefficientsForProperty[i];
                                    }
                                    Console.WriteLine($"Total amount of value in gold: {thePrice/64} stacks and {thePrice%64} ingots.");
                                }
                                else
                                {
                                    Console.WriteLine("No property - no money.");
                                }
                            }
                            catch (Exception)
                            {
                                incorrectInput = true;
                                Console.WriteLine("Well, this imput format is definitly hard to understand, but I belive in you! Keep trying!");
                            }
                        }
                        userReaction = "";
                        break;
                    case "Long Live the Dynasty! ":
                        Console.WriteLine($"A new {(rnd.Next(2) == 1 ? "male" : "female")} member of the current dynasty was born!");
                        Console.WriteLine("Due to that fact, there was a great festival and nobody had worked. Also food consumption for this turn is doubled.");
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
        public static string startPath = AppDomain.CurrentDomain.BaseDirectory.Remove(AppDomain.CurrentDomain.BaseDirectory.IndexOf("bin")); // gets path with username (to work on other devices) and cuts unnececary parts of it.
        public string stateDirPath = @"Decks\StateTypeDecks\".Insert(0, startPath); // to ensure usage on other devices
        public string pPRDirPath = @"Decks\PeoplePerRealmDecks\".Insert(0, startPath);

        public abstract void GetRandomCard();
        public abstract void GetRandomCard(int amount);
    }
}