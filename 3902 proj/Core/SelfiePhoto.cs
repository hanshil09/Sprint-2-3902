using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace TransformersGame.Core
{
    internal static class SelfiePhoto
    {
        public const string FolderName = "Armadillo Selfies";

        public static Task<bool> SaveAsync(Texture2D photo)
        {
            // Read the render target on the graphics thread; write the file off that thread.
            using MemoryStream png = new MemoryStream();
            photo.SaveAsPng(png, photo.Width, photo.Height);
            byte[] bytes = png.ToArray();
            return Task.Run(() => TryWrite(bytes));
        }

        private static bool TryWrite(byte[] png)
        {
            try
            {
                string profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                if (string.IsNullOrEmpty(profile)) return false;

                string folder = Path.Combine(profile, "Downloads", FolderName);
                Directory.CreateDirectory(folder);
                string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss-fff", CultureInfo.InvariantCulture);
                string path = Path.Combine(folder, $"Selfie-{timestamp}.png");
                using FileStream output = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
                output.Write(png);
                Console.WriteLine($"Selfie saved: {path}");
                return true;
            }
            catch (IOException error)
            {
                Console.WriteLine($"Could not save selfie: {error.Message}");
                return false;
            }
            catch (UnauthorizedAccessException error)
            {
                Console.WriteLine($"Could not save selfie: {error.Message}");
                return false;
            }
        }
    }
}
