using System;

namespace MonopolySim
{
    class Bank
    {
        System.Collections.Generic.IDictionary<int, int> playerCash = new System.Collections.Generic.Dictionary<int, int>();
        System.Collections.Generic.IDictionary<int, int> propertyOwnership = new System.Collections.Generic.Dictionary<int, int>();
    }

    class TurnTracker
    {
        public void advanceTurnTracker()
        {
            currentPlayer = (currentPlayer + 1) % numberOfPlayers;
        }

        public int CurrentPlayer
        {
            get
            {
                return currentPlayer;
            }
        }

        public int NumberOfPlayers
        {
            get
            {
                return numberOfPlayers;
            }
            set
            {
                numberOfPlayers = value;
            }
        }

        int currentPlayer = 0;
        int numberOfPlayers = 4;
    }

    class Game
    {
        public Game(int players)
        {
            for(int i = 0; i < players; i++)
            {
                board.setPlayerSpace(i, 0);
            }
            turnTracker.NumberOfPlayers = players;
        }

        public void executeTurn()
        {
            int player = turnTracker.CurrentPlayer;
            int dieRoll = die.Next(1, 7) + die.Next(1, 7);
            board.advancePlayerSpace(player, dieRoll);
            board.getCurrentSpace(player).PlayerInteract(this, player);
            Console.WriteLine("Player {0} advanced {1} spaces and landed on {2}", player, dieRoll, board.getCurrentSpace(player).ToString());
            turnTracker.advanceTurnTracker();
        }

        public TurnTracker GameTurnTracker
        {
            get
            {
                return turnTracker;
            }
        }

        public Board GameBoard
        {
            get
            {
                return board;
            }
        }

        public Bank GameBank
        {
            get
            {
                return bank;
            }
        }

        TurnTracker turnTracker = new TurnTracker();
        Random die = new Random();
        Bank bank = new Bank();
        Board board = new Board();
    }

    class CommunityChestCard
    {

    }

    class ChanceCard
    {

    }

    class Board
    {
        public Board()
        {
            int spaceCounter = 0;
            spaces[spaceCounter++] = new Go();

            spaces[spaceCounter++] = new Property("Mediterranean Avenue");
            spaces[spaceCounter++] = new CommunityChest();
            spaces[spaceCounter++] = new Property("Baltic Avenue");
            spaces[spaceCounter++] = new Tax();

            spaces[spaceCounter++] = new Property("Reading Railroad");

            spaces[spaceCounter++] = new Property("Oriental Avenue");
            spaces[spaceCounter++] = new Chance();
            spaces[spaceCounter++] = new Property("Vermont Avenue");
            spaces[spaceCounter++] = new Property("Connecticut Avenue");

            spaces[spaceCounter++] = new Jail();

            spaces[spaceCounter++] = new Property("Saint Charles Place");
            spaces[spaceCounter++] = new Property("Electric Company");
            spaces[spaceCounter++] = new Property("States Avenue");
            spaces[spaceCounter++] = new Property("Virginia Avenue");

            spaces[spaceCounter++] = new Property("Pennsylvania Railroad");

            spaces[spaceCounter++] = new Property("Saint James Place");
            spaces[spaceCounter++] = new CommunityChest();
            spaces[spaceCounter++] = new Property("Tennessee");
            spaces[spaceCounter++] = new Property("New York Avenue");

            spaces[spaceCounter++] = new FreeParking();

            spaces[spaceCounter++] = new Property("Kentucky Avenue");
            spaces[spaceCounter++] = new CommunityChest();
            spaces[spaceCounter++] = new Property("Indiana Avenue");
            spaces[spaceCounter++] = new Property("Illinois Avenue");

            spaces[spaceCounter++] = new Property("B. & O. Railroad");

            spaces[spaceCounter++] = new Property("Atlantic Avenue");
            spaces[spaceCounter++] = new Property("Ventnor Avenue");
            spaces[spaceCounter++] = new Property("Water Works");
            spaces[spaceCounter++] = new Property("Marvin Gardens");

            spaces[spaceCounter++] = new GoToJail(10);

            spaces[spaceCounter++] = new Property("Pacific Avenue");
            spaces[spaceCounter++] = new Property("North Carolina Avenue");
            spaces[spaceCounter++] = new CommunityChest();
            spaces[spaceCounter++] = new Property("Pennsylvania Avenue");

            spaces[spaceCounter++] = new Property("Short Line");

            spaces[spaceCounter++] = new Chance();
            spaces[spaceCounter++] = new Property("Park Place");
            spaces[spaceCounter++] = new Tax();
            spaces[spaceCounter++] = new Property("Boardwalk");
        }

