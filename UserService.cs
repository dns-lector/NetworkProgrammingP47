using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkProgrammingP47
{
    internal class UserService
    {
        public void Run()
        {
            while (true)
            {
                Console.WriteLine(
                    "Сервіс роботи з користувачами:\n" +
                    "1: реєстрація\n" +
                    "2: автентифікація (вхід)\n" +
                    "3: забув пароль\n" +
                    "0: вихід"
                );
                var keyInfo = Console.ReadKey();
                Console.WriteLine();
                switch (keyInfo.KeyChar)
                {
                    case '0': return;
                    case '1': SignUp();  break;

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
            } 
        }
    }
}
/* Сервіс роботи з користувачами:
 * реєстрація
 * перевірка пошти
 * забув пароль
 */