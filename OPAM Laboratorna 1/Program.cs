using System;

namespace SportsClubApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Спортивний клуб — розрахунок абонементів";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== Вітаємо у спортивному клубі! ===");
            Console.ResetColor();

            // Ціни абонементів
            double monthPrice = 800;
            double halfYearPrice = 4200;
            double yearPrice = 7500;

            // Ввід загальної кількості членів
            Console.Write("\nВведіть загальну кількість членів клубу: ");
            int totalMembers = Convert.ToInt32(Console.ReadLine());

            // Меню
            Console.WriteLine("\nТипи абонементів:");
            Console.WriteLine($"1. Місячний — {monthPrice} грн");
            Console.WriteLine($"2. Піврічний — {halfYearPrice} грн");
            Console.WriteLine($"3. Річний — {yearPrice} грн");

            // Ввід кількості абонементів
            Console.Write("\nВведіть кількість місячних абонементів: ");
            int monthCount = Convert.ToInt32(Console.ReadLine());

            Console.Write("Введіть кількість піврічних абонементів: ");
            int halfYearCount = Convert.ToInt32(Console.ReadLine());

            Console.Write("Введіть кількість річних абонементів: ");
            int yearCount = Convert.ToInt32(Console.ReadLine());

            // Розрахунок сум
            double totalMonth = monthCount * monthPrice;
            double totalHalf = halfYearCount * halfYearPrice;
            double totalYear = yearCount * yearPrice;
            double totalSum = totalMonth + totalHalf + totalYear;

            // Генерація випадкової знижки
            Random rand = new Random();
            double discountPercent = rand.Next(5, 16); // від 5 до 15%
            double discountAmount = totalSum * discountPercent / 100;
            double finalSum = totalSum - discountAmount;

            // Розрахунок членів із абонементом
            int totalSubscribers = monthCount + halfYearCount + yearCount;
            double percentWithAbonement = (double)totalSubscribers / totalMembers * 100;

            // Вивід результатів
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

            Console.WriteLine("\nДякуємо за користування програмою!");
        }
    }
}