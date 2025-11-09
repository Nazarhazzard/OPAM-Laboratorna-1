using System;

namespace SportsClubMenuApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Спортивний клуб — головне меню";
            ShowMainMenu();
        }
        static void ShowMainMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== ГОЛОВНЕ МЕНЮ СПОРТИВНОГО КЛУБУ ===");
            Console.ResetColor();

            Console.WriteLine("1. Розрахунок абонементів (з ЛР №1)");
            Console.WriteLine("2. Переглянути інформацію про клуб");
            Console.WriteLine("3. Налаштування");
            Console.WriteLine("4. Контактна інформація");
            Console.WriteLine("0. Вихід");

            Console.Write("\nВаш вибір: ");

            try
            {
                int choice = int.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        CalculateMemberships();
                        break;
                    case 2:
                        ShowClubInfo();
                        break;
                    case 3:
                        Settings();
                        break;
                    case 4:
                        ShowContacts();
                        break;
                    case 0:
                        Console.WriteLine("\nДякуємо! Програма завершена.");
                        return;
                    default:
                        Console.WriteLine("\nНевірний вибір! Спробуйте ще раз.");
                        break;
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("\nПомилка: введіть номер пункту меню!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nНепередбачена помилка: {ex.Message}");
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу, щоб повернутися в меню...");
            Console.ReadKey();
            ShowMainMenu();
        }
        static void CalculateMemberships()
        {
            Console.Clear();
            Console.Title = "Розрахунок абонементів";

            double monthPrice = 800;
            double halfYearPrice = 4200;
            double yearPrice = 7500;

            Console.WriteLine("=== Розрахунок абонементів спортивного клубу ===");

            try
            {
                Console.Write("\nВведіть загальну кількість членів клубу: ");
                int totalMembers = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("\nТипи абонементів:");
                Console.WriteLine($"1. Місячний — {monthPrice} грн");
                Console.WriteLine($"2. Піврічний — {halfYearPrice} грн");
                Console.WriteLine($"3. Річний — {yearPrice} грн");

                Console.Write("\nВведіть кількість місячних абонементів: ");
                int monthCount = Convert.ToInt32(Console.ReadLine());

                Console.Write("Введіть кількість піврічних абонементів: ");
                int halfYearCount = Convert.ToInt32(Console.ReadLine());

                Console.Write("Введіть кількість річних абонементів: ");
                int yearCount = Convert.ToInt32(Console.ReadLine());

                double totalMonth = monthCount * monthPrice;
                double totalHalf = halfYearCount * halfYearPrice;
                double totalYear = yearCount * yearPrice;
                double totalSum = totalMonth + totalHalf + totalYear;

                Random rand = new Random();
                double discountPercent = rand.Next(5, 16); // 5–15%
                double discountAmount = totalSum * discountPercent / 100;
                double finalSum = totalSum - discountAmount;

                int totalSubscribers = monthCount + halfYearCount + yearCount;
                double percentWithAbonement = (double)totalSubscribers / totalMembers * 100;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n--- РЕЗУЛЬТАТИ РОЗРАХУНКУ ---");
                Console.ResetColor();

                Console.WriteLine($"Місячні: {monthCount} × {monthPrice} = {totalMonth} грн");
                Console.WriteLine($"Піврічні: {halfYearCount} × {halfYearPrice} = {totalHalf} грн");
                Console.WriteLine($"Річні: {yearCount} × {yearPrice} = {totalYear} грн");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nЗагальна сума: {Math.Round(totalSum, 2)} грн");
                Console.WriteLine($"Знижка: {discountPercent}% (-{Math.Round(discountAmount, 2)} грн)");
                Console.WriteLine($"До оплати: {Math.Round(finalSum, 2)} грн");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"\nКвадратний корінь із загальної суми: {Math.Round(Math.Sqrt(totalSum), 2)}");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\nУ клубі всього {totalMembers} членів.");
                Console.WriteLine($"Абонементи мають {totalSubscribers} осіб ({Math.Round(percentWithAbonement, 2)}%).");
                Console.ResetColor();
            }
            catch (FormatException)
            {
                Console.WriteLine("\nПомилка: введено некоректне число!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nПомилка: {ex.Message}");
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу, щоб повернутися в головне меню...");
            Console.ReadKey();
        }
        static void ShowClubInfo()
        {
            Console.Clear();
            Console.WriteLine("=== ІНФОРМАЦІЯ ПРО КЛУБ ===");
            Console.WriteLine("Назва: OLIMP FITNES CENTER");
            Console.WriteLine("Ми пропонуємо сучасні тренажери, групові заняття та персональні тренування.");
            Console.WriteLine("Клуб працює щодня з 9:00 до 21:00.");
            Console.WriteLine("\nНатисніть будь-яку клавішу, щоб повернутися...");
            Console.ReadKey();
        }
        static void Settings()
        {
            Console.Clear();
            Console.WriteLine("=== НАЛАШТУВАННЯ ===");
            Console.WriteLine("Функція в розробці...");
            Console.WriteLine("\nНатисніть будь-яку клавішу, щоб повернутися...");
            Console.ReadKey();
        }
        static void ShowContacts()
        {
            Console.Clear();
            Console.WriteLine("=== КОНТАКТНА ІНФОРМАЦІЯ ===");
            Console.WriteLine("Адреса: м. Виноградів, вул. Тараса Шевченка, 13");
            Console.WriteLine("Телефон: +380 (99) 030-41-31");
            Console.WriteLine("Email: info@vp-grup.ua");
            Console.WriteLine("\nНатисніть будь-яку клавішу, щоб повернутися...");
            Console.ReadKey();
        }
    }
}
