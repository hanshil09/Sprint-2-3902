using System;
using System.Collections.Generic;
using System.IO;
using System.Security;

namespace TransformersGame.Core
{
    public static class LevelLoader
    {
        public static LevelLoadResult Load(string path)
        {
            ArgumentNullException.ThrowIfNull(path);

            List<string> messages = new List<string>();
            string[] lines = TryReadLines(path, messages);

            if (lines != null)
            {
                LevelParseResult parsed = LevelParser.Parse(lines);

                foreach (string warning in parsed.Warnings)
                {
                    messages.Add($"warning: {path}: {warning}");
                }

                if (parsed.Success)
                {
                    return new LevelLoadResult(parsed.Data, false, messages);
                }

                foreach (string error in parsed.Errors)
                {
                    messages.Add($"error: {path}: {error}");
                }
            }

            messages.Add("using the built-in fallback room instead");
            return new LevelLoadResult(LoadFallback(), true, messages);
        }

        private static string[] TryReadLines(string path, List<string> messages)
        {
            try
            {
                return File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, path));
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException or SecurityException)
            {
                messages.Add($"error: could not read {path}: {ex.Message}");
                return null;
            }
        }

        private static LevelData LoadFallback()
        {
            LevelParseResult parsed = LevelParser.Parse(DefaultLevel.Text.Split('\n'));
            if (!parsed.Success)
            {
                throw new InvalidOperationException("The built-in fallback level is invalid: " + string.Join("; ", parsed.Errors));
            }
            return parsed.Data;
        }
    }
}