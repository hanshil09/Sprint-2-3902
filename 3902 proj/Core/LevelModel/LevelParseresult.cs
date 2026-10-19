using System.Collections.Generic;

namespace TransformersGame.Core
{
    public sealed class LevelParseResult
    {
        public LevelParseResult(LevelData data, IReadOnlyList<string> errors, IReadOnlyList<string> warnings)
        {
            Data = data;
            Errors = errors;
            Warnings = warnings;
        }

        public LevelData Data { get; }

        public IReadOnlyList<string> Errors { get; }

        public IReadOnlyList<string> Warnings { get; }

        public bool Success => Errors.Count == 0 && Data != null;
    }
}