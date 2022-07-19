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
        public int Distance { get; set; }
        public int? Price { get; set; }
    }

    public class Customer
    {
        public string Name{ get; set; }
        public string City{ get; set; }
        
    }

    public class Solution
    {
        private static Dictionary<string, int> cachedDistances = new Dictionary<string, int>();
        
        static void Main(string[] args)
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
            var customer = new Customer{ Name = "Mr. Fake", City = "New York"};
            // var query = from result in customer
            //     where result.Contains("New York")
            //     select result;
            
            
            // 1. TASK
            
            //Get eligible events
            var eligibleEvents = events.Where(x => x.City.Equals(customer.City));
            
            foreach(var item in eligibleEvents)
            {
                AddToEmail(customer, item);
            }
            /*
            * We want you to send an email to this customer with all events in their city
            * Just call AddToEmail(customer, event) for each event you think they should get
            */
            
            // 2. SEND EMAIL FOR 5 CLOSEST EVENTS
            var fiveClosest = events.Select(e => new Event { Name = e.Name, City = e.City, Distance = GetDistance(customer.City, e.City) })
                                                            .OrderBy(e => e.Distance)
                                                            .Take(5);
            
            foreach (var item in fiveClosest)
            {
                AddToEmail(customer, item);
            }
            
            // 3. If the GetDistance method is an API call which could fail or is too expensive, OptimizedGetDistance method created below
            var fiveClosestOptimized = events.Select(e => new Event { Name = e.Name, City = e.City, Distance = OptimizedGetDistance(customer.City, e.City) })
                                                                .OrderBy(e => e.Distance)
                                                                .Take(5);
            
            foreach (var item in fiveClosestOptimized)
            {
                AddToEmail(customer, item);
            }
            
            
            // 4. If the GetDistance method can fail, we don't want the process to fail.
            
                //SAME AS SOLUTION 3 ABOVE
                
            
            // 5. Sort by other fields (e.g price)
            var furtherSortedEvents = events.Select(e => new Event { Name = e.Name, City = e.City, Distance = OptimizedGetDistance(customer.City, e.City) })
                                                            .OrderBy(e => e.Distance)
                                                            .ThenBy(e => e.Price)
                                                            .Take(5);

            foreach (var item in furtherSortedEvents)
            {
                AddToEmail(customer, item, item.Price);
            }
        }

        static int OptimizedGetDistance(string fromCity, string toCity)
        {
            
            if (fromCity.Equals(toCity))
                return 0;
            
            var key = fromCity + "_" + toCity;

            if (cachedDistances.TryGetValue(key, out var cachedDistance))
            {
                return cachedDistance;
            }
            
            try
            {
                var distance = GetDistance(fromCity, toCity);
                cachedDistances.Add(key, distance);

                return distance;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
            //Return max value since the GetDistance method failed
            return int.MaxValue;
        }
        
        
        
        // You do not need to know how these methods work
        static void AddToEmail(Customer c, Event e, int? price = null)
        {
            var distance = GetDistance(c.City, e.City);
            Console.Out.WriteLine($"{c.Name}: {e.Name} in {e.City}"
                                  + (distance > 0 ? $" ({distance} miles away)" : "")
                                  + (price.HasValue ? $" for ${price}" : ""));
        }
        
        static int GetPrice(Event e)
        {
            return (AlphebiticalDistance(e.City, "") + AlphebiticalDistance(e.Name, "")) / 10;
        }
        
        static int GetDistance(string fromCity, string toCity)
        {
            return AlphebiticalDistance(fromCity, toCity);
        }
        
        private static int AlphebiticalDistance(string s, string t)
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