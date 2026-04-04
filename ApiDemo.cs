using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NetworkProgrammingP47
{
    internal class ApiDemo
    {
        public void Run()
        {
            Console.WriteLine("Курси валют НБУ");
            DemoJson();
        }

        private void DemoJson()
        {
            String url = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json";
            using HttpClient httpClient = new();
            String body = httpClient.GetStringAsync(url).Result;
            // Десеріалізація:
            // є два види - за структурою JSON, та об'єктна-типізована
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(body);
            if (jsonElement.ValueKind == JsonValueKind.Array)
            {
                Console.WriteLine("Одержано {0} записів", jsonElement.GetArrayLength());
                foreach(var rate in jsonElement.EnumerateArray())
                {
                    // Console.WriteLine( String.Join(", ",
                    //     rate.EnumerateObject()
                    //     .Select(p => $"{p.Name}: {p.Value}")
                    // ));
                    String name = rate.GetProperty("txt").GetString()!;
                    String abbr = rate.GetProperty("cc").GetString()!;
                    double course = rate.GetProperty("rate").GetDouble();
                    Console.WriteLine($"{abbr} ({name}) {course:F2}");
                    // вивести наступні відомості: Долар США: 1 USD = 42 HRN, 1 HRN = 0.024 USD
                    // Д.З. Забезпечити збереження одержаних курсів валют та вивести 
                    // користувачеві меню:
                    // 1: вивести за збільшенням курсу
                    // 2: вивести за зменшенням курсу.
                    // 0: вихід
                    // Реалізувати відповідні режими виведення 
                }
            }
            else
            {
                Console.WriteLine("Не очікувано! JSON має тип: {0}", jsonElement.ValueKind);
            }

            // Console.WriteLine(body);
        }

        private void DemoXml()
        {
            String url = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange";
            using HttpClient httpClient = new();
            String body = httpClient.GetStringAsync(url).Result;
            Console.WriteLine(body);
        }
    }
}
/* API Application Program Interface
 * Інтерфейс взаємодії між програмою та застосунками.
 * 
 * Програма - "центральна" частина комплексу, зазвичай, з 
 *   спільною БД. Повністю самостійна, залежить тільки від ОС
 *   (бекенд)
 * Застосунок - відокремлена частина комплексу, зазвичай,
 *   призначена для роботи з користувачами. Відносно самостійна,
 *   може працювати без Програми, але з дуже обмеженою
 *   функціональністю
 *   (фронтенд)
 * Додаток - відокремлений модуль, що розширює функціональність
 *   програми або застосунку. Повністю залежить від них
 *   (plugin, addon, розширення)
 *   
 *   
 *                     Програма (Backend)  --------- Зовнішні програми
 *                 /          |            \
 *              API - Application Program Interface
 *               /            |              \  
 *    Мобільний         Браузерний              Десктоп
 *    застосунок       (веб) застосунок         застосунок
 *   
 * 
 * 
 * Серіалізація - подання об'єктів у послідовному (serial) вигляді
 *                    
 *                  User  
 * User {           [*name, *surname]                ->{"name":"Користувач","surname":"Прізвище"}
 *  Name              \             \...Прізвище
 *  Surname          ...Користувач
 * }
 * 
 * 
 * Рядкові (текстові) формати серіалізації
 * Найбільш поширені - XML та JSON
 * 
 * XML (eXtendable Markup Language) - розміткова мова схожа на HTML
 * <?xml version="1.0" encoding="utf-8"?>  -- перший рядок
 * <root>  -- кореневий елемент - XML складається тільки з одного елементу
 *     <item id="123">   дочірній (вкладений) елемент та атрибут id
 *        value          тіло (контент, значення) елементу     
 *     </item>
 *     <item id="123" value="3254" />   -- самозакритий тег
 * </root>
 * 
 * JSON - JavaScript Object Notation - запис об'єктів мовою JavaScript
 * елемент :
 *  - примітив :
 *    = рядок: завжди у подвійних лапках: "The String", спецсимволи екрануються: \t\n\"
 *    = число: без лапок, може бути дробовим або інженерним
 *    = true/false
 *    = null
 *  - масив: [елемент,елемент,елемент] 
 *  - об'єкт: {"key1": елемент, "key2": елемент}
 *     (назви ключів також у подвійних лапках)
 *     
[
  {
    "r030": 12,
    "txt": "Алжирський динар",
    "rate": 0.32842,
    "cc": "DZD",
    "exchangedate": "06.04.2026",
    "special": null
  },   
...
 */
