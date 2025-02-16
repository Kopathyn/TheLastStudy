using System.Text.RegularExpressions;

namespace TPLnTM_Lab2
{
    public class RegularExpression
    {
        /// <summary>
        /// Функция проверки строки на "ПЕРЕМЕННАЯ = ВЫРАЖЕНИЕ"
        /// </summary>
        /// <param name="str">Строка</param>
        /// <returns>Результат</returns>
        static public bool CheckString(string str)
        {
            string pattern = @"^([a-zA-Z_][a-zA-Z0-9_]*=([0-9]+(\.[0-9]+)?|[a-zA-Z_][a-zA-Z0-9_]*|(\([^\)]*\))|([\+\*]))*)$"; 

            if (Regex.IsMatch(str, pattern))
                return true;
            else
                return false;
        }
    }
}