using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SportsClubLab5
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
            Console.WriteLine($"{Id,3} | {Name,-20} | {Category,-12} | {Price,9:F2} грн | Qty: {Quantity}");
        }

        public string ToCsvLine()
        {
            return $"{Id};{Escape(Name)};{Price.ToString(CultureInfo.InvariantCulture)};{Quantity};{Escape(Category)}";
        }

        static string Escape(string s)
        {
            if (s == null) return "";
            return s.Replace(";", ",").Replace("\n", " ").Replace("\r", " ");
        }
    }

    struct User
    {
        public int Id;
        public string Email;
        public string PasswordHash; //SHA256

        public User(int id, string email, string passwordHash)
        {
            Id = id;
            Email = email;
            PasswordHash = passwordHash;
        }

        public string ToCsvLine()
        {
            return $"{Id};{Email};{PasswordHash}";
        }
    }

    static class CsvFileHelper
    {
        public static IEnumerable<string[]> ReadCsv(string path, string expectedHeader, char delimiter = ';')
        {
            if (!File.Exists(path))
                yield break;

            string[] allLines = File.ReadAllLines(path);
            if (allLines.Length == 0)
                yield break;

            if (!string.Equals(allLines[0].Trim(), expectedHeader.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                yield break;
            }

            for (int i = 1; i < allLines.Length; i++)
            {
                string line = allLines[i].Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = line.Split(delimiter);
                yield return parts;
            }
        }

        public static void EnsureFileWithHeader(string path, string header)
        {
            if (!File.Exists(path))
            {
                File.WriteAllText(path, header + Environment.NewLine);
            }
            else
            {
                var lines = File.ReadAllLines(path);
                if (lines.Length == 0 || !string.Equals(lines[0].Trim(), header.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    var rest = lines.Skip(1).Where(l => !string.IsNullOrWhiteSpace(l));
                    File.WriteAllText(path, header + Environment.NewLine + string.Join(Environment.NewLine, rest));
                }
            }
        }

        public static int GenerateNewId(string path, int idColumnIndex = 0, char delimiter = ';')
        {
            if (!File.Exists(path)) return 1;

            int max = 0;
            var lines = File.ReadAllLines(path).Skip(1);
            foreach (var raw in lines)
            {
                if (string.IsNullOrWhiteSpace(raw)) continue;
                var parts = raw.Split(delimiter);
                if (parts.Length <= idColumnIndex) continue;
                if (int.TryParse(parts[idColumnIndex], out int id))
                {
                    if (id > max) max = id;
                }
            }
            return max + 1;
        }
    }

    class Program
    {
        const string ProductsFile = "products.csv";
        const string UsersFile = "users.csv";
        const string ProductsHeader = "Id;Name;Price;Quantity;Category";
        const string UsersHeader = "Id;Email;PasswordHash";

        static List<Product> products = new List<Product>();
        static List<User> users = new List<User>();

        static void Main(string[] args)
        {
            Console.Title = "Фітнес центр ОЛІМП";

            CsvFileHelper.EnsureFileWithHeader(ProductsFile, ProductsHeader);
            CsvFileHelper.EnsureFileWithHeader(UsersFile, UsersHeader);

            LoadProductsFromFile();
            LoadUsersFromFile();

            Console.Clear();
            Console.WriteLine("Фітнес центр ОЛІМП\n");

            bool auth = false;
            while (!auth)
            {
                Console.WriteLine("1. Увійти\n2. Зареєструватися\n3. Робота як гість\n0. Вихід");
                Console.Write("Вибір: ");
                var c = Console.ReadLine();
                switch (c)
                {
                    case "1": auth = LoginUser(); break;
                    case "2": RegisterUser(); break;
                    case "3": auth = true; break;
                    case "0": return;
                    default: Console.WriteLine("Невірно"); break;
                }
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
                Console.WriteLine("5. Редагувати товар");
                Console.WriteLine("6. Сортування товарів");
                Console.WriteLine("7. Статистика");
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
                        case 5: EditProduct(); break;
                        case 6: SortProducts(); break;
                        case 7: ShowStatistics(); break;
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

        #region Users & Auth
        static void LoadUsersFromFile()
        {
            users.Clear();
            foreach (var parts in CsvFileHelper.ReadCsv(UsersFile, UsersHeader))
            {
                try
                {
                    if (parts.Length < 3) continue;
                    if (!int.TryParse(parts[0], out int id)) continue;
                    var email = parts[1];
                    var pass = parts[2];
                    users.Add(new User(id, email, pass));
                }
                catch { continue; }
            }
        }

        static void SaveUsersToFile()
        {
            var lines = new List<string> { UsersHeader };
            foreach (var u in users)
                lines.Add(u.ToCsvLine());
            File.WriteAllLines(UsersFile, lines);
        }

        static void RegisterUser()
        {
            Console.Clear();
            Console.WriteLine("=== Реєстрація ===");
            Console.Write("Email: ");
            string email = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                Pause("Некоректний email.");
                return;
            }

            if (users.Any(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase)))
            {
                Pause("Email вже використовується.");
                return;
            }

            Console.Write("Пароль: ");
            string pass = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(pass) || pass.Length < 4) { Pause("Пароль занадто короткий."); return; }

            string hash = HashPassword(pass);
            int newId = CsvFileHelper.GenerateNewId(UsersFile);
            var user = new User(newId, email, hash);
            users.Add(user);
            using (var sw = new StreamWriter(UsersFile, true))
            {
                sw.WriteLine(user.ToCsvLine());
            }
            Pause("Реєстрація пройшла успішно.");
        }
        static bool LoginUser()
        {
            Console.Clear();
            Console.WriteLine("=== Вхід ===");
            Console.Write("Email: ");
            string email = Console.ReadLine()?.Trim();
            Console.Write("Пароль: ");
            string pass = Console.ReadLine();

            var found = users.FirstOrDefault(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
            if (found.Email == null)
            {
                Pause("Користувача не знайдено.");
                return false;
            }

            string hash = HashPassword(pass);
            if (hash == found.PasswordHash)
            {
                Pause("Успішний вхід.");
                return true;
            }
            else
            {
                Pause("Неправильний пароль.");
                return false;
            }
        }
        static string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "");
            }
        }
        #endregion

        #region Products CRUD
        static void LoadProductsFromFile()
        {
            products.Clear();
            foreach (var parts in CsvFileHelper.ReadCsv(ProductsFile, ProductsHeader))
            {
                try
                {
                    if (parts.Length < 5) continue;
                    if (!int.TryParse(parts[0], out int id)) continue;
                    var name = parts[1];
                    if (!double.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double price)) continue;
                    if (!int.TryParse(parts[3], out int qty)) continue;
                    var cat = parts[4];
                    products.Add(new Product(id, name, price, qty, cat));
                }
                catch { continue; }
            }
        }
        static void SaveProductsToFile()
        {
            var lines = new List<string> { ProductsHeader };
            foreach (var p in products)
                lines.Add(p.ToCsvLine());
            File.WriteAllLines(ProductsFile, lines);
            Pause("Збережено.");
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

            for (int i = 0; i < count; i++)
            {
                Console.WriteLine($"\nТовар #{i + 1}:");
                Console.Write("Назва: ");
                string name = Console.ReadLine();
                Console.Write("Категорія: ");
                string cat = Console.ReadLine();

                Console.Write("Ціна: ");
                if (!double.TryParse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture, out double price) || price <= 0)
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

                int newId = CsvFileHelper.GenerateNewId(ProductsFile);
                var p = new Product(newId, name, price, qty, cat);
                products.Add(p);
                using (var sw = new StreamWriter(ProductsFile, true))
                {
                    sw.WriteLine(p.ToCsvLine());
                }
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

            Console.WriteLine($"{"ID",3} | {"Назва",-20} | {"Категорія",-12} | {"Ціна",9} | Qty");
            Console.WriteLine(new string('-', 65));

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
                if (!string.IsNullOrWhiteSpace(p.Name) && p.Name.IndexOf(query ?? "", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Console.WriteLine("Знайдено:");
                    p.Display();
                    found = true;
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

            int index = products.FindIndex(p => p.Id == id);
            if (index == -1)
            {
                Pause("Товар не знайдено.");
                return;
            }

            products.RemoveAt(index);
            SaveProductsToFile();
            Pause("Видалено.");
        }

        static void EditProduct()
        {
            Console.Clear();
            Console.WriteLine("=== Редагування товару ===");
            Console.Write("Введіть ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) { Pause("Помилка."); return; }

            int idx = products.FindIndex(p => p.Id == id);
            if (idx == -1) { Pause("Товар не знайдено."); return; }

            var p = products[idx];
            Console.WriteLine("Поточні дані:"); p.Display();

            Console.Write("Нова назва (Enter — без змін): "); string name = Console.ReadLine();
            Console.Write("Нова категорія (Enter — без змін): "); string cat = Console.ReadLine();
            Console.Write("Нова ціна (Enter — без змін): "); string priceStr = Console.ReadLine();
            Console.Write("Нова кількість (Enter — без змін): "); string qtyStr = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(name)) p.Name = name;
            if (!string.IsNullOrWhiteSpace(cat)) p.Category = cat;
            if (!string.IsNullOrWhiteSpace(priceStr) && double.TryParse(priceStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double newPrice)) p.Price = newPrice;
            if (!string.IsNullOrWhiteSpace(qtyStr) && int.TryParse(qtyStr, out int newQty)) p.Quantity = newQty;

            products[idx] = p;
            SaveProductsToFile();
            Pause("Оновлено.");
        }

        static void SortProducts()
        {
            Console.Clear();
            Console.WriteLine("=== Сортування ===");
            Console.WriteLine("1. За назвою");
            Console.WriteLine("2. За ціною");
            Console.WriteLine("3. За кількістю");

            Console.Write("Ваш вибір: ");
            if (!int.TryParse(Console.ReadLine(), out int ch)) { Pause("Невірний вибір."); return; }

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
        #endregion

        #region Statistics & Helpers
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

        static void Pause(string msg = "Натисніть клавішу...")
        {
            Console.WriteLine("\n" + msg);
            Console.ReadKey();
        }
        #endregion
    }
}