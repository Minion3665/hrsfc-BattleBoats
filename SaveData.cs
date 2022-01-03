using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
// See https://docs.microsoft.com/en-us/dotnet/api/system.text.json?view=net-6.0
// I'm not sure if this is included in .NET on Windows by default, but it is available on NuGet under the Microsoft namespace.

namespace BattleShips
{
    public class SaveData
    {
        public Board AiBoard = null;
        public Board PlayerBoard = null;
        public Board AiShotsBoard = null;
        public Board PlayerShotsBoard = null;
        public bool? PlayerTurn = null;

        private readonly string _filename;

        public static string[][] StateToJaggedArray(string[,] state)
        {
            var jaggedArray = new string[state.GetLength(0)][];
            for (var i = 0; i < state.GetLength(0); i++)
            {
                jaggedArray[i] = new string[state.GetLength(1)];
                for (var j = 0; j < state.GetLength(1); j++)
                {
                    jaggedArray[i][j] = state[i, j];
                }
            }
            return jaggedArray;
        }
        
        public static string[,] JsonJaggedArrayToState(JsonElement element)
        {
            var jaggedArray = JsonSerializer.Deserialize<string[][]>(element.GetRawText());
            
            if (jaggedArray == null)
            {
                throw new ArgumentException("Jagged array is null");
            }
            
            // Check that all rows are the same length
            var rowLength = jaggedArray[0].Length; 
            for (var i = 1; i < jaggedArray.Length; i++)
            {
                if (jaggedArray[i].Length != rowLength)
                {
                    throw new ArgumentException("All rows must be the same length");
                }
            }

            var state = new string[jaggedArray.Length, jaggedArray[0].Length];
            for (var i = 0; i < jaggedArray.Length; i++)
            {
                for (var j = 0; j < jaggedArray[0].Length; j++)
                {
                    state[i, j] = jaggedArray[i][j];
                }
            }
            return state;
        }

        public SaveData(string filePath = "./save.json")
        {
            _filename = filePath;
        }
        
        public void Save()
        {
            // We need to convert our multi-dimensional arrays to jagged arrays for JSON serialization

            var json = JsonSerializer.Serialize(new Dictionary<string, dynamic>{
                {"AiBoard", StateToJaggedArray(AiBoard.State)},
                {"PlayerBoard", StateToJaggedArray(PlayerBoard.State)},
                {"AiShotsBoard", StateToJaggedArray(AiShotsBoard.State)},
                {"PlayerShotsBoard", StateToJaggedArray(PlayerShotsBoard.State)},
                {"PlayerTurn", PlayerTurn}
            });
            File.WriteAllText(_filename, json);
        }
        
        public bool Load(Random random = null)
        {
            if (!File.Exists(_filename))
            {
                Console.WriteLine("No save file found");
                return false;
            }

            var json = File.ReadAllText(_filename);

            var r = random ?? new Random();
            
            // We need to remember to convert our jagged arrays back to multidimensional arrays.

            try
            {
                var data = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(json);

                if (data == null)
                {
                    Console.WriteLine("Invalid save file");
                    return false;
                }
                
                AiBoard = Board.FromState(r, JsonJaggedArrayToState(data["AiBoard"]));
                PlayerBoard = Board.FromState(r, JsonJaggedArrayToState(data["PlayerBoard"]));
                AiShotsBoard = Board.FromState(r, JsonJaggedArrayToState(data["AiShotsBoard"]));
                PlayerShotsBoard = Board.FromState(r, JsonJaggedArrayToState(data["PlayerShotsBoard"]));
                PlayerTurn = JsonSerializer.Deserialize<bool>(data["PlayerTurn"].GetRawText());
            } catch (Exception e)  // Likely an ArgumentError from converting from jagged array to multidimensional array or a JSON parse error
            {
                Console.WriteLine($"Deserialization error: {e}");
                return false;
            }

            return true;
        }

        public void Delete()
        {
            File.Delete(_filename);
        }
    }
}