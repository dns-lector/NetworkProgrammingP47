using NetworkProgrammingP47.Orm.Nbu;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace NetworkProgrammingP47
{
    internal class ApiDemo
    {
        private Exchange exchange;
        public void Run()
        {
            Console.WriteLine("Курси валют НБУ");
            DemoXmlOrm();   // у коді є завантаження exchange
            Console.WriteLine($"Завантажено {exchange.Currencies.Count} курсів");
            while(true)
            {                
                Console.Write("Введіть фрагмент назви валюти: ");
                String? fragment = Console.ReadLine();
                if (String.IsNullOrEmpty(fragment)) break;
                var query = exchange.Currencies.Where(c => c.ShortName.Contains(fragment));
                Console.WriteLine($"Знайдено {query.Count()} результатів");
                foreach(var c in query)
                {
                    Console.WriteLine(c);
                }
            }            
            // Реалізувати пошук без урахування розміру літер: USD = usd
            // Додати до пошуку збіги за іменем (також без урахування розміру літер)
        }
        /* Д.З. Реалізувати завантаження курсів валют на задану дату
         * https://bank.gov.ua/ua/open-data/api-dev
         * https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?date=20200302&json
         * - Користувачу пропонується ввести дату
         * - Здійснюється перевірка дати на валідність
         * - Перевіряється, чи дата належить минулому часу
         * - Виконується запит, переходить в режим пошуку (в меню вибору)
         * ** Після 16:00, а також у вихідні дні встановлюється курс на 
         *     найближчий робочий день. Врахувати це в умовах перевірки дати,
         *     що її вводить користувач.
         */

        private void DemoJsonOrm()
        {
            String url = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json";
            using HttpClient httpClient = new();
            String body = httpClient.GetStringAsync(url).Result;
            List<NbuRate> rates = JsonSerializer.Deserialize<List<NbuRate>>(body)!;
            foreach (NbuRate rate in rates.OrderBy(r => r.Rate))
            {
                Console.WriteLine(rate);
            }
        }

        private void DemoJson()
        {
            String url = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json";
            using HttpClient httpClient = new();
            String body = httpClient.GetStringAsync(url).Result;
            // Десеріалізація:
            // є два види - за структурою JSON, та об'єктна-типізована (ORM)
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

        private void DemoXmlOrm()
        {
            String url = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange";
            using HttpClient httpClient = new();
            Stream bodyStream = httpClient.GetStreamAsync(url).Result;
            XmlSerializer serializer = new(typeof(Exchange));
            exchange = (Exchange)serializer.Deserialize(bodyStream)!;
            // foreach(var currency in exchange.Currencies)
            // {
            //     Console.WriteLine(currency);
            // }
        }

        private void DemoXml()
        {
            String url = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange";
            using HttpClient httpClient = new();
            String body = httpClient.GetStringAsync(url).Result;
            // Console.WriteLine(body);
            XDocument xmlDocument = XDocument.Parse(body);
            foreach(var currency in xmlDocument.Root!.Descendants("currency"))
            {
                String cc = currency.Element("cc")!.Value;
                String text = currency.Element("txt")!.Value;
                Double rate = Double.Parse(
                    currency.Element("rate")!.Value,
                    CultureInfo.InvariantCulture      // щоб сприймав десятичку точку замість коми
                );
                Console.WriteLine($"{cc}  {text} {rate:F2}");
            }
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

/* ORM - Object Relation Mapping
 * "відображення" даних з їх зв'язками на об'єкти та їх зв'язки -
 * трансформація вихідних даних на структури, характерні для мови
 * програмування
 * 1. Описуємо тип даних (клас, структуру, запис тощо), за потреби
 *     зазначаємо співвідношення імен даних різного представлення
 * 2. Використовуємо інструментарій для перетворення переданих 
 *     даних (XML, JSON тощо) до об'єктів та їх колекцій
 * 3. У програмі використовуємо типізовані об'єкти замість 
 *     узагальнених елементів
 */
