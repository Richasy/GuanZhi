
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace Project_GuanZhi.Models
{
    public class SystemFont
    {
        public string Name { get; set; }

        public Windows.UI.Xaml.Media.FontFamily FontFamily { get; set; }

        public int FamilyIndex { get; set; }

        public int Index { get; set; }

        public static List<SystemFont> GetFonts()
        {
            var fontList = new List<SystemFont>();

            var factory = new Factory();
            var fontCollection = factory.GetSystemFontCollection(false);
            var familyCount = fontCollection.FontFamilyCount;

            for (int i = 0; i < familyCount; i++)
            {
                var fontFamily = fontCollection.GetFontFamily(i);
                var familyNames = fontFamily.FamilyNames;
                int index;

                if (!familyNames.FindLocaleName(CultureInfo.CurrentCulture.Name, out index))
                {
                    if (!familyNames.FindLocaleName("en-us", out index))
                    {
                        index = 0;
                    }
                }

                string name = familyNames.GetString(index);
                fontList.Add(new SystemFont()
                {
                    Name = name,
                    FamilyIndex = i,
                    Index = index,
                    FontFamily=new Windows.UI.Xaml.Media.FontFamily(name)
                });
            }

            return fontList;
        }

        public List<Character> GetCharacters()
        {
            var factory = new Factory();
            var fontCollection = factory.GetSystemFontCollection(false);
            var fontFamily = fontCollection.GetFontFamily(FamilyIndex);

            var font = fontFamily.GetFont(Index);

            var characters = new List<Character>();
            var count = 65535;
            for (var i = 0; i < count; i++)
            {
                if (font.HasCharacter(i))
                {
                    characters.Add(new Character()
                    {
                        Char = char.ConvertFromUtf32(i),
                        UnicodeIndex = i
                    });
                }
            }

            return characters;
        }
    }
    public class Character
    {
        public string Char { get; set; }

        public int UnicodeIndex { get; set; }
    }
}
