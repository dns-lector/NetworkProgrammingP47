using NetworkProgrammingP47.Dal;
using NetworkProgrammingP47.Models;
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
                    "\nСервіс роботи з користувачами:\n" +
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
                    case '2': SignIn();  break;
                    case '3': ForgotPassword();  break;
                    case 'i': try { dataAccessor.InstallTables(); } catch { return; }  break;

                    default: Console.WriteLine("Вибір не розпізнано\n"); break;
                }
            }
            
        }

        private void ForgotPassword()
        {
            // ТЗ переривати процес якщо пошта порожня
            Console.Write("Введіть ваш E-mail: ");
            String email = Console.ReadLine()!;
            Console.Write("Введіть ваше ім'я, вказане при реєстрації: ");
            String name = Console.ReadLine()!;
            String? newPassword;
            try
            {
                newPassword = dataAccessor.ResetPassword(email, name);
            }
            catch
            {
                Console.WriteLine("Виникла помилка, процес зупинено");
                return;
            }
            if (newPassword != null)
            {
                EmailService.SendNewPassword(email, newPassword);
                Console.WriteLine("Sent");
            }
            Console.WriteLine("Якщо ви ввели дані правильно, то на вашу пошту надіслано новий пароль");
        }

        private void SignIn()
        {
            String email;
            Console.Write("Введіть E-mail: ");  // azure.spd111.od.0@ukr.net  Ou\Ai7
            email = Console.ReadLine()!;

            Console.Write("Введіть пароль (символи не будуть зображатись, ESC - повтор): ");
            String? password;
            do
            {
                Console.WriteLine();
                Console.Write("> ");
                password = InputPassword();
            } while (password == null);
            // Console.WriteLine(password);
            Console.WriteLine();
            UserEntity? userEntity = dataAccessor.Authenticate(email, password);
            if(userEntity == null)
            {
                Console.WriteLine("У вході відмовлено");
                return;
            }
            Console.WriteLine($"Вітаємо, {userEntity.Name}");
            // Перевіряємо чи була підтверджена пошта за наявністю коду у БД
            if(userEntity.ConfirmCode != null)
            {
                Console.WriteLine(
                    $"У вас не підтверджена пошта, {userEntity.ConfirmCodeSentAt} " +
                    $"вам на пошту було надіслано код");

                int tries = 3;
                String code;
                while (true)
                {
                    tries -= 1;
                    if(tries < 0)
                    {
                        Console.WriteLine("Кількість спроб вичерпано");
                        return;
                    }
                    Console.Write("Введіть код (Enter - вихід): ");
                    code = Console.ReadLine()!;
                    if(code == "")
                    {
                        Console.WriteLine("Пошта лишається не підтвердженою");
                        return;
                    }
                    if(code == userEntity.ConfirmCode)
                    {
                        // код введено правильно - вносимо дані до БД
                        try { dataAccessor.ConfirmEmail(userEntity); }
                        catch { return; }
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Код не прийнято");
                    }
                }
                
            }
        }

        private String? InputPassword()
        {
            StringBuilder sb = new();
            ConsoleKeyInfo keyInfo;
            while (true)
            {
                keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Escape) return null;
                if (keyInfo.Key == ConsoleKey.Enter) break;
                if (keyInfo.Key == ConsoleKey.Backspace && sb.Length > 0)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
                else
                {
                    sb.Append(keyInfo.KeyChar);
                }
            }
            return sb.ToString();
        }

        private void SignUp()
        {
            Console.WriteLine("Реєстрація нового користувача");
            String email = "";
            // ТЗ переривати процес реєстрації якщо пошта порожня
            while (true)
            {
                Console.Write("Введіть E-mail (порожня - вихід): ");
                email = Console.ReadLine()!;
                if (string.IsNullOrWhiteSpace(email))
                {
                    Console.WriteLine("Реєстрацію скасовано");
                    return;
                }
                // перевіряємо пошту на зовнішній формат (валідація)
                if (Regex.IsMatch(email, @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("E-mail не відповідає формату, відкоригуйте");
                }
            }
            Console.Write("Створіть пароль: ");
            String? password = "";
            // ТЗ замінити введення паролю на відповідну функцію, за умови ESC - скасовувати реєстрацію
            while (true)
            {
                password = InputPassword();
                if (password == null)
                {
                    Console.WriteLine("Реєстрацію скасовано");
                    return;
                }
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
/* Д.З. Реалізувати "авторизований режим":
 * після успішної автентифікації запам'ятовувати її результат
 * та не виводити пп.1-3 меню, залишати лише "вихід"
 * ** замінити ці пункти на "перегляд персональних даних (кабінет)",
 *     "змінити пароль", "редагувати дані"
 */
/* Д.З. Реалізувати надсилання повідомлень про вхід:
 * при кожній новій автентифікації на пошту формується
 * лист з приблизним вмістом: "Зафіксовано новий вхід 
 * з вашим паролем 22.03.2026 18:00:15. Якщо це були 
 * не ви, то радимо змінити пароль"
 */
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