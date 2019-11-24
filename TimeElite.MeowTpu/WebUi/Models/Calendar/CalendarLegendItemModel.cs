using System.Drawing;

namespace WebUi.Models.Calendar
{
    /// <summary>
    /// Вью-модель легенды календаря.
    /// </summary>
    public class CalendarLegendItemModel
    {
        /// <summary>
        /// Цвет.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Имя.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Цвет в формате Hex.
        /// </summary>
        public string ColorHex => $"#{Color.R:X2}{Color.G:X2}{Color.B:X2}";

        /// <summary>
        /// Ссылка на iCal.
        /// </summary>
        public string iCalUrl { get; set; }
    }
}