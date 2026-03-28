using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkProgrammingP47
{
    internal class HttpDemo
    {
        private Stopwatch stopwatch = new();

        public async Task RunAsync()
        {
            Console.WriteLine("Http Demo");
            String url;
            try { url = GetAndValidateUrl(); }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            stopwatch.Start();
            // інструмент надсилання запитів - HttpClient
            HttpClient httpClient = new();
            String data = await httpClient.GetStringAsync(url);
            long ms = stopwatch.ElapsedMilliseconds;
            Console.WriteLine(data);
            Console.WriteLine("------------------------");
            Console.WriteLine($"Elapsed {ms} Milliseconds for loading, {stopwatch.ElapsedMilliseconds} total");
        }

        private String GetAndValidateUrl()
        {
            Console.Write("Введіть URL-адресу: ");
            String url = Console.ReadLine()!;
            int index = url.IndexOf("://");   //  роздільні символи для схеми запиту
            if(index == -1)
            {
                throw new FormatException("Введений URL повинен містити схему запиту");
            }
            String scheme = url[..index];
            if(scheme != "http" &&  scheme != "https")
            {
                throw new FormatException($"Cхема запиту '{scheme}' не підтримується: тільки http або https");
            }
            Console.WriteLine($"Схема запиту: '{scheme}'");
            /* Традиційно, адреси без параметрів (без query) мають завершуватись 
             * символом '/': https://itstep.org/
             * Але також традиційно адреси без слешів приймаються без зауважень: https://itstep.org
             * Відповідно, границею доменного імені є або перший після схеми '/', або кінець рядка
             * Задача: визначити доменне ім'я, відокремивши частину адреси від кінця схеми до 
             * першого слешу або кінця рядка, якщо слешів в залишку адреси немає.
             * https://od.itstep.org/contacts  -> od.itstep.org
             *        |             |
             *    кінець схеми   перший '/'
             *    
             * https://itstep.org    -> itstep.org
             *        |          |
             *    кінець схеми   кінець рядка
             *    
             */
            return "https://itstep.org/";
        }
    }
}
/* HTTP протокол прикладного рівня для обміну даними в Інтернет
 * Запит (Request) - дані, які надсилає клієнт, починаючи обмін
 *  Першою характеристикою запиту є його метод. Стандартизовано 9 методів,
 *  з яких 5 - загального призначення (4 - службового)
 *  метод          семантика
 *  GET             Read - одержання даних
 *  POST            Create - створення (додавання) нових даних
 *  PATCH           Update - часткове оновлення наявних даних
 *  PUT             Replace - повна заміна наявних даних
 *  DELETE          Delete - видалення даних
 *  
 * Запити та відповіді є відносно тривалими процесами, тому 
 * виконуються в асинхронному режимі.
 */
