using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace SOExercise
{
    class Program
    {
        private Task task;
        private MCP mcp;
        private int sumDeadLine;
        public struct SOL
        {
            public SOL(int id, int mcp, int core)
            {
                ID = id;
                MCP = mcp;
                CORE = core;
            }
            public int ID { get; set; }
            public int MCP { get; }
            public int CORE { get; }

            public void setId(int temp)
            {
                ID = temp;
            }
        }

        static long gcd(long a, long b)
        {
            if (a == 0)
                return b;

            return gcd(b % a, a);
        }

        static long lcm(long a, long b)
        {
            return a / gcd(a, b) * b;
        }
        public void readXML(XmlTextReader reader, int MCPId)
        {
            task = new Task();
            mcp = new MCP();
            sumDeadLine = 0;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element: // The node is an element.
                        if (reader.Name == "Task")
                        {
                            while (reader.MoveToNextAttribute()) // Read the attributes.
                            {
                                if (reader.Name == "Deadline")
                                {
                                    task.Deadline.Add(Convert.ToInt32(reader.Value));
                                    sumDeadLine += Convert.ToInt32(reader.Value);
                                }
                                else if (reader.Name == "Id")
                                {
                                    task.Id.Add(Convert.ToInt32(reader.Value));
                                }
                                else if (reader.Name == "Period")
                                {
                                    task.Period.Add(Convert.ToInt32(reader.Value));
                                }
                                else if (reader.Name == "WCET")
                                {
                                    task.WCET.Add(Convert.ToInt32(reader.Value));
                                }
                            }
                        }
                        else if (reader.Name == "MCP")
                        {
                            mcp.core.Add(new List<MCP.Core>());
                            while (reader.MoveToNextAttribute()) // Read the attributes.
                            {
                                if (reader.Name == "Id")
                                {
                                    MCPId = Convert.ToInt32(reader.Value);
                                }
                            }
                        }
                        else if (reader.Name == "Core")
                        {
                            int coreId = 0;
                            double coreWCETFactor = 0.0;

                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name == "Id")
                                {
                                    coreId = (Convert.ToInt32(reader.Value));
                                }
                                else if (reader.Name == "WCETFactor")
                                {
                                    coreWCETFactor = (Convert.ToDouble(reader.Value));
                                }
                            }
                            
                            mcp.core[MCPId].Add(new MCP.Core( coreId, coreWCETFactor));
                        }
                        break;
                    case XmlNodeType.EndElement: //Display the end of the element.
                        break;
                }
            }
        }
        public void writeXML(List<SOL> sol)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            XmlWriter writer = XmlWriter.Create("..\\..\\..\\test_cases\\solution_large.xml", settings);

            writer.WriteStartElement("solution");

            foreach (var s in sol)
            {
                writer.WriteStartElement("Task");
                writer.WriteAttributeString("Id", s.ID.ToString());
                writer.WriteAttributeString("MCP", s.MCP.ToString());
                writer.WriteAttributeString("Core", s.CORE.ToString());
                writer.WriteAttributeString("WCRT", ((int)(Math.Round(task.WCET[s.ID] * mcp.core[s.MCP][s.CORE].WCETF))).ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.Flush();

            Console.WriteLine("Write XML file and total laxity: " + ((int)calLaxity(sol)));
        }
        public bool isSolution(List<SOL> sol)
        {
            //Console.WriteLine("first test");
            // check if mcp matches
            for (int i = 0; i < mcp.core.Count(); i++)
            {
                for (int j = 0; j < mcp.core[i].Count(); j++)
                {
                    bool isMCPCoreMatch = false;
                    for (int k = 0; k < sol.Count(); k++)
                    {
                        if (sol[k].MCP == i && sol[k].CORE == j)
                        {
                            isMCPCoreMatch = true;
                            break;
                        }
                    }
                    if (!isMCPCoreMatch)
                    {
                        return false;
                    }
                }
            }
            //Console.WriteLine("2nd test");
            for (int i = 0; i < mcp.core.Count(); i++)
            {
                for (int j = 0; j < mcp.core[i].Count(); j++)
                {
                    List<int> taskId = new List<int>();
                    foreach(var s in sol)
                    {
                        if(s.MCP == i && s.CORE > 0)
                        {
                            taskId.Add(s.ID);
                        }
                    }
                    long lcmDeadline = 1;
                    if(taskId.Count() >= 1)
                    {
                        for(int k = 0; k < taskId.Count(); k++)
                        {
                            lcmDeadline = lcm(lcmDeadline, task.Deadline[taskId[k]]);
                        }
                    }

                    long sumOfdeadline = 0;

                    for(int k = 0; k < taskId.Count(); k++)
                    {
                        sumOfdeadline += (task.Deadline[taskId[k]] / lcmDeadline * task.WCET[taskId[k]]);
                    }
                    //Console.WriteLine("sumOfdeadline:" + sumOfdeadline + ", lcmDeadline:" + lcmDeadline);

                    if (sumOfdeadline > lcmDeadline)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        static int genRandom(int min, int max)
        {
            Random random = new Random(); 
            return random.Next(min, max);
        }
        public List<SOL> genRandomSolution()
        {
            List<SOL> sol = new List<SOL>();

            do
            {
                sol.Clear();
                // number of task
                for (int i = 0; i < task.Id.Count(); i++)
                {
                    // platform size ( number of mcp )
                    int rnp = genRandom(0, mcp.core.Count());
                    // number of mcp[]
                    int rnc = genRandom(0, mcp.core[rnp].Count());
                    sol.Add(new SOL(i, rnp, rnc));
                }
            } while (!isSolution(sol));

            Console.WriteLine("Solution is created");

            return sol;
        }

        public double calLaxity(List<SOL> sol)
        {
            double sum = 0;
            foreach (var s in sol)
            {
                sum += mcp.core[s.MCP][s.CORE].WCETF * task.WCET[s.ID];
            }

            return sumDeadLine - sum;
        }

        public List<SOL> swapNeighbourRandomly(List<SOL> sol)
        {
            int i = genRandom(0, sol.Count());
            int j = genRandom(0, sol.Count());

            int temp = sol[i].ID;
            sol[i].setId(sol[i].ID);
            sol[j].setId(temp);

            return sol;
        }

        public bool calProbability(double delta, double temp)
        {
            double exponential = Math.Exp((-1 / temp) * delta);
            double probability = 1 / genRandom(1,100);

            return exponential >= probability;
        }

        public List<SOL> simulatedAnnealing(List<SOL> initialSol)
        {
            double temp = 10000;
            double alpha = 0.997;
            double delta = 0.0;

            List<SOL> tempSol = initialSol;

            while (temp > 1)
            {
                List<SOL> randomSolution;
                do
                {
                    randomSolution = swapNeighbourRandomly(tempSol);
                } while (!isSolution(randomSolution));

                delta = calLaxity(tempSol) - calLaxity(randomSolution);

                if (delta < 0 || calProbability(delta, temp))
                {
                    tempSol = randomSolution;
                }

                temp *= alpha;
            }

            return tempSol;
        }

        static void Main(string[] args)
        {
            String URLString = "..\\..\\..\\test_cases\\large.xml";
            XmlTextReader reader = new XmlTextReader(URLString);
            Program p = new Program();
            List<SOL> initialSolution = new List<SOL>();
            List<SOL> sol = new List<SOL>();

            // read XML file
            p.readXML(reader, 0);
            
            // create random solution
            initialSolution = p.genRandomSolution();

            // run SA
            sol = p.simulatedAnnealing(initialSolution);

            // wrtie solution
            p.writeXML(sol);
        }
    }
}
