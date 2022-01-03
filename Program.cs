using System;

namespace BattleShips
{
	internal static class Program
	{
		private static void Main()
		{
			// DebugColors();

			var menu = new Menu();
			menu.AddChoice("Play against an AI");

			var save = new SaveData();
			if (save.Load())
			{
				menu.AddChoice("Load previous game");
			}

			menu.AddChoice("Show instructions")
				.AddChoice("Quit game");

			var quit = false;

			while (!quit)
			{
				var choice = menu.Show();
				switch (menu.choices[choice])
				{
					case "Play against an AI":
						var game = new Game();
						game.Start();
						break;
					case "Load previous game":
						game = new Game(save: save);
						game.Start(); 
						break;
					case "Show instructions":
						var interactiveInstructions = new InteractiveInstructions();
						interactiveInstructions.Show();
						break;
					case "Quit game":
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