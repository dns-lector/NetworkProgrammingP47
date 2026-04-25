using NetworkProgrammingP47.Dal;
using NetworkProgrammingP47.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NetworkProgrammingP47
{
    internal class UserService
    {
        private DataAccessor dataAccessor;
        public void Run()
        {
            try { dataAccessor = new(); }
            catch { return; }

            while (true)
            {
                Console.WriteLine(
                    "Сервіс роботи з користувачами:\n" +
                    "1: реєстрація\n" +
                    "2: автентифікація (вхід)\n" +
                    "3: забув пароль\n" +
                    "i: інсталювати таблиці БД\n" +
                    "0: вихід"
                );
                var keyInfo = Console.ReadKey();
                Console.WriteLine();
                switch (keyInfo.KeyChar)
                {
                    case '0': return;
                    case '1': SignUp();  break;
                    case '2': Console.WriteLine( OtpService.ConfirmCode() );  break;
                    case '3': Console.WriteLine( OtpService.TempPassword() );  break;
                    case 'i': try { dataAccessor.InstallTables(); } catch { return; }  break;

                    default: Console.WriteLine("Вибір не розпізнано\n"); break;
                }
            }
            
        }

        private void SignUp()
        {
            Console.WriteLine("Реєстрація нового користувача");
            String email = "";
            while (true)
            {
                Console.Write("Введіть E-mail: ");
                email = Console.ReadLine()!;
                // перевіряємо пошту на зовнішній формат (валідація)
                if(Regex.IsMatch(email, @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("E-mail не відповідає формату, відкоригуйте");
                }
            }
            Console.Write("Створіть пароль: ");
            String password = "";
            while (true)
            {
                password = Console.ReadLine()!;
                if (password.Length >= 6 && true)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Пароль має бути щонайменше 6 символів, " +
                        "серед яких має бути цифра, літера та спецсимвол");
                }
            }
            Console.Write("Як до вас звертатись? ");
            String name = Console.ReadLine()!;

            String confirmCode = OtpService.ConfirmCode();
            try
            {
                dataAccessor.AddUser(new()
                {
                    Name = name,
                    Email = email,
                    ConfirmCode = confirmCode,
                    Password = password
                });
            }
            catch { return; }
            EmailService.SendConfirmCode(email, confirmCode);

            // Console.Write("Введіть код, надісланий на пошту: ");
            // String code = Console.ReadLine()!;
            Console.WriteLine("Ви успішно зареєстровані. Використовуйте пошту та пароль для входу");
        }
    }
}
/* Д.З. Реалізувати у DataAccessor метод для перевірки електронної
 * пошти - чи є така вже у БД
 * bool IsEmailUsed(String email)
 * При введені користувачем пошти при реєстрації додати перевірку
 * на зайнятість і повторювати введення за таких умов
 * 
 */
/* Д.З. Забезпечити валідацію паролю, що вводить користувач при реєстрації
 * Пароль має бути щонайменше 6 символів,
 * серед яких має бути цифра, літера та спецсимвол
 * ** літери мають бути різного реєстру (як великі, так і маленькі)
 */
/* Сервіс роботи з користувачами:
 * реєстрація
 * перевірка пошти
 * забув пароль
 */