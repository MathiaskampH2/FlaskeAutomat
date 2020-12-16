using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace FlaskeAutomat
{
    class Program
    {
        static Queue<Bottle> bottlesMade = new Queue<Bottle>();
        static Queue<Bottle> bottlesSortedSoda = new Queue<Bottle>();
        static Queue<Bottle> bottlesSortedBeer = new Queue<Bottle>();

        private static int counter = 0;
        const int idSoda = 10;
        const int idBeer = 20;
        static Random rand = new Random();

        // method SetIDSoda builds a string which contains idSoda number and a counter number to make it easy to identify the soda
        public static string SetIdSoda(int counter)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(idSoda);
            sb.Append(counter);

            return sb.ToString();
        }

        // method SetIdBeer builds a string which contains idSoda number and a counter number to make it easy to identify the beer
        public static string SetIdBeer(int counter)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(idBeer);
            sb.Append(counter);

            return sb.ToString();
        }

        // makes either a soda bottle or beer bottle and enqueue it into bottlesMade queue
        public static void ProduceBottles()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                lock (bottlesMade)
                {
                    while (bottlesMade.Count != 1)

                    {
                        Bottle temp = new Bottle();

                        int num = rand.Next(0, 2);

                        if (num == 1)
                        {
                            counter++;
                            temp = new Bottle() {Id = SetIdSoda(counter), Name = "Soda"};
                            Console.WriteLine($"id : {temp.Id}  type :{temp.Name} has been produced");
                        }
                        else
                        {
                            counter++;
                            temp = new Bottle() {Id = SetIdBeer(counter), Name = "Beer"};
                            Console.WriteLine($"id : {temp.Id}  type :{temp.Name} has been produced");
                        }

                        bottlesMade.Enqueue(temp);
                    }

                    Monitor.PulseAll(bottlesMade);
                }

                Thread.Sleep(rand.Next(1000));
            }
        }

        // sorts the bottle of bottlesMade queue and adds it to either bottleSortedSoda or BottleSortedBeer depending on what type of Id the bottle has
        public static void SortProducedBottles()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                lock (bottlesMade)
                {
                    while (bottlesMade.Count == 0)
                    {
                        Monitor.Wait(bottlesMade);
                    }

                    Bottle temp = bottlesMade.Dequeue();

                    if (temp.Id.Substring(0, 2) == "10")
                    {
                        bottlesSortedSoda.Enqueue(temp);

                        Console.WriteLine($"id : {temp.Id}  type :{temp.Name} has been added to Soda queue");
                    }

                    else
                    {
                        bottlesSortedBeer.Enqueue(temp);
                        Console.WriteLine($"id : {temp.Id}  type :{temp.Name} has been added to Beer queue");
                    }

                }

                Thread.Sleep(1000);
                
            }
            
        }

        // Dequeue a soda from bottlesSortedSoda 
        public static void ConsumeSoda()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                while (bottlesSortedSoda.Count > 0)
                {
                    lock (bottlesSortedSoda)
                    {
                        Bottle temp = bottlesSortedSoda.Dequeue();

                        Console.WriteLine($"id : {temp.Id}  type :{temp.Name} has been consumed");
                    }
                }
            }

            Thread.Sleep(1000);
        }

        // Dequeue a beer from bottlesSortedBeer
        public static void ConsumeBeer()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                while (bottlesSortedBeer.Count > 0)
                {
                    lock (bottlesSortedBeer)
                    {
                        Bottle temp = bottlesSortedBeer.Dequeue();

                        Console.WriteLine($"id : {temp.Id}  type :{temp.Name} has been consumed");
                    }
                }
            }

            Thread.Sleep(1000);
        }


        static void Main(string[] args)
        {
            Thread produceThread = new Thread(ProduceBottles);
            Thread sortBottleType = new Thread(SortProducedBottles);
            Thread consumeSoda = new Thread(ConsumeSoda);
            Thread consumeBeer = new Thread(ConsumeBeer);
            produceThread.Start();
            sortBottleType.Start();
            consumeSoda.Start();
            consumeBeer.Start();
        }
    }
}