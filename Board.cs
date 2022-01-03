using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BattleShips
{
    public class Board
    {
        private readonly Random _random;

        private const string EmptyBoardSpaceSymbol = "•";
        public Board(Random random = null, int size = 10)
        {
            _random = random ?? new Random();
            State = new string[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    State[i, j] = EmptyBoardSpaceSymbol;
                }
            }
        }
        public (Tuple<int, int>, bool) PickSpaceInteractive(string prompt = "Please enter a coordinate to shoot at...", string action = "Shooting at", int spaceLength = 1)
        {
            // This method will use Console.ReadKey rather than console.readline to allow for a more interactive experience
            // This method will only update the board if the user enters a valid coordinate
            // This method will render the board after each coordinate part is entered, with the user's input highlighted
            Console.Clear();
            Console.CursorVisible = false;

            int? coordinateX = null;
            int? coordinateY = null;

            var submitted = false;
            var errorMakesInvalid = false;

            var rotate = false;

            string error = "";

            if (spaceLength < 1) spaceLength = 1;
            
            while (!submitted)
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine(prompt + "\n");
                Render(coordinateX, coordinateY, errorMakesInvalid ? ConsoleColor.Red : ConsoleColor.Green,  spaceLength, rotate);
                Console.WriteLine();
                Console.Write($"{action} ");
                if (spaceLength > 1)
                {
                    Console.Write($"{spaceLength} spaces from ");
                }
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write(coordinateX == null ? "-" : ((char)(coordinateX + 'a')).ToString().ToUpper());
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(coordinateY == null ? "-" : (coordinateY + 1).ToString());
                Console.ResetColor();
                if (spaceLength > 1)
                {
                    Console.Write($", rotated {(rotate ? "down       " : "to the left")} ");
                }

                Console.WriteLine("\n");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(error + new string(' ', Console.WindowWidth - 1));
                error = null;
                Console.ResetColor();
                var key = Console.ReadKey();
                Console.Write("\b ");
                if (key.Key == ConsoleKey.Spacebar)
                {
                    rotate = !rotate;
                } else if (key.Key - ConsoleKey.A < State.GetLength(0) && key.Key - ConsoleKey.A >= 0)
                {
                    coordinateX = key.Key - ConsoleKey.A;
                } else if (key.Key - ConsoleKey.D0 < State.GetLength(1) && key.Key - ConsoleKey.D0 >= 0)
                {
                    if ((coordinateY + 1) * 10 + (key.Key - ConsoleKey.D0) - 1 < State.GetLength(0))
                    {
                        coordinateY = (coordinateY + 1) * 10 + (key.Key - ConsoleKey.D0) - 1;
                    }
                    else if (key.Key - ConsoleKey.D0 == 0)
                    {
                        coordinateY = State.GetLength(0) - 1;
                    }
                    else
                    {
                        coordinateY = key.Key - ConsoleKey.D0 - 1;
                    }
                } else if (key.Key == ConsoleKey.Enter)
                {
                    if (coordinateX != null && coordinateY != null && !errorMakesInvalid)
                    {
                        submitted = true;
                    }
                    else
                    {
                        error = "You need to enter a valid spot";
                    }
                }
                else
                {
                    error = "That isn't a valid spot";
                }

                if (coordinateX == null || coordinateY == null) continue;
                // If either of the coordinates are null, there's no selection to check
                // We don't want to continue in order to avoid errors that trying to check anyway could bring
                
                var selectionLeavesBoard = false;

                if (rotate)
                {
                    if (coordinateY + spaceLength > State.GetLength(1)) selectionLeavesBoard = true;
                }
                else
                {
                    if (coordinateX + spaceLength > State.GetLength(0)) selectionLeavesBoard = true;
                }

                var selectionCrossesOther = false;

                if (selectionLeavesBoard) {
                    error = "Your selection would leave the board...";
                } else { 
                    // We only check selectionCrossesOther if not selectionLeavesBoard, because otherwise we will get an index error
                    if (Overlaps(new Tuple<int, int>((int)coordinateX, (int)coordinateY), rotate, spaceLength))
                    {
                        selectionCrossesOther = true;
                        error = "Your selection would cross another ship...";
                    }
                }

                errorMakesInvalid = selectionLeavesBoard || selectionCrossesOther;
            }
            
            return (new Tuple<int, int>((int)coordinateX, (int)coordinateY), rotate);
        }
        public (Tuple<int, int>, bool) PickSpaceRandom(int spaceLength = 1)
        {
            // Pick a random rotation
            var rotate = _random.Next(0, 2) == 0;
            
            Tuple<int, int> coordinate;
            
            // While the selection overlaps another ship, pick coordinates
            do
            {
                // Only include coordinates that don't leave the board
                var coordinateX = _random.Next(0, State.GetLength(0) - (!rotate ? spaceLength : 0)); 
                var coordinateY = _random.Next(0, State.GetLength(1) - (rotate ? spaceLength : 0));
                coordinate = new Tuple<int, int>(coordinateX, coordinateY);
            } while (Overlaps(coordinate, rotate, spaceLength));
            
            return (coordinate, rotate);
        }
        public void Place(Tuple<int, int> coordinate, bool rotation = false, int length = 1, char character = 'O')
        {
            foreach (var singleCoordinate in GetSelectedSpaces(coordinate, rotation, length))
            {
                var (x, y) = singleCoordinate;
                State[x, y] = character.ToString();
            }
        }
        public bool HasNoShips(char[] shipCharacters = null)
        {
            shipCharacters = shipCharacters ?? new[] {'O'};

            return shipCharacters.All(character => State.Cast<string>().All(space => space != character.ToString()));
        }
        public bool Overlaps(Tuple<int, int> coordinate, bool rotation = false, int length = 1)
        {
            foreach (var singleCoordinate in GetSelectedSpaces(coordinate, rotation, length))
            {
                var (x, y) = singleCoordinate;
                if (State[x, y] != EmptyBoardSpaceSymbol)
                {
                    return true;
                }
            }
            return false;
        }
        private static IEnumerable<Tuple<int, int>> GetSelectedSpaces(Tuple<int, int> coordinate, bool rotation, int shipLength)
        {
            var result = new Tuple<int, int>[shipLength];
            var (x, y) = coordinate;

            for (var i = 0; i < shipLength; i++)
            {
                result[i] = new Tuple<int, int>(x + (!rotation ? i : 0), y + (rotation ? i : 0));
            }

            return result;
        }
        public void Render(int? highlightX = null, int? highlightY = null, ConsoleColor highlightColor = ConsoleColor.Green, int highlightLength = 1, bool highlightRotate = false) 
        {
            // Write the letters along the top of the board
            Console.Write("/ ");
            for (var i = 0; i < State.GetLength(0); i++)
            {
                Console.Write($" {(char) ('A' + i)}");
            }
            Console.WriteLine();
            
            for (var j = 0; j < State.GetLength(1); j++)
            {
                Console.Write((j + 1).ToString().PadRight(2));
                for (var i = 0; i < State.GetLength(0); i++)
                {
                    Console.Write(" ");
                    
                    if (
                        (!highlightRotate && i >= highlightX && i < highlightX + highlightLength && j == highlightY) || 
                        (highlightRotate && j >= highlightY && j < highlightY + highlightLength && i == highlightX))
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = highlightColor;
                    } else if (i == highlightX)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.BackgroundColor = ConsoleColor.Black;
                    } else if (j == highlightY)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        Console.ResetColor();
                    }
                    Console.Write($"{State[i, j]}");
                }
                Console.ResetColor();
                Console.WriteLine();
            }
        }
        
        public static Board FromState(Random random, string[,] boardState)
        {
            var board = new Board(random, boardState.GetLength(0))
            {
                State = boardState
            };
            return board;
        }

        public string[,] State { get; private set; }
    }
}