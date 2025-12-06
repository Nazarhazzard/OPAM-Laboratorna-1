using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SportsClubMenuApp_Lab4
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
            Console.WriteLine($"{Id,3} | {Name,-20} | {Category,-10} | {Price,7:F2} грн | Qty: {Quantity}");
        }
    }

    class Program
    {
        static List<Product> products = new List<Product>();
        const string ProductsFile = "products.csv";

        const string CorrectLogin = "admin";
        const string CorrectPassword = "1234";

        static void Main(string[] args)
        {
            Console.Title = "Спортивний клуб — Лабораторна робота №4";

            LoadProductsFromFile();

            if (!LoginSystem())
            {
                Console.WriteLine("Спроби вичерпані. Завершення програми.");
                return;
            }

            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("=== ГОЛОВНЕ МЕНЮ ===");
                Console.ResetColor();

                Console.WriteLine("1. Додати товари");
                Console.WriteLine("2. Список товарів");
                Console.WriteLine("3. Пошук товару");
                Console.WriteLine("4. Видалити товар");
                Console.WriteLine("5. Сортування товарів");
                Console.WriteLine("6. Статистика");
                Console.WriteLine("7. Власне сортування (бульбашкою)");
                Console.WriteLine("8. Зберегти товари у файл");
                Console.WriteLine("9. Завантажити товари з файлу");
                Console.WriteLine("0. Вихід");

                Console.Write("\nВиберіть пункт: ");
                string input = Console.ReadLine();

                try
                {
                    int choice = int.Parse(input);
                    switch (choice)
                    {
                        case 1: AddProducts(); break;
                        case 2: ListProductsTable(); break;
                        case 3: SearchProduct(); break;
                        case 4: DeleteProduct(); break;
                        case 5: SortProducts(); break;
                        case 6: ShowStatistics(); break;
                        case 7: BubbleSortProducts(); break;
                        case 8: SaveProductsToFile(); break;
                        case 9: LoadProductsFromFile(); Pause("Завантажено."); break;
                        case 0:
                            exit = true;
                            Console.WriteLine("До побачення!");
                            break;
                        default: Pause("Невірний вибір."); break;
                    }
                }
                catch
                {
                    Pause("Помилка вводу!");
                }
            }
        }

        static bool LoginSystem()
        {
            int attempts = 0;
            while (attempts < 3)
            {
                Console.Clear();
                Console.WriteLine("=== Вхід до системи ===");

                Console.Write("Логін: ");
                string login = Console.ReadLine();

                Console.Write("Пароль: ");
                string pass = Console.ReadLine();

                if (login == CorrectLogin && pass == CorrectPassword)
                {
                    Pause("Успішний вхід!");
                    return true;
                }

                attempts++;
                Pause($"Невірно. Спроб залишилось: {3 - attempts}");
            }
            return false;
        }

        static void AddProducts()
        {
            Console.Clear();
            Console.WriteLine("=== Додавання товарів ===");

            Console.Write("Скільки товарів додати? ");
            if (!int.TryParse(Console.ReadLine(), out int count) || count <= 0)
            {
                Pause("Некоректне число.");
                return;
            }

            int nextId = products.Count > 0 ? products[products.Count - 1].Id + 1 : 1;

            for (int i = 0; i < count; i++)
            {
                Console.WriteLine($"\nТовар #{i + 1}:");

                Console.Write("Назва: ");
                string name = Console.ReadLine();

                Console.Write("Категорія: ");
                string cat = Console.ReadLine();

                Console.Write("Ціна: ");
                if (!double.TryParse(Console.ReadLine(), out double price) || price <= 0)
                {
                    Console.WriteLine("Ціна некоректна. Пропуск.");
                    continue;
                }

                Console.Write("Кількість: ");
                if (!int.TryParse(Console.ReadLine(), out int qty) || qty < 0)
                {
                    Console.WriteLine("Кількість некоректна. Пропуск.");
                    continue;
                }

                products.Add(new Product(nextId++, name, price, qty, cat));
                Console.WriteLine("Додано.");
            }

            Pause();
        }

        static void ListProductsTable()
        {
            Console.Clear();
            Console.WriteLine("=== СПИСОК ТОВАРІВ ===");

            if (products.Count == 0)
            {
                Pause("Список порожній.");
                return;
            }

            Console.WriteLine($"{"ID",3} | {"Назва",-20} | {"Категорія",-10} | {"Ціна",7} | Qty");
            Console.WriteLine(new string('-', 55));

            foreach (var p in products)
                p.Display();

            Pause();
        }

        static void SearchProduct()
        {
            Console.Clear();
            Console.WriteLine("=== Пошук товару ===");
            Console.Write("Введіть назву або частину: ");

            string query = Console.ReadLine();
            bool found = false;

            foreach (var p in products)
            {
                if (p.Name.ToLower().Contains(query.ToLower()))
                {
                    Console.WriteLine("Знайдено:");
                    p.Display();
                    found = true;
                    break;
                }
            }

            if (!found) Console.WriteLine("Не знайдено.");
            Pause();
        }

        static void DeleteProduct()
        {
            Console.Clear();
            Console.WriteLine("=== Видалення товару ===");

            Console.Write("Введіть ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Pause("Помилка.");
                return;
            }

            int index = -1;
            for (int i = 0; i < products.Count; i++)
                if (products[i].Id == id) index = i;

            if (index == -1)
            {
                Pause("Товар не знайдено.");
                return;
            }

            products.RemoveAt(index);
            Pause("Видалено.");
        }

        static void SortProducts()
        {
            Console.Clear();
            Console.WriteLine("=== Сортування ===");
            Console.WriteLine("1. За назвою");
            Console.WriteLine("2. За ціною");
            Console.WriteLine("3. За кількістю");

            Console.Write("Ваш вибір: ");
            int ch = int.Parse(Console.ReadLine());

            products.Sort((a, b) =>
            {
                switch (ch)
                {
                    case 1: return a.Name.CompareTo(b.Name);
                    case 2: return a.Price.CompareTo(b.Price);
                    case 3: return a.Quantity.CompareTo(b.Quantity);
                }
                return 0;
            });

            Pause("Відсортовано.");
        }

        static void BubbleSortProducts()
        {
            Console.Clear();
            Console.WriteLine("=== Власне сортування: Бульбашка по ціні ===");

            for (int i = 0; i < products.Count - 1; i++)
                for (int j = 0; j < products.Count - i - 1; j++)
                    if (products[j].Price > products[j + 1].Price)
                    {
                        var temp = products[j];
                        products[j] = products[j + 1];
                        products[j + 1] = temp;
                    }

            Pause("Сортування завершено.");
        }

        static void ShowStatistics()
        {
            Console.Clear();
            Console.WriteLine("=== Статистика ===");

            if (products.Count == 0)
            {
                Pause("Порожньо.");
                return;
            }

            double sum = 0;
            double min = double.MaxValue;
            double max = double.MinValue;
            int totalQty = 0;

            foreach (var p in products)
            {
                sum += p.Price;
                totalQty += p.Quantity;
                if (p.Price < min) min = p.Price;
                if (p.Price > max) max = p.Price;
            }

            Console.WriteLine($"Кількість товарів: {products.Count}");
            Console.WriteLine($"Середня ціна: {sum / products.Count:F2}");
            Console.WriteLine($"Мінімальна ціна: {min:F2}");
            Console.WriteLine($"Максимальна ціна: {max:F2}");
            Console.WriteLine($"Загальна кількість одиниць: {totalQty}");

            Pause();
        }

        static void SaveProductsToFile()
        {
            using (StreamWriter sw = new StreamWriter(ProductsFile))
            {
                sw.WriteLine("Id;Name;Price;Quantity;Category");
                foreach (var p in products)
                    sw.WriteLine($"{p.Id};{p.Name};{p.Price};{p.Quantity};{p.Category}");
            }
            Pause("Збережено.");
        }

        static void LoadProductsFromFile()
        {
            products.Clear();
            if (!File.Exists(ProductsFile)) return;

            using (StreamReader sr = new StreamReader(ProductsFile))
            {
                sr.ReadLine();
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var c = line.Split(';');
                    products.Add(new Product(
                        int.Parse(c[0]),
                        c[1],
                        double.Parse(c[2]),
                        int.Parse(c[3]),
                        c[4]
                    ));
                }
            }
        }

        static void Pause(string msg = "Натисніть клавішу...")
        {
            Console.WriteLine("\n" + msg);
            Console.ReadKey();
        }
    }
}