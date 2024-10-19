using Confluent.Kafka;
using OrchestratorService.Contracts;
using System;
using System.Net.Http;
using System.Security.Cryptography;

namespace OrchestratorService.Handlers
{
    /// <summary>
    /// Предоставляет методы для резервирования товаро пользователями.
    /// </summary>
    public class ReserveHandler : IHostedService
    {
        private ReserveParamsDto[] queue;
        private WeaponDto sword;
        private WeaponDto knife;
        private WeaponDto arrow;
        private UserDto ivan;
        private UserDto petr;
        private UserDto oleg;
        private static bool isRandomEnabled = false;
        private readonly List<string> topics = new List<string> {
            "Service1.Add.Weapon1",
            "Service1.Add.Weapon2",
            "Service1.Add.Weapon3",
            "Service2.Add.Weapon1",
            "Service2.Add.Weapon2",
            "Service2.Add.Weapon3",
        };
        private static bool isReserveEnabled = false;
        private readonly Uri service1uri = new Uri("http://localhost:5134/");
        private readonly Uri service2uri = new Uri("http://localhost:5135/");

        public ReserveHandler()
        {
            sword = new WeaponDto
            {
                WeaponId = 1,
                Title = "Sword",
            };
            knife = new WeaponDto
            {
                WeaponId = 2,
                Title = "Knife",
            };
            arrow = new WeaponDto
            {
                WeaponId = 3,
                Title = "Arrow",
            };
            ivan = new UserDto
            {
                Id = 1,
                Name = "Ivan",
            };
            petr = new UserDto
            {
                Id = 2,
                Name = "Petr",
            };
            oleg = new UserDto
            {
                Id = 3,
                Name = "Oleg",
            };
            queue = new ReserveParamsDto[] {
                new ReserveParamsDto
                {
                    Weapon = sword,
                    UsersQueue = new List<UserDto>(),
                },
                new ReserveParamsDto
                {
                    Weapon = knife,
                    UsersQueue = new List<UserDto>(),
                },
                new ReserveParamsDto
                {
                    Weapon = arrow,
                    UsersQueue = new List<UserDto>(),
                }
            };
        }

        public async Task StartAddRandomUserToQueue()
        {
            isRandomEnabled = true;
            var rnd = new Random();
            int rndQue = rnd.Next(3);
            int rndUser = rnd.Next(3) + 1;
            UserDto user = new UserDto();
            
            while (isRandomEnabled)
            {
                switch (rndUser)
                {
                    case 1:
                        user = ivan;
                        break;
                    case 2:
                        user = petr;
                        break;
                    case 3:
                        user = oleg;
                        break;
                    default:
                        break;
                }

                queue[rndQue].UsersQueue.Add(user);
                Console.WriteLine($"Add user {user.Id}-{user.Name} to reserve {queue[rndQue].Weapon.Title}");
                rndQue = rnd.Next(3);
                rndUser = rnd.Next(3) + 1;
                await Task.Delay(3000);
            }
        }

        public void StopAddRandomUserToQueue()
        {
            isRandomEnabled = false;
        }

    
        public Task StartAsync(CancellationToken canceltoken)
        {
            StartAddRandomUserToQueue();
            var config = new ConsumerConfig
            {
                GroupId = "orhecstrator.group",
                BootstrapServers = "localhost:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };
            isReserveEnabled = true;

            try
            {
                using (var consumerBuilder = new ConsumerBuilder<Ignore, string>(config).Build())
                {
                    consumerBuilder.Subscribe(topics);

                    try
                    {
                        while (isReserveEnabled)
                        {
                            var consumer = consumerBuilder.Consume(canceltoken);
                            var top = consumer.Topic;
                            var log = consumer.Message.Value;
                            TryToReserve(top, Convert.ToInt32(log));
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        consumerBuilder.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async void TryToReserve(string topic, int quantity)
        {
            string route = "Weapon/reserve-weapon";
            int weaponId = 0;
            Uri uri = null;
            switch (topic)
            {
                case "Service1.Add.Weapon1":
                    uri = service1uri;
                    weaponId = 1;
                    break;
                case "Service2.Add.Weapon1":
                    uri = service2uri;
                    weaponId = 1;
                    break;
                case "Service1.Add.Weapon2":
                    uri = service1uri;
                    weaponId = 2;
                    break;
                case "Service2.Add.Weapon2":
                    uri = service2uri;
                    weaponId = 2;
                    break;
                case "Service1.Add.Weapon3":
                    uri = service1uri;
                    weaponId = 3;
                    break;
                case "Service2.Add.Weapon3":
                    uri = service2uri;
                    weaponId = 3;
                    break;
            }

            if (uri == null) return;

            HttpClient client = new()
            {
                BaseAddress = uri,
            };
            
            for (int i = 0; i < quantity; i ++)
            {
                if (queue[weaponId - 1].UsersQueue.Count == 0) break;

                using HttpResponseMessage response = await client.PostAsJsonAsync<long>(
                    route,
                    value: weaponId);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                var result = await response.Content.ReadFromJsonAsync<bool>();
                if (!result) break;

                var user = queue[weaponId - 1].UsersQueue[0];
                queue[weaponId - 1].UsersQueue.RemoveAt(0);
                Console.WriteLine($"-----{user.Id}-{user.Name} succefully reserve {queue[weaponId - 1].Weapon.Title}");
            }
        }
    }
}
