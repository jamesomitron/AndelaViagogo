    using System; 
    using System.Collections.Generic; 
    using System.IO;
    using System.Linq;

namespace Viagogo 
    {
        public class Event
        {
            public string Name{ get; set; }
            public string City{ get; set; }
            public decimal Price{ get; set; } = 0;
        }
        public class Customer
        {
            public string Name{ get; set; }
            public string City{ get; set; }
        }


        public class Solution
        {
            static async Task Main(string[] args)
            {
                var events = new List<Event>{
                new Event{ Name = "Phantom of the Opera", City = "New York"},
                new Event{ Name = "Metallica", City = "Los Angeles"},
                new Event{ Name = "Metallica", City = "New York"},
                new Event{ Name = "Metallica", City = "Boston"},
                new Event{ Name = "LadyGaGa", City = "New York"},
                new Event{ Name = "LadyGaGa", City = "Boston"},
                new Event{ Name = "LadyGaGa", City = "Chicago"},
                new Event{ Name = "LadyGaGa", City = "San Francisco"},
                new Event{ Name = "LadyGaGa", City = "Washington"}
                };
                //1. find out all events that arein cities of customer
                // then add to email.

                var customers = new List<Customer>{new Customer{ Name = "Nathan", City = "New York"},
                                                    new Customer{ Name = "Bob", City = "Boston"},
                                                    new Customer{ Name = "Cindy", City = "Chicago"},
                                                    new Customer{ Name = "Lisa", City = "Los Angeles"}
                                                    };

                foreach(var customer in customers){
                    Console.WriteLine(customer.Name);

                    await SendInCityEvents(events, customer);
                    await SendNearCityEvents(events, customer);

                    BatchProcessNearCityEvents(events, customer);
                }
            }

            private static async Task SendInCityEvents(List<Event> events, Customer customer, SortOptions sortOptions = SortOptions.Event){
                var eventsCustomerCity = events.Where(c => c.City == customer.City);

                if (sortOptions == SortOptions.Event){
                    eventsCustomerCity.OrderBy(c => c.Name);
                }

                if (sortOptions == SortOptions.City){
                    eventsCustomerCity.OrderBy(c => c.City);
                }

                if (sortOptions == SortOptions.Price){
                    eventsCustomerCity.OrderBy(c => c.Price);
                }

                Console.WriteLine("Events In Your City");
                foreach(var item in eventsCustomerCity)
                {
                    await AddToEmail(customer, item);
                    Console.WriteLine(item.Name);
                }
                /*
                We want you to send an email to this customer with all events in their city
                Just call AddToEmail(customer, event) for each event you think they should get
                */
            }

            private static async Task SendNearCityEvents(List<Event> events, Customer customer, SortOptions sortOptions = SortOptions.Event){
                //2. TASK
                //The assumption in this solution is that if GetDistance returns an integer less than 401, then it is close
                try{
                    var closeCityEvents = events.Where(c => c.City != customer.City);

                    Console.WriteLine("Events Near Your City");
                    foreach(var item in closeCityEvents)
                    {
                    var dist = await GetDistance(customer.City, customer.City) <= 400;
                    if (dist)
                    {
                        await AddToEmail(customer, item);
                        Console.WriteLine(item.Name);
                    }
                            
                    }
                }
                catch{
                    throw new Exception("Error encountered, please try again");
                }
            }
            // You do not need to know how these methods work

            private async static Task BatchProcessNearCityEvents(List<Event> events, Customer customer){
                //Due to the possibility of failure and the expensive interface, we can schedule the task to run it at a later time.
                await SendNearCityEvents(events, customer);
            }

            private enum SortOptions{
                Event = 0,
                City = 1,
                Price = 2
            }

            static async Task AddToEmail(Customer c, Event e, int? price = null)
            {
                var distance = await GetDistance(c.City, e.City);
                Console.Out.WriteLine($"{c.Name}: {e.Name} in {e.City}"
                + (distance > 0 ? $" ({distance} miles away)" : "")
                + (price.HasValue ? $" for ${price}" : ""));
            }
            static async Task<int> GetPrice(Event e)
            {
                return (await AlphebiticalDistance(e.City, "") + await AlphebiticalDistance(e.Name, "")) / 10;
            }
            static async Task<int> GetDistance(string fromCity, string toCity)
            {
                return await AlphebiticalDistance(fromCity, toCity);
            }
            private static async Task<int> AlphebiticalDistance(string s, string t)
            {
                var result = 0;
                var i = 0;
                for(i = 0; i < Math.Min(s.Length, t.Length); i++)
                {
                // Console.Out.WriteLine($"loop 1 i={i} {s.Length} {t.Length}");
                result += Math.Abs(s[i] - t[i]);
                }
                for(; i < Math.Max(s.Length, t.Length); i++)
                {
                // Console.Out.WriteLine($"loop 2 i={i} {s.Length} {t.Length}");
                result += s.Length > t.Length ? s[i] : t[i];
                }

                return result;
            }
        }
    }