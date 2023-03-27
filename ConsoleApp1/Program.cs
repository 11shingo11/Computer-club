using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Threading;

namespace ConsoleApp1
{

    internal class Program
    {
        static void Main(string[] args)
        {
            ComputerClub computerClub = new ComputerClub(8);
            computerClub.Work();
            
        }

        class ComputerClub
        {
            private int _money = 0;
            private List<Computer> _computers = new List<Computer>();
            private Queue<Client> _clients = new Queue<Client>();

            public ComputerClub(int computersCount)
            {
                Random random = new Random();

                for (int i = 0; i < computersCount; i++)
                {
                    _computers.Add(new Computer(random.Next(5, 15)));
                }
                CreateNewClients(25, random);
            }

            public void CreateNewClients(int count, Random random)
            {
                for (int i = 0;i < count;i++)
                {
                    _clients.Enqueue(new Client(random.Next(100, 251), random));
                }
            }

            public void Work()
            {
                while (_clients.Count > 0)
                {
                    Client newClient = _clients.Dequeue();
                    Console.WriteLine($"Баланс: {_money}р. Ждем нового клиента.");
                    Console.WriteLine($"У вас новый клиент хочет купить {newClient.DesireMinutes} минут");
                    ShowAllComputersState();

                    Console.WriteLine("\nВы предлагаете ему компьютер: ");
                    string userInput = Console.ReadLine();
                    if(int.TryParse(userInput, out int computerNumber))
                    {
                        computerNumber -=1;
                        if(computerNumber >= 0 && computerNumber < _computers.Count)
                        {
                            if (_computers[computerNumber].IsTaken)
                            {
                                Console.WriteLine("Comp is taken sorry.Client is gone");
                            }
                            else
                            {
                                if (newClient.CheckSolvency(_computers[computerNumber]))
                                {
                                    Console.WriteLine("Payment sucsess your computer - " + computerNumber + 1);
                                    _money += newClient.Pay();
                                    _computers[computerNumber].BecomeTaken(newClient); 
                                }
                                else
                                {
                                    Console.WriteLine("Not enoght money!");
                                }
                            }
                        }
                    }
                    else
                    {   
                        CreateNewClients(1, new Random());
                        Console.WriteLine("Error, Try again.");
                    }


                    Console.ReadKey();
                    Console.Clear();
                    SpendOneMinute();
                }
            }

            private void ShowAllComputersState()
            {
                Console.WriteLine("\nСписок всех компьютеров: ");
                for (int i = 0; i < _computers.Count; i++)
                {   
                    Console.Write(i + 1 + " - ");
                    _computers[i].ShowState();
                }
                    
            }

            private void SpendOneMinute()
            {
                foreach(var computer in _computers)
                {
                    computer.SpendOneMinute();
                }
            }
        }

        class Computer
        {
            private Client _client;
            private int _minutesRemaining;
            public bool IsTaken
            {
                get
                {
                    return _minutesRemaining > 0;
                }
            }
            public int PricePerMinute { get; private set; }

            public Computer(int pricePerMinute)
            {
                PricePerMinute = pricePerMinute;

            }
            public void BecomeTaken(Client client)
            {
                _client = client;
                _minutesRemaining = _client.DesireMinutes;
            }

            public void BecomeEmpty()
            {
                _client = null;
            }

            public void SpendOneMinute()
            {
                _minutesRemaining--;
            }

            public void ShowState()
            {
                if (IsTaken)
                    Console.WriteLine($"Компьютез будет занят еще {_minutesRemaining} минут");
                else
                    Console.WriteLine($"Компьютер свободен. Цена : {PricePerMinute}");
            }

        }

        class Client
        {
            private int _money;
            private int _moneyToPay;
            public int DesireMinutes { get; private set; }

            public Client(int money, Random random) 
            {
                _money = money;
                DesireMinutes = random.Next(10,30);
            }

            public bool CheckSolvency(Computer computer)
            {
                _moneyToPay = DesireMinutes * computer.PricePerMinute;
                if( _money >= _moneyToPay)
                {
                    return true;
                }
                else
                {
                    _moneyToPay = 0;
                    return false;
                }
            }

            public int Pay()
            {
                _money -= _moneyToPay;
                return _moneyToPay;
            }
        }
    } 
}
