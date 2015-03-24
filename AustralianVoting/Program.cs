using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace AustralianVoting
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            //Read all lines in go
            var lines = File.ReadAllLines(@"C:\Users\Chetan\Desktop\votes.txt");

            //First line indicates num of elections
            int numOfElections = Convert.ToInt32(lines[0]);

            //Using this variable to be able to get the last election lines in one go
            int electionsTracked = 0;
            
            //Creating an array of 'async' tasks to enable parallel processing
            int taskCount = 0;
            Task<ElectionResult>[] tasks = new Task<ElectionResult>[numOfElections];
            
            int start = 2;
            for (var i = 2; i < lines.Count(); i++)
            {
                //If we have extracted all other lines except for the last one
                if ((electionsTracked + 1) == numOfElections)
                {
                    //Extract all the relevant lines for the last election
                    int arrayLength = lines.Count() - start;
                    var newElectionArray = new string[arrayLength];
                    Array.Copy(lines, start, newElectionArray, 0, arrayLength);

                    int localTaskCount = taskCount;
                    tasks[localTaskCount] = Task<ElectionResult>.Factory.StartNew(() => { return new ElectionAnalyzer(newElectionArray.ToList(), (localTaskCount + 1)).GetWinner(); });

                    break;
                    
                }
                //Break in line indicates that a new election section begins next.
                //Hence get all the lines before it excluding the lines already picked
                else if(String.IsNullOrEmpty(lines[i].Trim()))
                {
                    //Extract all the relevant lines for this election
                    int arrayLength = i - start;
                    var newElectionArray = new string[arrayLength];
                    Array.Copy(lines, start, newElectionArray, 0, arrayLength);                    

                    int localTaskCount = taskCount;
                    tasks[localTaskCount] = Task<ElectionResult>.Factory.StartNew(() => { return new ElectionAnalyzer(newElectionArray.ToList(), (localTaskCount + 1)).GetWinner(); });

                    start = i + 1;
                    electionsTracked++;
                    taskCount++;
                }
            }

            //Wait till all async tasks complete
            Task.WaitAll(tasks);

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;

            //Display the output
            foreach(var task in tasks)
            {
                Console.WriteLine(task.Result.ToString());
            }

            string elapsedTime = String.Format("{0:00}Hrs:{1:00}Mins:{2:00}Secs:{3:00}Ms",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);

            Console.ReadLine();
        }
    }
}
