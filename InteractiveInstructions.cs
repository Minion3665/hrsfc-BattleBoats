using System;

namespace BattleShips
{
    public class InteractiveInstructions
    {
        public void Show()
        {
            var shotsBoard = new Board();
            var shipsBoard = new Board();

            var aiShipsBoard = new Board(); // We'll be doing some sneaky stuff here to make the player always succeed

            Console.Clear();
            Console.WriteLine(
                "Welcome to Battle Ships! To play, you'll first need to place your ships on the board. For this game, we'll just be playing with a single 3-unit ship.");
            Console.WriteLine("Press any key to continue...");
            Console.Read();

            var (coordinate, rotation) =
                shipsBoard.PickSpaceInteractive("Enter the coordinates you want your ship to be at...", "Placing at",
                    3);
            shipsBoard.Place(coordinate, rotation, 3);

            Console.Clear();
            Console.WriteLine(
                "To win, you'll need to sink all of the enemy's ships. You won't be able to see where they are, but if you hit one of their ships, you'll be told that you've hit it.");
            Console.WriteLine("You'll need to hit every space on the ship to sink it.");
            Console.WriteLine("Press any key to continue...");
            Console.Read();

            (coordinate, rotation) =
                shotsBoard.PickSpaceInteractive("Guess a place to shoot your shot...", "Shooting at");
            shotsBoard.Place(coordinate, rotation, character: 'H');

            aiShipsBoard.Place(coordinate.Item1 <= 7 ? coordinate : new Tuple<int, int>(7, coordinate.Item2), false, 3);
            aiShipsBoard.Place(coordinate, rotation, character: 'X');

            Console.Clear();
            Console.WriteLine(
                "It's a hit!!!\nCongratulations! You've hit your first ship! Keep guessing until you sink the whole ship!");
            Console.WriteLine("Press any key to continue...");

            while (!aiShipsBoard.HasNoShips())
            {
                (coordinate, _) = shotsBoard.PickSpaceInteractive("Please enter a coordinate for your shot...", "Shooting");
                if (aiShipsBoard.Overlaps(coordinate, false)) {
                    aiShipsBoard.Place(coordinate, character: 'X');
                    shotsBoard.Place(coordinate, character: 'H');
                } else {
                    aiShipsBoard.Place(coordinate, character: '◦');
                    shotsBoard.Place(coordinate, character: '◦');
                }
            }
            Console.Clear();
            Console.WriteLine("And that's sunk... Well done! You've sunk the enemy ship and defended your waters! You win!");
            Console.WriteLine("Try playing a full game to see how you do!");
            Console.WriteLine("Press any key to return to the main menu...");
            Console.Read();
        }
    }
}