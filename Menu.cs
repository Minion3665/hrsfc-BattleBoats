using System;
using System.Collections.Generic;

namespace BattleShips
{
    public class Menu
    {
        List<string> choices = new List<string>();

        public Menu AddChoice(string prompt)
        {
            choices.Add(prompt);
            return this;
        }
        
        public int Show()
        {
            Console.Clear();
            for (var i = 0; i < choices.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {choices[i]}");
            }

            int choice;

            string c;
            do
            {
                Console.Write("Choose your option: ");
                c = Console.ReadLine();
            } while (!int.TryParse(c, out choice) || choice < 1 || choice > choices.Count);
            return choice - 1;
        }
    }
}