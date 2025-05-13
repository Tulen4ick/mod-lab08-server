using System;
using System.Text;
using System.Threading;

namespace TPProj
{
    class Program
    {
        static void Main()
        {
            double mu = 1.0;
            int countOfProcessors = 5;
            int countOfRequests = 30;
            var currentDir = Directory.GetCurrentDirectory();
            var directory = new DirectoryInfo(currentDir);
            while (directory != null && !directory.GetFiles("*.csproj").Any())
            {
                directory = directory.Parent;
            }
            var path = Path.Combine(directory.FullName, "../data.txt");
            for (double lambda = 1.0; lambda <= 10; ++lambda)
            {
                Console.WriteLine($"Обрабатывается лямбда = {lambda}");
                StartModeling(lambda, mu, countOfRequests, countOfProcessors, path);
            }
        }

        static void StartModeling(double lambda, double mu, int countOfRequests, int countOfProcessors, string filepath)
        {
            double P0_theoretical, P0_real, Pn_theoretical, Pn_real, Q_theoretical, Q_real, A_theoretical, A_real, k_theoretical, k_real;
            int requestTime = (int)(500 / lambda);
            int responseTime = (int)(500 / mu);

            Server server = new Server(responseTime, countOfProcessors);
            Client client = new Client(server);
            server.StartTimer();
            for (int id = 1; id <= countOfRequests; id++)
            {
                client.send(id);
                Thread.Sleep(requestTime);
            }
            server.StopTimer();
            double rho = lambda / mu;
            P0_theoretical = P0(rho, countOfProcessors);
            Pn_theoretical = Pn(rho, countOfProcessors, P0_theoretical);
            Q_theoretical = 1 - Pn_theoretical;
            A_theoretical = lambda * Q_theoretical;
            k_theoretical = A_theoretical / mu;
            P0_real = (double)server.CountOfUnusedTicks / (double)server.CountOfTicks;
            Pn_real = (double)server.rejectedCount / (double)server.requestCount;
            Q_real = (double)server.processedCount / (double)server.requestCount;
            A_real = lambda * Q_real;
            k_real = A_real / mu;
            SaveToFile(lambda, mu, P0_theoretical, P0_real, Pn_theoretical, Pn_real, Q_theoretical, Q_real, A_theoretical, A_real, k_theoretical, k_real, filepath);
        }

        static double P0(double rho, int countOfProcessors)
        {
            double result = 1;
            int factorialOfIndex = 1;
            for (int i = 1; i <= countOfProcessors; ++i)
            {
                factorialOfIndex *= i;
                result += Math.Pow(rho, i) / factorialOfIndex;
            }
            return (1 / result);
        }

        static double Pn(double rho, int countOfProcessors, double p0)
        {
            int factorialOfIndex = 1;
            for (int i = 1; i <= countOfProcessors; ++i)
            {
                factorialOfIndex *= i;
            }
            return Math.Pow(rho, countOfProcessors) * p0 / factorialOfIndex;
        }

        static void SaveToFile(double lambda, double mu, double P0_theoretical, double P0_real, double Pn_theoretical,
        double Pn_real, double Q_theoretical, double Q_real, double A_theoretical,
        double A_real, double k_theoretical, double k_real, string filepath)
        {
            string newLine = "";
            newLine += lambda.ToString() + " ";
            newLine += mu.ToString() + " ";
            newLine += P0_theoretical.ToString() + " ";
            newLine += P0_real.ToString() + " ";
            newLine += Pn_theoretical.ToString() + " ";
            newLine += Pn_real.ToString() + " ";
            newLine += Q_theoretical.ToString() + " ";
            newLine += Q_real.ToString() + " ";
            newLine += A_theoretical.ToString() + " ";
            newLine += A_real.ToString() + " ";
            newLine += k_theoretical.ToString() + " ";
            newLine += k_real.ToString() + "\n";
            Console.WriteLine(newLine);
            File.AppendAllText(filepath, newLine);
        }
    }

    struct PoolRecord
    {
        public Thread thread;
        public bool in_use;
    }

    class Server
    {
        private PoolRecord[] pool;
        int responseTime;
        int countOfProcessors;
        private object threadLock = new object();
        public int requestCount = 0;
        public int processedCount = 0;
        public int rejectedCount = 0;
        public int CountOfTicks = 0;
        public int CountOfUnusedTicks = 0;
        private Timer timer;

        public Server(int responseTime, int countOfProcessors)
        {
            pool = new PoolRecord[countOfProcessors];
            this.countOfProcessors = countOfProcessors;
            this.responseTime = responseTime;
        }

        public void StartTimer()
        {
            int num = 0;
            TimerCallback tm = new TimerCallback(IsUnused);
            timer = new Timer(tm, num, 0, 500);
        }

        public void StopTimer()
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void IsUnused(object obj)
        {
            CountOfTicks++;
            for (int i = 0; i < pool.Length; ++i)
            {
                if (pool[i].in_use)
                    return;
            }
            CountOfUnusedTicks++;
        }
        public void proc(object sender, procEventArgs e)
        {
            lock (threadLock)
            {
                //Console.WriteLine("Заявка с номером: {0}", e.id);
                requestCount++;
                for (int i = 0; i < countOfProcessors; i++)
                {
                    if (!pool[i].in_use)
                    {
                        pool[i].in_use = true;
                        pool[i].thread = new Thread(new ParameterizedThreadStart(Answer));
                        pool[i].thread.Start(e.id);
                        processedCount++;
                        return;
                    }
                }
                rejectedCount++;
            }
        }

        public void Answer(object arg)
        {
            int id = (int)arg;
            //Console.WriteLine("Обработка заявки: {0}", id);
            DateTime StartOfWork = DateTime.Now;
            Thread.Sleep(responseTime);
            for (int i = 0; i < countOfProcessors; i++)
            {
                if (pool[i].thread == Thread.CurrentThread)
                {
                    pool[i].in_use = false;
                }
            }
        }
    }

    class Client
    {
        private Server server;
        public event EventHandler<procEventArgs> request;

        public Client(Server server)
        {
            this.server = server;
            this.request += server.proc;
        }

        public void send(int id)
        {
            procEventArgs args = new procEventArgs();
            args.id = id;
            OnProc(args);
        }

        protected virtual void OnProc(procEventArgs e)
        {
            EventHandler<procEventArgs> handler = request;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }

    public class procEventArgs : EventArgs
    {
        public int id { get; set; }
    }
}