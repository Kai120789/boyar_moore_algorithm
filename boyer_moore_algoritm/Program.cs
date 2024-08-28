using System;
using System.Collections.Generic;
using System.Linq;

namespace CWA
{
    class Program
    {
        static void Main(string[] args)
        {
            //string str = GetString();
            //Console.Write("Введите количество подстрок: ");
            //int N = Convert.ToInt32(Console.ReadLine());
            //for (int i = 0; i < N; i++)
            //    SearchString(GetSubString(), str, GetMas());

            //Console.Write("Введите строку: ");
            //string str1 = Console.ReadLine();
            //Console.Write("Введите подстроку: ");
            //string str2 = Console.ReadLine();
            //SearchString(str2, str1, GetMas());

            SearchString("ere", "derdogererer", GetMas());
        }

        public static void Print(int[] res)
        {
            if (res[0] == -1)
            {
                Console.WriteLine(res[0]);
                Console.WriteLine("Количество вхождений: 0");
            }
            else
            {
                int count = 0;
                for (int i = 0; res[i] != -1; i++)
                {
                    Console.WriteLine("Индекс вхождения: " + res[i]);
                    count++;
                }
                Console.WriteLine($"Количество вхождений: {count}");
            }
        }

        public static int[] GetMas()
        {
            int[] res = new int[(int.MaxValue - 1) / 4];
            for (int k = 0; k < res.Length; k++)             //заполняем массив индексов -1, на случай попадания подстроки с начальным индексом 0
            {
                res[k] = -1;
            }
            return res;
        }

        public static string GetString()
        {
            Random rnd = new Random();
            Console.Write("Введите размер строки: ");
            int stringlen = Convert.ToInt32(Console.ReadLine());
            if (stringlen > ((int.MaxValue - 1) / 4))
                stringlen = (int.MaxValue - 1) / 4;
            int randValue;
            string strIn = "abcdefghijkl mnopqrstuvwxyz";
            string strOut = "";
            for (int i = 0; i < stringlen; i++)
            {
                randValue = rnd.Next(0, 26);
                strOut += strIn[randValue];
            }
            //Console.WriteLine("Строка: " + strOut);
            return strOut;
        }

        public static string GetSubString()
        {
            Random rand = new Random();
            Console.Write("Введите размер подстроки: ");
            int substringlen = Convert.ToInt32(Console.ReadLine());
            if (substringlen > ((int.MaxValue - 1) / 4))
                substringlen = (int.MaxValue - 1) / 4;
            int randVal;
            string substrIn = "abcdefghijkl mnopqrstuvwxyz";
            string substrOut = "";
            for (int i = 0; i < substringlen; i++)
            {
                randVal = rand.Next(0, 26);
                substrOut += substrIn[randVal];
            }
            //Console.WriteLine("Строка: " + substrOut);
            return substrOut;
        }

        public static int[] SearchString(string find, string main, int[] res)
        {
            if (find.Length > main.Length)
            {
                Console.WriteLine("Подстрока больше строки");
                SearchString(GetSubString(), GetString(), GetMas());
                return res;
            }
            else
            {
                DateTime t1 = DateTime.Now;
                int index = 0;

                int findl = find.Length;                //проверяемый символ строки
                int findl2 = find.Length;               //проверяемый символ подстроки

                int[] tap = new int[find.Length];               //массив сдвигов при нахождении хорошего суффикса
                string[] good = new string[find.Length];        //массив хороших суффиксов

                GoodCharHeuristic(find, tap, good);

                int[] top = new int[find.Length];               //массив сдвигов при нахождении плохого символа
                char[] bad = new char[find.Length];             //массив плохих символов

                BadCharHeuristic(find, top, bad);

                int count = 0;
                while (findl2 < main.Length + 1)
                {
                    if (find[findl - 1] == main[findl2 - 1])               //сравниваем правый символ подстроки с символом строки
                    {
                        findl--; findl2--;              //двигаемся справа налево
                        count++;
                        if (findl == 0)                //проверяем, прошли ли мы все символы подстроки и наличие индекса начала подстроки в строке в массиве индексов
                        {
                            res[index] = findl2;
                            index++;
                            findl = find.Length;
                            findl2 += findl2 - findl + 1;
                            count = 0;

                        }
                    }
                    else if (findl != find.Length)               //для использования суффикса проверяем, что мы посмотрели больше 1 символа строки
                    {
                        findl2 = find.Length + findl2 - findl + tap[find.Length - count];               //так как таблица перевёрнута, возврат на начальный индекс символа строки и потом добавление по словарю
                        count = 0;
                        findl = find.Length;
                    }
                    else
                    {
                        if (bad.Contains(main[findl2 - 1]) == false)             //проверяем есть ли символ строки в массиве символов
                        {
                            findl2 = find.Length + findl2 - findl + top[find.Length - 1 - count];           //так как таблица перевёрнута, возврат на начальный индекс символа строки и потом добавление по словарю  
                            count = 0;
                            findl = find.Length;
                        }
                        else
                        {
                            findl2 += 1;
                            findl = find.Length;
                        }
                    }
                }
                DateTime t2 = DateTime.Now;
                Print(res);
                Console.WriteLine($"Время выполнения: {t2.Subtract(t1).TotalSeconds} c");
                return res;
            }

        }
        private static void GoodCharHeuristic(string str, int[] tap, string[] good)         //таблица хороших суффиксов
        {
            int length = str.Length;

            for (int j = 0; j < good.Length; j++)                   //разбитие на подстроки
            {
                String Substring = str.Substring(j, length - j);
                good[j] = Substring;
                tap[j] = length;
            }
            string st = str.Remove(length - 1);
            for (int k = 0; k < length; k++)
            {
                if (st.Contains(good[k]) == true)
                {
                    int index2 = st.IndexOf(good[k]);
                    string str2 = str;
                    str2 = str2.Remove(index2, good[k].Length);
                    int index1 = str2.IndexOf(good[k]);
                    tap[k] = index1 - index2 + 1 + good[k].Length - 1;
                }
            }
        }
        private static void BadCharHeuristic(string str, int[] top, char[] bad)             //таблица стоп символов
        {
            int length = str.Length;
            int ind = 0;

            foreach (char s in str)          //заполнение таблицы символов
            {
                bad[ind] = s;
                top[ind] = length;          //величина сдвига при несовпавшем символе равна размеру
                ind++;
            }
            for (int i = 0; i < length; i++)             //поиск совпадающих символов
                for (int j = 0; j < length; j++)
                {
                    if (i != j && bad[i] == bad[j])
                    {
                        top[i] = Math.Abs(i - j);               //изменение величины сдвига если нашлись совпадающие символы
                    }
                }
        }
    }
}
