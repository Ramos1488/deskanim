using Drawing = System.Drawing;
using Forms = System.Windows.Forms;

namespace DeskAnim;

/// <summary>Dark colors for the WinForms tray context menu.</summary>
internal sealed class DarkColors : Forms.ProfessionalColorTable
{
    private static readonly Drawing.Color Bg = Drawing.Color.FromArgb(0x2A, 0x2A, 0x32);
    private static readonly Drawing.Color Line = Drawing.Color.FromArgb(0x3E, 0x3E, 0x4C);
    private static readonly Drawing.Color Hover = Drawing.Color.FromArgb(0x45, 0x45, 0x55);
    private static readonly Drawing.Color Accent = Drawing.Color.FromArgb(0x7C, 0x6C, 0xFF);

    public override Drawing.Color ToolStripDropDownBackground => Bg;
    public override Drawing.Color ImageMarginGradientBegin => Bg;
    public override Drawing.Color ImageMarginGradientMiddle => Bg;
    public override Drawing.Color ImageMarginGradientEnd => Bg;
    public override Drawing.Color MenuBorder => Line;
    public override Drawing.Color MenuItemBorder => Accent;
    public override Drawing.Color MenuItemSelected => Hover;
    public override Drawing.Color MenuItemSelectedGradientBegin => Hover;
    public override Drawing.Color MenuItemSelectedGradientEnd => Hover;
    public override Drawing.Color MenuItemPressedGradientBegin => Hover;
    public override Drawing.Color MenuItemPressedGradientEnd => Hover;
    public override Drawing.Color SeparatorDark => Line;
    public override Drawing.Color SeparatorLight => Line;
    public override Drawing.Color CheckBackground => Accent;
    public override Drawing.Color CheckSelectedBackground => Accent;
    public override Drawing.Color CheckPressedBackground => Accent;
}
