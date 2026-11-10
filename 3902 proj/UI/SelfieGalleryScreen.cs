using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TransformersGame.Core;

namespace TransformersGame.UI;

// Browses the selfies saved by SelfiePhoto. Only the visible page is kept in memory.
internal sealed class SelfieGalleryScreen : IDisposable
{
    private const int Columns = 3;
    private const int Rows = 2;
    private const int PerPage = Columns * Rows;
    private const int CellWidth = 280;
    private const int CellHeight = 158;
    private const int CellGap = 20;
    private const int GridLeft = 40;
    private const int GridTop = 62;
    private const int HeaderY = 13;
    private const int FooterY = 497;
    private const int Border = 3;
    private static readonly Color Accent = new Color(255, 207, 113);
    private static readonly Color PlaceholderColor = new Color(77, 58, 41);

    private readonly GraphicsDevice graphics;
    private readonly SpriteFont font;
    private readonly List<Texture2D> pageTextures = new();
    private string[] photos = Array.Empty<string>();
    private int page;
    private int viewing = -1;
    private ButtonState previousLeftButton;

    public SelfieGalleryScreen(GraphicsDevice graphics, SpriteFont font)
    {
        this.graphics = graphics;
        this.font = font;
    }

    private int PageCount => Math.Max(1, (photos.Length + PerPage - 1) / PerPage);

    public void Open()
    {
        photos = SelfiePhoto.ListPhotos();
        page = 0;
        viewing = -1;
        previousLeftButton = Mouse.GetState().LeftButton;
        LoadPage();
    }

    // Returns true when a photo was open and has just been closed.
    public bool CloseViewer()
    {
        if (viewing < 0) return false;

        viewing = -1;
        return true;
    }

    public void PreviousPage() => ChangePage(page - 1);

    public void NextPage() => ChangePage(page + 1);

    public void Update()
    {
        MouseState mouse = Mouse.GetState();
        if (mouse.LeftButton == ButtonState.Pressed && previousLeftButton == ButtonState.Released)
        {
            if (viewing >= 0)
            {
                viewing = -1;
            }
            else
            {
                OpenPhotoAt(mouse.Position);
            }
        }

        previousLeftButton = mouse.LeftButton;
    }

    public void Draw(SpriteBatch batch)
    {
        batch.Begin(samplerState: SamplerState.PointClamp);
        if (viewing >= 0)
        {
            DrawViewer(batch);
        }
        else
        {
            DrawGrid(batch);
        }

        batch.End();
    }

    public void Dispose() => DisposePage();

    private void ChangePage(int newPage)
    {
        if (viewing >= 0 || newPage < 0 || newPage >= PageCount) return;

        page = newPage;
        LoadPage();
    }

    private void OpenPhotoAt(Point position)
    {
        for (int slot = 0; slot < pageTextures.Count; slot++)
        {
            if (CellBounds(slot).Contains(position))
            {
                viewing = page * PerPage + slot;
                return;
            }
        }
    }

    private void LoadPage()
    {
        DisposePage();
        int first = page * PerPage;
        int last = Math.Min(photos.Length, first + PerPage);
        for (int i = first; i < last; i++)
        {
            pageTextures.Add(TryLoad(photos[i]));
        }
    }

    private Texture2D TryLoad(string path)
    {
        try
        {
            using FileStream stream = File.OpenRead(path);
            return Texture2D.FromStream(graphics, stream);
        }
        catch (IOException)
        {
            return null;
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    private void DisposePage()
    {
        foreach (Texture2D texture in pageTextures)
        {
            texture?.Dispose();
        }

        pageTextures.Clear();
    }

    private static Rectangle CellBounds(int slot) => new Rectangle(
        GridLeft + slot % Columns * (CellWidth + CellGap),
        GridTop + slot / Columns * (CellHeight + CellGap), CellWidth, CellHeight);

    private void DrawGrid(SpriteBatch batch)
    {
        string count = photos.Length == 1 ? "1 photo" : $"{photos.Length} photos";
        batch.DrawString(font, $"SELFIE GALLERY  {count}  page {page + 1}/{PageCount}",
            new Vector2(GridLeft, HeaderY), Accent);
        if (photos.Length == 0)
        {
            batch.DrawString(font, "No selfies yet. Press F2 in a room to take one.",
                new Vector2(GridLeft, GridTop), Color.White);
        }

        for (int slot = 0; slot < pageTextures.Count; slot++)
        {
            Rectangle cell = CellBounds(slot);
            Rectangle frame = new Rectangle(cell.X - Border, cell.Y - Border,
                cell.Width + 2 * Border, cell.Height + 2 * Border);
            HudFrame.Fill(batch, frame, Accent);
            DrawPhoto(batch, pageTextures[slot], cell);
        }

        batch.DrawString(font, "Click a photo to enlarge  |  Left / Right change page  |  F3 or Esc closes",
            new Vector2(GridLeft, FooterY), Color.White);
    }

    private void DrawViewer(SpriteBatch batch)
    {
        Viewport viewport = graphics.Viewport;
        int slot = viewing - page * PerPage;
        Texture2D photo = slot >= 0 && slot < pageTextures.Count ? pageTextures[slot] : null;
        Rectangle area = new Rectangle(40, 46, viewport.Width - 80, viewport.Height - 46 - 52);
        float scale = photo == null ? 1f
            : Math.Min(area.Width / (float)photo.Width, area.Height / (float)photo.Height);
        int width = photo == null ? area.Width : (int)(photo.Width * scale);
        int height = photo == null ? area.Height : (int)(photo.Height * scale);
        Rectangle bounds = new Rectangle(area.X + (area.Width - width) / 2,
            area.Y + (area.Height - height) / 2, width, height);
        batch.DrawString(font, $"SELFIE {viewing + 1}/{photos.Length}  {Path.GetFileNameWithoutExtension(photos[viewing])}",
            new Vector2(GridLeft, HeaderY), Accent);
        HudFrame.Fill(batch, new Rectangle(bounds.X - Border, bounds.Y - Border,
            bounds.Width + 2 * Border, bounds.Height + 2 * Border), Accent);
        DrawPhoto(batch, photo, bounds);
        batch.DrawString(font, "Click or Esc to go back", new Vector2(GridLeft, FooterY), Color.White);
    }

    private void DrawPhoto(SpriteBatch batch, Texture2D photo, Rectangle bounds)
    {
        if (photo != null)
        {
            batch.Draw(photo, bounds, Color.White);
            return;
        }

        HudFrame.Fill(batch, bounds, PlaceholderColor);
        const string message = "Can't open photo";
        Vector2 position = bounds.Center.ToVector2() - font.MeasureString(message) / 2f;
        batch.DrawString(font, message, position, Color.White);
    }
}
