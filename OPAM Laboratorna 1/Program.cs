using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SportsClubMenuApp_Extended
{
    struct Product
    {
        public int Id;
        public string Name;
        public double Price;
        public int Quantity;
        public string Category;

        public Product(int id, string name, double price, int quantity, string category)
        {
            Id = id;
            Name = name;
            Price = price;
            Quantity = quantity;
            Category = category;
        }

        public void Display()
        {
            Console.WriteLine($"ID: {Id} | {Name} | {Category} | Price: {Price:F2} грн | Qty: {Quantity}");
        }
    }

    struct Client
    {
        public int Id;
        public string FullName;
        public string Phone;
        public string Email;

        public Client(int id, string fullName, string phone, string email)
        {
            Id = id;
            FullName = fullName;
            Phone = phone;
            Email = email;
        }
    }

    struct Booking
    {
        public int BookingId;
        public int ClientId;
        public int ProductId;
        public DateTime Date;

        public Booking(int bookingId, int clientId, int productId, DateTime date)
        {
            BookingId = bookingId;
            ClientId = clientId;
            ProductId = productId;
            Date = date;
        }
    }

    class Program
    {
        static List<Product> products = new List<Product>();
        static List<Client> clients = new List<Client>();
        static List<Booking> bookings = new List<Booking>();

        const string ProductsFile = "products.csv";

        const string CorrectLogin = "admin";
        const string CorrectPassword = "1234";

        static void Main(string[] args)
        {
            Console.Title = "Спортивний клуб — розширена програма (Лаб. №3)";

            LoadProductsFromFile();

            if (!LoginSystem())
            {
                Console.WriteLine("\nСпроби вичерпані. Програма завершена.");
                return;
            }

            bool exit = false;
            while (!exit) 
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("=== ГОЛОВНЕ МЕНЮ СПОРТИВНОГО КЛУБУ (Розширене) ===");
                Console.ResetColor();

                Console.WriteLine("1. Ввести/додати товари");
                Console.WriteLine("2. Показати всі товари");
                Console.WriteLine("3. Статистика по товарах");
                Console.WriteLine("4. Пошук товару");
                Console.WriteLine("5. Зберегти товари у файл");
                Console.WriteLine("6. Завантажити товари з файлу");
                Console.WriteLine("7. Вивести звіт");
                Console.WriteLine("0. Вихід");

                Console.Write("\nВаш вибір: ");
                string input = Console.ReadLine();

                try
                {
                    int choice = int.Parse(input);
                    switch (choice)
                    {
                        case 1:
                            AddProductsInteractive();
                            break;
                        case 2:
                            ListAllProducts();
                            break;
                        case 3:
                            ShowStatistics();
                            break;
                        case 4:
                            SearchProductByName();
                            break;
                        case 5:
                            SaveProductsToFile();
                            break;
                        case 6:
                            LoadProductsFromFile();
                            Console.WriteLine("\nЗавантаження завершено.");
                            Pause();
                            break;
                        case 7:
                            PrintReport();
                            break;
                        case 0:
                            Console.WriteLine("\nДякуємо! Програма завершена.");
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("\nНевірний вибір! Спробуйте ще раз.");
                            Pause();
                            break;
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("\nПомилка: введіть номер пункту меню!");
                    Pause();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nНепередбачена помилка: {ex.Message}");
                    Pause();
                }
            }
        }

        static bool LoginSystem()
        {
            int attempts = 0;
            const int maxAttempts = 3;
            string login, password;

            do
            {
                Console.Clear();
                Console.WriteLine("=== ВХІД ДО СИСТЕМИ ===");
                Console.Write("Логін: ");
                login = Console.ReadLine();
                Console.Write("Пароль: ");
                password = ReadPasswordMasked();

                if (login == CorrectLogin && password == CorrectPassword)
                {
                    Console.WriteLine("\nВхід успішний. Ласкаво просимо!");
                    Pause();
                    return true;
                }
                else
                {
                    attempts++;
                    Console.WriteLine($"\nНевірні дані. Залишилось спроб: {maxAttempts - attempts}");
                    if (attempts < maxAttempts)
                        Pause();
                }
            } while (attempts < maxAttempts);

            return false;
        }

        static string ReadPasswordMasked()
        {
            string pass = "";
            ConsoleKeyInfo key;
            while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    pass = pass.Substring(0, pass.Length - 1);
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
            }
            Console.WriteLine();
            return pass;
        }

        static void AddProductsInteractive()
        {
            Console.Clear();
            Console.WriteLine("=== ДОДАТИ ТОВАРИ ===");
            int numberToAdd = 0;

            try
            {
                Console.Write("Скільки товарів додати (мінімум 1): ");
                numberToAdd = int.Parse(Console.ReadLine());
                if (numberToAdd <= 0)
                {
                    Console.WriteLine("Кількість має бути > 0.");
                    Pause();
                    return;
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Некоректне число.");
                Pause();
                return;
            }

            int nextId = products.Count > 0 ? products[products.Count - 1].Id + 1 : 1;

            for (int i = 0; i < numberToAdd; i++)
            {
                Console.WriteLine($"\nВведення товару #{i + 1}:");
                string name;
                double price = 0;
                int qty = 0;
                string category;

                Console.Write("Назва: ");
                name = Console.ReadLine();

                while (true)
                {
                    Console.Write("Ціна (грн): ");
                    string s = Console.ReadLine();
                    if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out price))
                    {
                        if (price <= 0)
                        {
                            Console.WriteLine("Ціна має бути більше 0 — пропускаємо цей товар (continue).");
                          
                            goto SkipThisProduct;
                        }
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Некоректне число, спробуйте ще раз.");
                    }
                }

                while (true)
                {
                    Console.Write("Кількість: ");
                    string s = Console.ReadLine();
                    if (int.TryParse(s, out qty))
                    {
                        if (qty < 0)
                        {
                            Console.WriteLine("Кількість не може бути від'ємною. Введіть ще раз.");
                            continue; 
                        }
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Некоректне число. Спробуйте ще раз.");
                    }
                }

                Console.Write("Категорія: ");
                category = Console.ReadLine();

                products.Add(new Product(nextId++, name, price, qty, category));
                Console.WriteLine("Товар додано.");

            SkipThisProduct:
                ;
            }

            Pause();
        }

        static void ListAllProducts()
        {
            Console.Clear();
            Console.WriteLine("=== СПИСОК ТОВАРІВ ===");
            if (products.Count == 0)
                Console.WriteLine("Товарів немає.");
            else
            {
                foreach (var p in products)
                {
                    p.Display();
                }
            }
            Pause();
        }

     
        static void ShowStatistics()
        {
            Console.Clear();
            Console.WriteLine("=== СТАТИСТИКА ПО ТОВАРАХ ===");

            if (products.Count == 0)
            {
                Console.WriteLine("Немає даних.");
                Pause();
                return;
            }

            double totalValue = 0;
            double sumPrices = 0;
            int countValidPrices = 0;
            int countPriceMoreThan500 = 0;
            double minPrice = double.MaxValue;
            double maxPrice = double.MinValue;

          
            for (int i = 0; i < products.Count; i++)
            {
                var p = products[i];

               
                if (p.Price <= 0)
                    continue;

                totalValue += p.Price * p.Quantity;
                sumPrices += p.Price;
                countValidPrices++;

                if (p.Price > 500)
                    countPriceMoreThan500++;

                if (p.Price < minPrice)
                    minPrice = p.Price;

                if (p.Price > maxPrice)
                    maxPrice = p.Price;
            }

            double averagePrice = countValidPrices > 0 ? sumPrices / countValidPrices : 0;

            Console.WriteLine($"Загальна вартість (усі одиниці): {totalValue:F2} грн");
            Console.WriteLine($"Середня ціна товару: {averagePrice:F2} грн");
            Console.WriteLine($"Кількість товарів з ціною > 500 грн: {countPriceMoreThan500}");
            Console.WriteLine($"Мінімальна ціна: {(minPrice == double.MaxValue ? 0 : minPrice):F2} грн");
            Console.WriteLine($"Максимальна ціна: {(maxPrice == double.MinValue ? 0 : maxPrice):F2} грн");

            Pause();
        }

        static void SearchProductByName()
        {
            Console.Clear();
            Console.WriteLine("=== ПОШУК ТОВАРУ ===");
            Console.Write("Введіть назву для пошуку (частина імені): ");
            string q = Console.ReadLine();

            bool found = false;
            for (int i = 0; i < products.Count; i++)
            {
                if (products[i].Name.IndexOf(q, StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    Console.WriteLine("\nЗнайдено перший збіг:");
                    products[i].Display();
                    found = true;
                    break;
                }
            }

            if (!found)
                Console.WriteLine("Схожих товарів не знайдено.");

            Pause();
        }

      
        static void SaveProductsToFile()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(ProductsFile))
                {
                
                    sw.WriteLine("Id;Name;Price;Quantity;Category");
                    foreach (var p in products)
                    {
                        sw.WriteLine($"{p.Id};{EscapeCsv(p.Name)};{p.Price.ToString(CultureInfo.InvariantCulture)};{p.Quantity};{EscapeCsv(p.Category)}");
                    }
                }
                Console.WriteLine($"\nДані збережено у файл: {ProductsFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nПомилка збереження: {ex.Message}");
            }
            Pause();
        }

        static void LoadProductsFromFile()
        {
            products.Clear();
            if (!File.Exists(ProductsFile))
                return;

            try
            {
                using (StreamReader sr = new StreamReader(ProductsFile))
                {
                    string header = sr.ReadLine();
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        var parts = line.Split(';');
                        if (parts.Length < 5)
                            continue;

                        if (!int.TryParse(parts[0], out int id)) continue;
                        string name = UnescapeCsv(parts[1]);
                        if (!double.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double price)) price = 0;
                        if (!int.TryParse(parts[3], out int qty)) qty = 0;
                        string category = UnescapeCsv(parts[4]);

                        products.Add(new Product(id, name, price, qty, category));
                    }
                }
            }
            catch
            {
             
            }
        }

        static string EscapeCsv(string s) => s?.Replace(";", ",") ?? "";
        static string UnescapeCsv(string s) => s?.Replace(",", ";") ?? "";

        static void PrintReport()
        {
            Console.Clear();
            Console.WriteLine("=== ЗВІТ ПО ТОВАРАХ (Форматований) ===");
            Console.WriteLine($"Дата: {DateTime.Now}");
            Console.WriteLine(new string('-', 60));
            Console.WriteLine($"{"ID",3} | {"Назва",-20} | {"Категорія",-12} | {"Ціна",7} | {"Кільк.",6}");
            Console.WriteLine(new string('-', 60));

            foreach (var p in products)
            {
                Console.WriteLine($"{p.Id,3} | {Truncate(p.Name, 20),-20} | {Truncate(p.Category, 12),-12} | {p.Price,7:F2} | {p.Quantity,6}");
            }

            Console.WriteLine(new string('-', 60));
       
            double totalValue = 0;
            int totalQty = 0;
            foreach (var p in products)
            {
                totalValue += p.Price * p.Quantity;
                totalQty += p.Quantity;
            }
            Console.WriteLine($"Загальна кількість товарів (шт): {totalQty}");
            Console.WriteLine($"Загальна вартість: {totalValue:F2} грн");
            Console.WriteLine(new string('-', 60));

            Pause();
        }

        static string Truncate(string s, int maxLen) => s.Length <= maxLen ? s : s.Substring(0, maxLen - 3) + "...";

        static void Pause()
        {
            Console.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
            Console.ReadKey();
        }
    }
}
