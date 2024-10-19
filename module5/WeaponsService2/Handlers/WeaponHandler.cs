using Confluent.Kafka;
using System.Net;
using WeaponsService2.Contracts;

namespace WeaponsService2.Handlers
{
    /// <summary>
    /// Предоставляет методы для работы с оружием.
    /// </summary>
    public class WeaponHandler
    {
        private WeaponDto sword;
        private WeaponDto knife;
        private WeaponDto arrow;
        private static bool isRandomEnabled = false;
        ProducerConfig config = new ProducerConfig
        {
            BootstrapServers = "localhost:9092",
            ClientId = Dns.GetHostName(),
        };

        public WeaponHandler()
        {
            sword = new WeaponDto
            {
                Id = 1,
                Title = "Sword",
                Quantity = 0,
            };
            knife = new WeaponDto
            {
                Id = 2,
                Title = "Knife",
                Quantity = 0,
            };
            arrow = new WeaponDto
            {
                Id = 3,
                Title = "Arrow",
                Quantity = 0,
            };
        }

        /// <summary>
        /// Убрать 1 оружие из списка доступных.
        /// </summary>
        /// <param name="id">Id оружия.</param>
        /// <returns>Признак успеха.</returns>
        public bool ReduceWeapon(long id)
        {
            WeaponDto weapon;
            switch (id)
            {
                case 1:
                    weapon = sword;
                    break;
                case 2:
                    weapon = knife;
                    break;
                case 3:
                    weapon = arrow;
                    break;
                default:
                    return false;
            }
            if (weapon.Quantity <= 0)
            {
                return false;
            }

            weapon.Quantity--;
            Console.WriteLine($"Remove 1 {weapon.Title}");
            return true;
        }

        /// <summary>
        /// Добавиь n-ое количество оружия в список доступных.
        /// </summary>
        /// <param name="id">Id оружия.</param>
        /// <param name="quantity">Количество оружия.</param>
        public async void AddWeapon(long id, int quantity)
        {
            WeaponDto weapon = GetWeapon(id);
            try
            {
                using (var producer = new ProducerBuilder<Null, string>(config).Build())
                {
                    weapon.Quantity = weapon.Quantity + quantity;
                    var result = await producer.ProduceAsync($"Service2.Add.Weapon{weapon.Id}", new Message<Null, string> { Value = $"{weapon.Quantity}" });

                    Console.WriteLine($"Add {quantity} {weapon.Title}. Available quantity - {weapon.Quantity}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task StartAddRandomWeapon()
        {
            isRandomEnabled = true;
            var rnd = new Random();
            int weaponId = 0;
            int quantity;
            while (isRandomEnabled)
            {
                weaponId = rnd.Next(3) + 1;
                quantity = rnd.Next(5) + 1;
                AddWeapon(weaponId, quantity);
                await Task.Delay(10000);
            }
        }

        public void StopAddRandomWeapon()
        {
            isRandomEnabled = false;
        }

        public bool ReserveWeapon(long weaponId)
        {
            WeaponDto weapon = GetWeapon(weaponId);
            if (weapon.Quantity <= 0) return false;

            weapon.Quantity--;
            Console.WriteLine($"Remove 1 {weapon.Title}. Available quantity - {weapon.Quantity}");
            return true;            
        }

        private WeaponDto GetWeapon(long id)
        {
            switch (id)
            {
                case 1:
                    return sword;
                case 2:
                    return knife;
                case 3:
                    return  arrow;
                default:
                    return null;
            }
        }
    }
}
