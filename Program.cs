using System;
using System.Linq;
using ConsoleApp1;
using System.Threading;

namespace TempratureController
{
    class Program
    {
        public delegate void func_ptr();
        static int[] random_array = new int[10];
       
        public class GenericClass<T>
        {
            private T data;

            public T value
            {
                get
                {
                    return this.data;
                }
                
                set
                {
                    this.data = value;
                }
            }
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("1. Generate random and insert in database");
            Console.WriteLine("2. Retreive from database and find average of the temperature");

            int option = int.Parse(Console.ReadLine());

            switch (option)
            {
                case 1:
                    func_ptr del_object = new func_ptr(generateTemp);
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

        public static void generateTemp()
        {
            int random_number;
            int insertSuccess = 0;
            Random random = new Random();

            Console.WriteLine("Enter the minimum temperature: ");
            GenericClass<int> minimunTemperature = new GenericClass<int>();
            minimunTemperature.value = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter the maximum temperature: ");
            GenericClass<int> maximumTemperature = new GenericClass<int>();
            maximumTemperature.value = int.Parse(Console.ReadLine());

            Console.WriteLine("Generating 10 random values each after 1 sec ");

            for (int count = 0; count < 10; count++)
            {
                random_number = random.Next(minimunTemperature.value - 10, maximumTemperature.value + 11);
                Console.Write(random_number + "\t ");
                Thread.Sleep(1000);
                random_array[count] = random_number;
            }
            Console.WriteLine("\n");

            for (int count = 0; count < 10; count++)
            {
                if (random_array[count] <= minimunTemperature.value || random_array[count] >= maximumTemperature.value)
                {
                    insertSuccess = 1;
                    using (Temprature_DetailEntities context = new Temprature_DetailEntities())
                    {
                        Temprature_Details temperature = new Temprature_Details
                        {
                            Temprature = random_array[count].ToString(),
                            Time = DateTime.Now
                        };
                        context.Temprature_Details.Add(temperature);
                        context.SaveChanges();
                    }
                    Warning.RaiseWarning(random_array[count]);
                }
            }
            if (insertSuccess == 0)
            {
                Console.WriteLine("Temperature didnt cross threshold value");
            }
            Console.WriteLine("Press a key to exit ....");
        }

        public static void Retrieve()
        {
            Temprature_DetailEntities context = new Temprature_DetailEntities();
            var values = from temp in context.Temprature_Details select temp;
            int avg_temp = 0, count = 0;

            foreach (var item in values)
            {
                Console.WriteLine("The value recorded was: " + item.Temprature + " at: " + item.Time);
                avg_temp += int.Parse(item.Temprature);
                count++;
            }
            Console.WriteLine("Average Temperature : " + (avg_temp / count) + "\n");
            Console.WriteLine("Press a key to exit ....");
        }
    }
}

public class Warning
{
    public static void RaiseWarning(int warningValue)
    {
        Warn warn = new Warn();
        Notify notification = new Notify();
        notification.Send(warn);
        warn.notificationMessage("The temp has reached: " + warningValue + " degrees at ");
    }
}

//Raising the Event 
public class Warn
{
    public delegate void warnMessage(string message);
    public event warnMessage sendMessage = null;
    public void notificationMessage(string message)
    {
        sendMessage?.Invoke(message);
    }
}

public class Notify
{
    public void sendMessage(string message)
    {
        Console.WriteLine("Warning: " + message + DateTime.Now);
    }

    public void Send(Warn send)
    {
        send.sendMessage += sendMessage;
    }
}
