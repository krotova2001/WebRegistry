using System.Text.RegularExpressions;

namespace WebRegistry.Services.ShifrRezerver
{
    public class ShifrGenerator
    {
        public static string GetRandomShifr()
        {
            var random = new Random();
            string chars = "КПС";
            string number = "1234567890";
            int size = 10;
            string randomstring = "";

            for (int i = 0; i < size; i++)
            {
                int x = random.Next(number.Length);
                randomstring = randomstring + number[x];
            }
            return chars + " " + randomstring;
        }

        /// <summary>
        /// Найти следующий по порядку свободный шифр
        /// </summary>
        /// <param name="exeptions">Список существующих шифров</param>
        /// <returns></returns>
        public static string GetNextShifr(string[] exeptions)
        {
            Regex regex = new Regex(@"КПС(\d*)", RegexOptions.IgnorePatternWhitespace);
            int newIndex = 0;
            List<int> indexes = new List<int>();
            foreach (string s in exeptions)
            {
                if (regex.IsMatch(s))
                {
                    int.TryParse(s.Split("КПС")[1], out newIndex);
                    indexes.Add(newIndex);
                }
            }
            int max = indexes.Max();
            return "КПС " + (++max);
        }

        //дать уникальную строку, не совпадающую с массивом
        public static string GetUniqString(string[] exeptions)
        {
            string res = GetRandomShifr();
            foreach (string s in exeptions)
            {
                if (res == s)
                    return GetUniqString(exeptions);
            }
            return res;

        }

    }
}
