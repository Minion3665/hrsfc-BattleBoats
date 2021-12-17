using System;
using System.Data;
using System.Runtime.Remoting.Messaging;

namespace BattleShips
{
	internal static class Program
	{
		private static void Main()
		{
			var finished = false;
			while (!finished)
			{
				switch (Menu())
				{
					case 1:
						// Play the game
						Play();
						break;
					case 2:
						// Show the instructions
						Instructions();
						break;
					default:
						// Quit the game
						finished = true;
						break;
				}
			}
		}

		private static int Menu()
		{
			// Show a menu
			// The player should be able to do the following:
			// Play battleships
			// View how to play
			// Quit the game

			// Ask the player what they want to do
			int choice;
			do
			{
				Console.WriteLine("What would you like to do?");
				Console.WriteLine("1. Play Battleships");
				Console.WriteLine("2. View how to play");
				Console.WriteLine("3. Quit");
				Console.Write(">>> ");
			} while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 ||
			         choice > 3); // Keep asking until the user enters a valid choice

			return choice;
		}

		private static void Instructions()
		{
			// Show the instructions
			// The player should be able to do the following:
			// View the instructions
			// Return to the main menu
			// The instructions should scroll and fit the size of the console
		}

		private static void Play()
		{
			// Play the game
			// The player should be able to do the following:
			// Place the ships on the board
			// Shoot at the AI board
			// Return to the main menu

			// Create the boards
			char[,] playerBoard = InitializeBoard();
			char[,] AIBoard = InitializeBoard();

			// Place the ships
			PlayerPlaceShips(playerBoard);
			AIPlaceShips(AIBoard);
		}

		private static char[,] InitializeBoard(int size = 10)
		{
			// Initialize the board
			// The board should be a 2D array
			// Initialize every cell to "-"
			var board = new char[size, size];

			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					board[i, j] = '-';
				}
			}

			return board;
		}

		private static void PlayerPlaceShips(char[,] board, int[] shipSizes = null)
		{
			shipSizes = shipSizes ?? new int[] {5, 4, 3, 3, 2};
			// Place the ships on the board
			// The player should be able to do the following:
			// Place the ships on the board
			// Return to the main menu
			// The ships should be placed depending on player input
			// The player should be able to input the character 'r' to rotate the ship
			// The positions of the ships should be validated with the function PlaceShip

			// Ask the player to place the ships
			for (int i = 0; i < shipSizes.Length; i++)
			{
				// Ask the player where they want to place the ship
				// The player should be able to do the following:
				// Input the row and column of the top left cell as a string separated by a space or a comma
				// Rotate the ship with the character 'r'
				// This should be done until the ship is placed correctly
				// This should be handled with Console.ReadKey() instead of Console.ReadLine()
				// An ascii art graphic of the ship should update when the player rotates it

				var placed = false;
				var rotated = false;
				var error = "";
				var input = "";
				while (!placed)
				{
					// Clear the console
					Console.Clear();

					Console.WriteLine($"Place your {shipSizes[i].ToString()}-cell ship");
					Console.WriteLine(
						"Enter the row and column of the top left cell");
					Console.WriteLine("Press 'r' to rotate the ship");

					// Print the board
					PrintBoard(board);

					// Display the ship
					// The ship should be displayed to the right of the board
					// The ship should be rotated depending on the player input
					Console.WriteLine($@"{rotated}, {shipSizes[i].ToString()}"); // TODO: Display the ship

					// Set the console color to red
					Console.ForegroundColor = ConsoleColor.Red;
					// Display the error
					Console.WriteLine(error);
					// Reset the console color
					Console.ResetColor();
					// Clear the error
					error = "";

					// Write the current input
					Console.WriteLine(input);
					
					// Get the input
					var key = Console.ReadKey().Key;

					// If the character is 'r', rotate the ship
					if (key == ConsoleKey.R)
					{
						rotated = !rotated;
					}
					else if (key == ConsoleKey.Enter)
					{
						// If the character is enter and both coordinates have been entered, place the ship
						var row = int.Parse(input.Split(' ')[0]);
						var col = int.Parse(input.Split(' ')[1]);
						if (PlaceShip(board, shipSizes[i], row, col, rotated))
						{
							placed = true;
						}
					}
					else
					{
						// If the character is a number, add it to the input
						if (key >= ConsoleKey.D0 && key <= ConsoleKey.D9)
						{
							// (key - ConsoleKey.D0);
						}
						else if (key == ConsoleKey.Backspace)
						{
							// If the input is empty, we can't remove a character
							if (input.Length == 0)
							{
								error = "You've already removed the last character";
								continue;
							}
							
							// If the character is backspace, remove the last character from the input if it's a number, or the last 2 if it's a space
							if (input[input.Length - 1] == ' ')
							{
								input = input.Substring(0, input.Length - 2);
							}
							else
							{
								input = input.Substring(0, input.Length - 1);
							}
						}
					}
				}
			}
		}

		private static void AIPlaceShips(char[,] board, int[] shipSizes = null)
		{
			// Place the ships on the board
			// The AI should be able to do the following:
			// Place the ships on the board
			// Return to the main menu
			// The ships should be placed randomly
			// The positions of the ships should be validated with the function PlaceShip
			shipSizes = shipSizes ?? new[] {5, 4, 3, 3, 2};
		}

		private static bool PlaceShip(char[,] board, int shipSize, int row, int col, bool rotate = false)
		{
			// Validate the position of the ships and place them on the board
			// The ships should be placed in a way that they don't overlap
			// The ships should be placed in a way that they don't go out of the board

			// Check if the ship is out of the board
			if (row + shipSize > board.GetLength(0) || col + shipSize > board.GetLength(1))
			{
				return false;
			}

			// Check if the ship overlaps with another ship
			for (int i = 0; i < shipSize; i++)
			{
				for (int j = 0; j < shipSize; j++)
				{
					if (board[row + i, col + j] != '-')
					{
						return false;
					}
				}
			}

			// Place the ship
			for (int i = 0; i < shipSize; i++)
			{
				for (int j = 0; j < shipSize; j++)
				{
					board[row + i, col + j] = 'S';
				}
			}

			return true;
		}

		private static void PrintBoard(char[,] board, int? highlightRow = null, int? highlightCol = null)
		{
			// Print the board
			// The board should be printed to the console
			// The board should be printed with the following format:
			// /  A B C D E F G H I J
			// 1  - - - - - - - - - -
			// 2  - - - - - - - - - -
			// 3  - - - - - - - - - -
			// 4  - - - - - - - - - -
			// 5  - - - - - - - - - -
			// 6  - - - - - - - - - -
			// 7  - - - - - - - - - -
			// 8  - - - - - - - - - -
			// 9  - - - - - - - - - -
			// 10 - - - - - - - - - -

			// Write the column letters
			Console.Write("/ ");
			for (var i = 0; i < board.GetLength(1); i++)
			{
				Console.Write($" {((char) (i + 65)).ToString()}");
			}
			Console.WriteLine();

			for (var i = 0; i < board.GetLength(0); i++)
			{
				// Write the row number, padded so that everything is inline
				Console.Write((i + 1).ToString().PadRight(2));
				for (var j = 0; j < board.GetLength(1); j++)
				{
					Console.Write($" {board[i, j].ToString()}");
				}
				Console.WriteLine();
			}
		}
	}
}