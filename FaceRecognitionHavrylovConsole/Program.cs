using System;
using FaceRecognitionHavrylov;
using System.IO;
using System.Linq;

namespace FaceRecognitionHavrylovConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("Enter Image Path: ");
                    var path = Console.ReadLine();
                    Console.WriteLine($"Found faces: {OpenCVWrapper.RecognizeFaces(path)}");
                    var result = path == null ? $"{Directory.GetCurrentDirectory()}/result.png" : $"{path}result.{path.Split('.').Last()}";
                    Console.WriteLine($"Resulting image saved as {result}");
                    Console.WriteLine("Press any key");
                    Console.ReadKey();
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.WriteLine("Press any key");
                    Console.ReadKey();
                }
            }

        }
    }
}
