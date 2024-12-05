using System;
using System.IO;

namespace SoundCrashFix
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: SoundCrashFix.exe <path_to_userdata0010>");
                return;
            }

            string filePath = args[0];
            long offset = 0x204E;
            byte patchValue = 0x01;

            Console.WriteLine("SoundCrashFix is running...");
            Console.WriteLine($"Patching file: {filePath}");

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Error: File not found. Ensure the path is correct.");
                return;
            }

            PatchFile(filePath, offset, patchValue);

            Console.WriteLine("\nDone! Press [Esc] to exit.");

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(intercept: true).Key;
                    if (key == ConsoleKey.Escape)
                    {
                        Console.WriteLine("\nExiting...");
                        break;
                    }
                }
            }
        }

        static void PatchFile(string filePath, long offset, byte value)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
                {
                    fs.Seek(offset, SeekOrigin.Begin);
                    byte currentValue = (byte)fs.ReadByte();

                    if (currentValue == value)
                    {
                        Console.WriteLine($"No changes needed. The value at offset 0x{offset:X} is already {value:X2}.");
                    }
                    else
                    {
                        fs.Seek(offset, SeekOrigin.Begin);
                        fs.WriteByte(value);
                        Console.WriteLine($"Value at offset 0x{offset:X} changed from {currentValue:X2} to {value:X2}.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error patching file: {ex.Message}");
            }
        }
    }
}