        public Space getCurrentSpace(int player)
        {
            return spaces[playerSpaceLocations[player]];
        }

        public void advancePlayerSpace(int player, int numberOfSpaces)
        {
            playerSpaceLocations[player] = (playerSpaceLocations[player] + numberOfSpaces) % spaces.Length;
        }

        public void setPlayerSpace(int player, int spaceIndex)
        {
            playerSpaceLocations[player] = spaceIndex;
        }

        public Space getSpace(int index)
        {
            return spaces[index];
        }

        public int getPlayerSpaceIndex(int player)
        {
            return playerSpaceLocations[player];
        }

        public override string ToString()
        {
            System.Text.StringBuilder returnString = new System.Text.StringBuilder();
            int index = 1;
            foreach(Space s in spaces)
            {
                returnString
                    .Append(index++)
                    .Append(": ")
                    .Append(s.ToString())
                    .Append("\n");
            }
            return returnString.ToString();
        }

        public void PrintBoardStatistics()
        {
            foreach (Space s in spaces)
            {
                Console.Write("{0}:\n{1}",s.ToString(),s.GetPlayerStatistics());
            }
        }

        Space[] spaces = new Space[40];
        System.Collections.Generic.IDictionary<int, int> playerSpaceLocations = new System.Collections.Generic.Dictionary<int,int>();
    }

    class Space
    {
        public virtual void PlayerInteract(Game context, int playerIndex)
        {
            int value = 0;
            if(playerSpaceLandings.TryGetValue(playerIndex,out value))
            {
                ++playerSpaceLandings[playerIndex];
            }
            else
            {
                playerSpaceLandings[playerIndex] = 0;
            }
        }

        public override string ToString()
        {
            return "Space";
        }

        public string GetPlayerStatistics()
        {
            System.Text.StringBuilder statisticsString = new System.Text.StringBuilder();
            foreach(int playerIndex in playerSpaceLandings.Keys)
            {
                statisticsString.Append("Player: ")
                                .Append(playerIndex)
                                .Append(" landed here ")
                                .Append(playerSpaceLandings[playerIndex])
                                .Append(" times\n");
            }
            return statisticsString.ToString();
        }

        System.Collections.Generic.IDictionary<int, int> playerSpaceLandings = new System.Collections.Generic.Dictionary<int, int>();
    }

    class Jail : Space
    {
        public override string ToString()
        {
            return "Jail";
        }
    }

    class GoToJail : Space
    {
        public GoToJail(int jailIndex)
        {
            this.jailIndex = jailIndex;
        }

        public override void PlayerInteract(Game context, int playerIndex)
        {
            int currentPlayerSpace = context.GameBoard.getPlayerSpaceIndex(playerIndex);
            context.GameBoard.setPlayerSpace(playerIndex, currentPlayerSpace);
        }

        public override string ToString()
        {
            return "Go To Jail";
        }

        int jailIndex = 0;
    }

    class Property : Space
    {
        public Property(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return name;
        }

        string name;
    }

    class Tax : Space
    {
        public override string ToString()
        {
            return "Tax";
        }
    }

    class FreeParking : Space
    {
        public override string ToString()
        {
            return "Free Parking";
        }
    }

    class Go : Space
    {
        public override string ToString()
        {
            return "Go";
        }
    }

    class Chance : Space
    {
        public override string ToString()
        {
            return "Chance";
        }
    }

    class CommunityChest : Space
    {
        public override string ToString()
        {
            return "Community Chest";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Game g = new Game(4);
            Board b = new Board();
            Console.WriteLine(b.ToString());
            for(UInt32 i = 0; i < 4000; i++)
            {
                g.executeTurn();
            }
            g.GameBoard.PrintBoardStatistics();
            Console.ReadLine();
        }
    }
}