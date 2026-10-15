using System.Collections.Generic;

namespace TransformersGame.Core
{
    public sealed class LevelLoadResult
    {
        public LevelLoadResult(LevelData data, bool usedFallback, IReadOnlyList<string> messages)
        {
            Data = data;
            UsedFallback = usedFallback;
            Messages = messages;
        }

        public LevelData Data { get; }

        public bool UsedFallback { get; }

        public IReadOnlyList<string> Messages { get; }
    }
}