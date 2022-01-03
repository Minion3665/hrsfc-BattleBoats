using System;

namespace BattleShips
{
	internal static class Program
	{
		private static void Main()
		{
			// DebugColors();

			var menu = new Menu();
			menu.AddChoice("Play against an AI")
				.AddChoice("Show instructions")
				.AddChoice("Quit game");

			var quit = false;

			while (!quit)
			{
				var choice = menu.Show();
				switch (choice)
				{
					case 0:
						var game = new Game();
						game.Start();
						break;
					case 1:
						var interactiveInstructions = new InteractiveInstructions();
						interactiveInstructions.Show();
						break;
					case 2:
						quit = true;
						break;
				}
			}
		}

		private static void DebugColors()
		{
			foreach (ConsoleColor color in Enum.GetValues(typeof(ConsoleColor)))
			{
				Console.BackgroundColor = color;
				Console.Write(color.ToString());
				Console.ResetColor();
				Console.WriteLine();
			}
		}
	}
}