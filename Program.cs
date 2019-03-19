using ConsoleApp1;
using System;
using System.Linq;
using System.Threading;

namespace TemperatureController
{

    class Program
    {
        public delegate void func_ptr();
        public delegate void message_func_ptr(int temp, DateTime date);
        static int[] random_array = new int[10];
        static int min = 0, max = 0;

        public static void Main(string[] args)
        {
            Raise_Event raise = new Raise_Event();
            Console.WriteLine("1. Generate random and insert in database");
            Console.WriteLine("2. Retreive from database and find average of the temperature");

            int option = int.Parse(Console.ReadLine());
            func_ptr del_object;

            switch (option)
            {
                case 1:
                    del_object = new func_ptr(generateTemp);
                    del_object.Invoke();
                    Console.ReadKey();
                    break;
                case 2:
                    del_object = new func_ptr(Retrieve);
                    del_object.Invoke();
                    Console.ReadKey();
                    break;
                default:
                    Console.WriteLine("Enter correct value");
                    Main(null);
                    break;
            }
        }
        static void generateTemp()
        {
            int random_number;
            Random random = new Random();

            Console.WriteLine("Enter the Min Temperature: ");
            min = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter the Max treshold: ");
            max = int.Parse(Console.ReadLine());

            for (int count = 0; count < 10; count++)
            {
                random_number = random.Next(min, max);
                Thread.Sleep(1000);
                random_array[count] = random_number;
            }

            for (int count = 0; count < 10; count++)
            {
                if (random_array[count] == min || random_array[count] == max)
                {
                    using (Temprature_DetailEntities context = new Temprature_DetailEntities())
                    {
                        Temprature_Details tmp = new Temprature_Details
                        {
                            Temprature = random_array[count].ToString(),
                            Time = DateTime.Now
                        };
                        context.Temprature_Details.Add(tmp);
                        context.SaveChanges();
                    }
                    message_func_ptr del_object = new message_func_ptr(send_warning);
                    del_object(random_array[count], DateTime.Now);
                    Console.ReadKey();

                }
            }
            Console.ReadKey();
        }

        static void Retrieve()
        {
            Temprature_DetailEntities context = new Temprature_DetailEntities();
            var values= from temp in context.Temprature_Details select temp;
            float avg_temp = 0, count = 0;
            foreach (var item in values)
            {
                Console.WriteLine("The value recorded was: " + item.Temprature + " at: " + item.Time);
                avg_temp += int.Parse(item.Temprature);
                count++;
            }
            Console.WriteLine("Average Temperature : " + (avg_temp / count));
        }

        public class send_message
        {
            Raise_Event raise;

            public void send_warning(int temp, DateTime date)
            {
                for (int count = 0; count < 10; count++)
                {
                    if (random_array[count] == min || random_array[count] == max)
                    {
                        Console.WriteLine("{0} degree reached at {1}", temp, date);
                    }
                }
            }
            
        }

    }

    class Raise_Event
    {
        public delegate void EventHandler(string val);
        public event EventHandler Warning = null;

        public void ThresholdReached(string msg)
        {
            Warning?.Invoke(msg);
        }

        


    }
}