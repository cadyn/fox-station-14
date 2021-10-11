using System;
using System.Collections.Generic;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.AnthroSystem
{
    [Serializable, NetSerializable]
    public class AnthroMarking : IEquatable<AnthroMarking>, IComparable<AnthroMarking>, IComparable<string>
    {
        private List<Color> _markingColors = new();

        private AnthroMarking(string markingId,
            List<Color> markingColors)
        {
            MarkingId = markingId;
            _markingColors = markingColors;
        }

        public AnthroMarking(string markingId,
            IReadOnlyList<Color> markingColors)
            : this(markingId, new List<Color>(markingColors))
        {
        }

        /*
        public AnthroMarking(string markingId)
            : this(markingId, new List<Color>())
        {
        }
        */

        public AnthroMarking(string markingId, int colorCount)
        {
            MarkingId = markingId;
            List<Color> colors = new();
            for (int i = 0; i < colorCount; i++)
                colors.Add(Color.White);
            _markingColors = colors;
        }

        [DataField("markingId")]
        [ViewVariables]
        public string MarkingId { get; } = default!;

        [DataField("markingColor")]
        [ViewVariables]
        public IReadOnlyList<Color> MarkingColors => _markingColors;

        public void SetColor(int colorIndex, Color color) =>
            _markingColors[colorIndex] = color;

        public int CompareTo(AnthroMarking? marking)
        {
            if (marking == null) return 1;
            else return this.MarkingId.CompareTo(marking.MarkingId);
        }

        public int CompareTo(string? markingId)
        {
            if (markingId == null) return 1;
            return this.MarkingId.CompareTo(markingId);
        }

        public bool Equals(AnthroMarking? other)
        {
            if (other == null) return false;
            return (this.MarkingId.Equals(other.MarkingId));
        }

        // This is ABSURDLY messy,
        // but only because I want to **avoid** putting
        // a dynamic flat file prototype struct into SQL
        // that would end up translating to a many-many
        // while also attempting to keep layer rank
        // it's just really messy.
        new public string ToString()
        {
            // reserved character
            string sanitizedName = this.MarkingId.Replace('%', '_');
            List<string> colorStringList = new();
            foreach (Color color in _markingColors)
                colorStringList.Add(color.ToHex());

            return $"{sanitizedName}%{String.Join(',', colorStringList)}";
        }

        public static AnthroMarking? ParseFromDbString(string input)
        {
            if (input.Length == 0) return null;
            var split = input.Split('%');
            if (split.Length != 2) return null;
            List<Color> colorList = new();
            foreach (string color in split[1].Split(','))
                colorList.Add(Color.FromHex(color));

            return new AnthroMarking(split[0], colorList);
        }
    }
}
