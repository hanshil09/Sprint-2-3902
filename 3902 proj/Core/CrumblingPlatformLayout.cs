using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TransformersGame.Entities;
using TransformersGame.Factories;
using TransformersGame.Interfaces;

namespace TransformersGame.Core
{
    internal static class CrumblingPlatformLayout
    {
        public static void Apply(List<IBlock> blocks, LevelData data, Func<int, int, Vector2> cellPosition, Random random)
        {
            List<Site> sites = FindSites(data);
            int count = Math.Min(sites.Count, random.Next(2, 4));
            int firstKind = random.Next(3);
            for (int i = 0; i < count; i++)
            {
                int index = random.Next(sites.Count);
                Site site = sites[index];
                sites.RemoveAt(index);
                Vector2 position = cellPosition(site.Column, site.Row);
                int tileSize = BlockSpriteFactory.BlockSize;
                Rectangle bounds = new Rectangle((int)position.X, (int)position.Y, site.Length * tileSize, tileSize);
                blocks.RemoveAll(block => bounds.Contains(block.Bounds));
                blocks.Add(new CrumblingPlatform(position, site.Length, (CrumbleKind)((firstKind + i) % 3)));
            }
        }

        private static List<Site> FindSites(LevelData data)
        {
            List<Site> sites = new List<Site>();
            List<Placement<BlockKind>> tiles = new List<Placement<BlockKind>>(data.Blocks);
            tiles.Sort(CompareTiles);
            int index = 0;
            while (index < tiles.Count)
            {
                Placement<BlockKind> first = tiles[index];
                int length = RunLength(tiles, index);
                if (length >= 2 && first.Row < data.Rows - 1
                    && first.Kind != BlockKind.Hazard && first.Kind != BlockKind.Crate)
                {
                    sites.Add(new Site(first.Column, first.Row, length));
                }
                index += length;
            }
            return sites;
        }

        private static int RunLength(List<Placement<BlockKind>> tiles, int index)
        {
            int length = 1;
            Placement<BlockKind> first = tiles[index];
            while (index + length < tiles.Count && tiles[index + length].Row == first.Row
                && tiles[index + length].Column == first.Column + length
                && tiles[index + length].Kind == first.Kind)
            {
                length++;
            }
            return length;
        }

        private static int CompareTiles(Placement<BlockKind> left, Placement<BlockKind> right)
        {
            int row = left.Row.CompareTo(right.Row);
            return row == 0 ? left.Column.CompareTo(right.Column) : row;
        }

        private sealed class Site
        {
            public Site(int column, int row, int length)
            {
                Column = column;
                Row = row;
                Length = length;
            }
            public int Column { get; }
            public int Row { get; }
            public int Length { get; }
        }
    }
}
