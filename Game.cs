using System;

namespace BattleShips
{
    public class Game
    {
        private Board _aiBoard;
        private Board _playerBoard;
        private Board _aiShotsBoard;
        private Board _playerShotsBoard;
        private int[] _shipLengths;
        private bool _gameOver;
        
        private readonly Random _random = new Random();
        public Game(int[] shipLengths = null)
        {
            _shipLengths = shipLengths ?? new[] {5, 4, 3, 3, 2};
            _aiBoard = new Board(_random);
            _playerBoard = new Board(_random);
            
            _aiShotsBoard = new Board(_random);
            _playerShotsBoard = new Board(_random);
        }
        public void Start()
        {
            if (_gameOver)
            {
                Console.WriteLine("Programmer error: Game is already over");
                return;
            }
            foreach (var shipLength in _shipLengths)
            {
                _playerBoard.Render();
                var (coordinate, rotation) = _playerBoard.PickSpaceInteractive(
                    "Please enter a coordinate for your boat",
                    "Placing",
                    shipLength);

                _playerBoard.Place(coordinate, rotation, shipLength);
            }

            foreach (var shipLength in _shipLengths)
            {
                var (coordinate, rotation) = _aiBoard.PickSpaceRandom(shipLength);
                
                _aiBoard.Place(coordinate, rotation, shipLength);
                
                /*// For debugging only
                Console.WriteLine($"{coordinate} {rotation}");
                _aiBoard.Render();
                Console.Read();*/
            }

            Console.WriteLine("Thank you for picking your ships.\nIt's time to play!");

            var playerTurn = true;
            _gameOver = false;
            
            while (!_gameOver)
            {
                Tuple<int, int> coordinate;
                
                Board shotBoard;
                Board shipBoard;
                
                if (playerTurn)
                {
                    Console.Clear();
                    Console.WriteLine("\nIt's your turn! Here's your board:");
                    _playerBoard.Render();
                    Console.WriteLine("\nHere's the shots you've made:");
                    _playerShotsBoard.Render();
                    Console.WriteLine("Press a key to make a move...");
                    Console.Read();
                    (coordinate, _) = _playerShotsBoard.PickSpaceInteractive(
                        "Please enter a coordinate for your shot",
                        "Shooting");

                    shotBoard = _playerShotsBoard;
                    shipBoard = _aiBoard;
                }
                else
                {
                    (coordinate, _) = _aiShotsBoard.PickSpaceRandom();
                    
                    shotBoard = _aiShotsBoard;
                    shipBoard = _playerBoard;
                }

                if (shipBoard.Overlaps(coordinate))
                {
                    shipBoard.Place(coordinate, character: 'X');
                    shotBoard.Place(coordinate, character: 'H');
                } else {
                    shipBoard.Place(coordinate, character: '◦');
                    shotBoard.Place(coordinate, character: '◦');
                }

                if (shipBoard.HasNoShips())
                {
                    Console.Clear();
                    Console.WriteLine($"Game over..., the {(playerTurn ? "player" : "ai")} won!");
                    _gameOver = true;
                }

                playerTurn = !playerTurn;
            }
            
            Console.WriteLine("Here were the were the ships at the end of the game...:");
            Console.WriteLine("For the player...");
            _playerBoard.Render();
            Console.WriteLine("");
            Console.WriteLine("and for the AI...");
            _aiBoard.Render();
            
            Console.WriteLine("Thank you for playing, press enter to return to the menu...");
            Console.ReadLine();
        }
    }
}