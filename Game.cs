using System;

namespace BattleShips
{
    public class Game
    {
        private int[] _shipLengths;
        private bool _gameOver;
        
        private SaveData _saveData;
        
        private readonly Random _random = new Random();
        public Game(int[] shipLengths = null, SaveData save = null)
        {
            _shipLengths = shipLengths ?? new[] {5, 4, 3, 3, 2};
            _saveData = save;
        }
        public void Start()
        {
            if (_gameOver)
            {
                Console.WriteLine("Programmer error: Game is already over");
                return;
            }
            
            if (_saveData == null)
            {

                var playerBoard = new Board(_random);
                var aiBoard = new Board(_random);
                var playerShotsBoard = new Board(_random);
                var aiShotsBoard = new Board(_random);
                
                foreach (var shipLength in _shipLengths)
                {
                    playerBoard.Render();
                    var (coordinate, rotation) = playerBoard.PickSpaceInteractive(
                        "Please enter a coordinate for your boat",
                        "Placing",
                        shipLength);

                    playerBoard.Place(coordinate, rotation, shipLength);
                }

                foreach (var shipLength in _shipLengths)
                {
                    var (coordinate, rotation) = aiBoard.PickSpaceRandom(shipLength);

                    aiBoard.Place(coordinate, rotation, shipLength);

                    /*// For debugging only
                    Console.WriteLine($"{coordinate} {rotation}");
                    _aiBoard.Render();
                    Console.Read();*/
                }

                Console.WriteLine("Thank you for picking your ships.\nIt's time to play!");
                
                _saveData = new SaveData()
                {
                    AiBoard = aiBoard,
                    PlayerBoard = playerBoard,
                    AiShotsBoard = aiShotsBoard,
                    PlayerShotsBoard = playerShotsBoard,
                    PlayerTurn = true
                };
            }
            
            _gameOver = false;
            
            while (!_gameOver)
            {
                Tuple<int, int> coordinate;
                
                Board shotBoard;
                Board shipBoard;
                
                if (_saveData.PlayerTurn ?? true)
                {
                    Console.Clear();
                    Console.WriteLine("\nIt's your turn! Here's your board:");
                    _saveData.PlayerBoard.Render();
                    Console.WriteLine("\nHere's the shots you've made:");
                    _saveData.PlayerShotsBoard.Render();
                    Console.WriteLine("Press a key to make a move...");
                    Console.Read();
                    (coordinate, _) = _saveData.PlayerShotsBoard.PickSpaceInteractive(
                        "Please enter a coordinate for your shot",
                        "Shooting");

                    shotBoard = _saveData.PlayerShotsBoard;
                    shipBoard = _saveData.AiBoard;
                }
                else
                {
                    (coordinate, _) = _saveData.AiShotsBoard.PickSpaceRandom();
                    
                    shotBoard = _saveData.AiShotsBoard;
                    shipBoard = _saveData.PlayerBoard;
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
                    Console.WriteLine($"Game over..., the {(_saveData.PlayerTurn ?? true ? "player" : "ai")} won!");
                    _gameOver = true;
                }

                _saveData.PlayerTurn = !_saveData.PlayerTurn ?? false;
                _saveData.Save();
            }
            
            _saveData.Delete();
            
            Console.WriteLine("Here were the were the ships at the end of the game...:");
            Console.WriteLine("For the player...");
            _saveData.PlayerBoard.Render();
            Console.WriteLine("");
            Console.WriteLine("and for the AI...");
            _saveData.AiBoard.Render();
            
            Console.WriteLine("Thank you for playing, press enter to return to the menu...");
            Console.ReadLine();
        }
    }
}