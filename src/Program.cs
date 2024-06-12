using System.Runtime.InteropServices;
using Windows.UI;
using Windows.UI.ViewManagement;

var accentPalette = new AccentPalette();

foreach (var (type, color) in Enum.GetValues<UIColorType>()
    .Select(c => (type: c, color: accentPalette.GetColorValue(c)))
    .Where(c => c.color.A == 0xff))
{
    Console.WriteLine($"{type}: \t#{color.R:X2}{color.G:X2}{color.B:X2}");
}

Console.WriteLine();
Console.WriteLine("Press any key to continue...");
Console.ReadKey();

public class AccentPalette
{
    private Color[]? colors;

    public Color GetColorValue(UIColorType colorType)
    {
        if (colors == null)
        {
            var accentPaletteBytes = Microsoft.Win32.Registry.CurrentUser
                .OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Accent")?
                .GetValue("AccentPalette") as byte[];
            if (accentPaletteBytes != null)
            {
                var intColors = MemoryMarshal.Cast<byte, int>(accentPaletteBytes);

                colors = new Color[intColors.Length];

                for (int i = 0; i < intColors.Length; i++)
                {
                    unchecked
                    {
                        colors[i] = Color.FromArgb(
                            0xff,
                            (byte)(intColors[i]),
                            (byte)(intColors[i] >> 8),
                            (byte)(intColors[i] >> 16));
                    }
                }
            }
            else
            {
                colors = [];
            }
        }

        var idx = MapToColorIndex(colorType);
        if (idx >= 0 && idx < colors.Length)
        {
            return colors[idx];
        }

        return default;
    }

    private static int MapToColorIndex(UIColorType colorType)
    {
        if ((int)colorType <= 1 || (int)colorType > 8) return -1;

        return 8 - (int)colorType;
    }
}