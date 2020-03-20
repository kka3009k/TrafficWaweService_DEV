using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrafficWaveService.Dictionaries
{
    public class RuToLatin
    {
        private Dictionary<string, string> iso = new Dictionary<string, string>(); //ISO 9-95

        public RuToLatin()
        {
            Transliteration();
        }

        public string Front(string text)
        {
            string output = text;

            foreach (KeyValuePair<string, string> key in iso)
            {
                output = output.Replace(key.Key, key.Value);
            }

            return output;
        }

        private void Transliteration()
        {
            iso.Add("Є", "Ye");
            iso.Add("І", "I");
            iso.Add("Ѓ", "G");
            iso.Add("і", "i");
            iso.Add("№", "#");
            iso.Add("є", "ye");
            iso.Add("ѓ", "g");
            iso.Add("А", "A");
            iso.Add("Б", "B");
            iso.Add("В", "V");
            iso.Add("Г", "G");
            iso.Add("Д", "D");
            iso.Add("Е", "E");
            iso.Add("Ё", "Yo");
            iso.Add("Ж", "Zh");
            iso.Add("З", "Z");
            iso.Add("И", "I");
            iso.Add("Й", "I");
            iso.Add("К", "K");
            iso.Add("Л", "L");
            iso.Add("М", "M");
            iso.Add("Н", "N");
            iso.Add("О", "O");
            iso.Add("П", "P");
            iso.Add("Р", "R");
            iso.Add("С", "S");
            iso.Add("Т", "T");
            iso.Add("У", "U");
            iso.Add("Ф", "F");
            iso.Add("Х", "KH");
            iso.Add("Ц", "C");
            iso.Add("Ч", "Ch");
            iso.Add("Ш", "Sh");
            iso.Add("Щ", "Shch");
            iso.Add("Ъ", "'");
            iso.Add("Ы", "Y");
            iso.Add("Ь", "");
            iso.Add("Э", "E");
            iso.Add("Ю", "Yu");
            iso.Add("Я", "Ya");
            iso.Add("а", "a");
            iso.Add("б", "b");
            iso.Add("в", "v");
            iso.Add("г", "g");
            iso.Add("д", "d");
            iso.Add("е", "e");
            iso.Add("ё", "yo");
            iso.Add("ж", "zh");
            iso.Add("з", "z");
            iso.Add("и", "i");
            iso.Add("й", "i");
            iso.Add("к", "k");
            iso.Add("л", "l");
            iso.Add("м", "m");
            iso.Add("н", "n");
            iso.Add("о", "o");
            iso.Add("п", "p");
            iso.Add("р", "r");
            iso.Add("с", "s");
            iso.Add("т", "t");
            iso.Add("у", "u");
            iso.Add("ф", "f");
            iso.Add("х", "kh");
            iso.Add("ц", "c");
            iso.Add("ч", "ch");
            iso.Add("ш", "sh");
            iso.Add("щ", "shch");
            iso.Add("ъ", "");
            iso.Add("ы", "y");
            iso.Add("ь", "");
            iso.Add("э", "e");
            iso.Add("ю", "yu");
            iso.Add("я", "ya");
            iso.Add("«", "");
            iso.Add("»", "");
            iso.Add("—", "-");
        }
    }
}