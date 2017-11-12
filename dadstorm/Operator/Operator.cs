using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharedLibService;
using System.Diagnostics;

namespace DADSTORM
{
    class Operator
    {
        private TcpChannel channel;
        private string name;
        private string input_ops;
        private string rep_fact;
        private string routing;
        private string address;
        private string operator_spec;
        private string semantics;
        private string loggingLevel;
        private string previousRepFact;
        private string imlast;
        private int myPort;
        private int connectedPort;
        public RemoteOPService remoteOperator;
        public OperatorInterface previousOp; 
        public PuppetMasterInterface remotePM;
        public List<OperatorInterface> connectedReplicas;
        public string inputContent;
        public string answer;
        public string previousLog;
        public string computation;
        public string[] operation;
        public List<string> freezeInput;
        public List<string> output;
        bool isFrozen;
        bool gotHashing;
        public Dictionary<int, List<string>> uniqDictionary;

        public Operator()
        {
            answer = "";
            isFrozen = false;
            freezeInput = new List<string>();
            output = new List<string>();
            remoteOperator = new RemoteOPService();
            remoteOperator.inputCheck += new DelInputChecker(getInputContent);
            remoteOperator.hashCheck += new DelHashingChecker(getHashing);
            remoteOperator.processCommand += new DelProcessCommand(processCommand);
            remoteOperator.crashProcess += new DellKillCommand(crashProcess);
            remoteOperator.setFreezeMode += new DelFreezeCommand(setFreezeMode);
            remoteOperator.setUnreezeMode += new DelUnfreezeCommand(setUnreezeMode);
            remoteOperator.executeList += new DelExecuteList(executeList);
            connectedReplicas = new List<OperatorInterface>();
            uniqDictionary = new Dictionary<int, List<string>>();
        }
        
        public void MyInterfaceRegistery()
        {
            //--------------------------------OP Interface creation-----------------------------------//
            if (name.Contains("R"))
            {
                myPort = 8080 + Int32.Parse((name[2]).ToString()) + Int32.Parse(name[name.Length - 1].ToString()) * 100;
            }
            else
            {
                myPort = 8080 + Int32.Parse((name[2]).ToString());
            }

            Console.WriteLine("my port is: " + myPort);
            channel = new TcpChannel(myPort);
            ChannelServices.RegisterChannel(channel, false);
            RemotingServices.Marshal(remoteOperator, "op",
                typeof(RemoteOPService));
            
            
        }

        public void PreviousOpInterfaceConnection()
        {
            //---------------------------------Next OP connection------------------------------------------------------//
            if (input_ops.Contains("OP"))
            {
                connectedPort = 8080 + Int32.Parse((input_ops[2]).ToString());
                Console.WriteLine("Im going to connect to port: " + connectedPort);
                previousOp = (OperatorInterface)Activator.GetObject(
                    typeof(OperatorInterface),
                   "tcp://localhost:" + connectedPort + "/op");
                connectedReplicas.Add(previousOp);
                if (previousRepFact != "only_one")
                {
                    ThreadStart td = new ThreadStart(connectToReplicas);
                    Thread t_d = new Thread(td);
                    t_d.Start();
                }
            }
        }

        public void crashProcess()
        {
            ChannelServices.UnregisterChannel(channel);
            Process.GetCurrentProcess().Kill();
        }

        public void setFreezeMode()
        {
            isFrozen = true;
            Console.WriteLine("I was freezed!");
        }

        public void setUnreezeMode()
        {
            isFrozen = false;
            Console.WriteLine("Unfreezed!");
        }

        public void connectToReplicas()
        {
            int i = 1;
            while (i < Int32.Parse(previousRepFact))
            {
                int connectedPort2 = 8080 + Int32.Parse((input_ops[2]).ToString())+i*100;
                Console.WriteLine("Im going to connect to port: " + connectedPort2);
                OperatorInterface replicaOp = (OperatorInterface)Activator.GetObject(
                    typeof(OperatorInterface),
                   "tcp://localhost:" + connectedPort2 + "/op");
                connectedReplicas.Add(replicaOp);
                i++;
            }
        }

        public void checkInput()
        {
            if (input_ops.Contains(".dat"))
            {
                String path = "../../../tweeters.dat";
                String V = Path.GetExtension(path);
                String line;

                if (V == ".dat")
                {
                    try
                    {
                        using (StreamReader sr = new StreamReader(path))
                        {
                            while ((line = sr.ReadLine()) != null)
                            {
                                if (!line.Contains('%')) {
                                    inputContent += line + "\r\n";
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("The file could not be read");
                    }
                }
            }
            else
            {
                int i = 0;
                for (i = 0; i < connectedReplicas.Count; i++)
                {
                    if (connectedReplicas[i].getHashing() == true)
                    {
                        hasHashing();
                        break;
                    }
                    else
                    {
                        inputContent = connectedReplicas[i].getInput(); 
                        if (inputContent != null)
                        {
                            break;
                        }
                        if (i == connectedReplicas.Count)
                        {
                            i = 0;
                        }
                    }
                }
            }
        }

        public void hasHashing()
        {
            while (inputContent == null)
            {
                foreach (var element in connectedReplicas) 
                {
                    if(element.getInput()!="NO_RESULT")
                        inputContent += element.getInput();
                }
            }
        }

        public bool getHashing()
        {
            return gotHashing;
        }

        public string getInputContent()
        {
            return computation;
        }

        public void checkIfHashing()
        {   
            if (routing.Contains("hashing"))
            {
                gotHashing = true;
                string[] parse = routing.Split('(');
                string[] parse1 = parse[1].Split(')');
                int f_number = Int32.Parse(parse1[0]);
                string[] fileLines = inputContent.Split('\n');
                fileLines = fileLines.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                inputContent = "";
                fileLines = fileLines.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                foreach (string s in fileLines)
                {
                    string[] line = s.Split(',');
                    int hash = line[f_number - 1].GetHashCode();
                    if (hash != 0 && s!="")
                    {
                        int replicaID = Math.Abs(hash % (Int32.Parse(rep_fact)));
                        if (!name.Contains('R'))
                        {
                            if (replicaID == 0)
                            {
                                inputContent+=s+"\r\n";
                            }
                        }
                        else
                        {
                            int repNum = Int32.Parse(name[name.Length - 1].ToString());
                            if (repNum == replicaID)
                            {
                                inputContent += s + "\r\n";
                            }
                        }
                    }
                }
                if (inputContent == "")
                {
                    inputContent = "NO_RESULT";
                }
            }
        }

        public void processCommand(string command)
        {
            string result;
            if (isFrozen == true)
            { 
                freezeInput.Add(command);
                return;
            }

            if (command.Contains("Start"))
            {
                if (inputContent == null)
                    checkInput();
                checkIfHashing();          
                result = processOperator(); 
                Console.WriteLine("operator processed, result: " + result);
                if (loggingLevel == "full")
                {
                    logToMaster();
                }
            }

            if (command.Contains("Interval"))
            {
                string[] words = command.Split(' ');
                if (loggingLevel == "full")
                {
                    answer = "Wait " + words[1] + " ms...";
                    logToMaster();
                }
                Console.WriteLine("Wait {0} ms...", words[1]);
                Thread.Sleep(Int32.Parse(words[1]));
                Console.WriteLine("Waited {0} ms...", words[1]);
                //computation = "INTERVAL";
            }

            if (command == "Status")
            {
                Console.WriteLine("");
                Console.WriteLine("############# STATUS #############");
                Console.WriteLine("name: " + name);
                Console.WriteLine("previous command result: " + "\r\n"+computation);
                Console.WriteLine("");
               
            }

        }

        public void executeList()
        {
            foreach(string s in freezeInput)
            {
                processCommand(s);
            }
            freezeInput.Clear();
        }

        public void logToMaster()
        {
            if (loggingLevel == "full")
            {
                ThreadStart td = new ThreadStart(sendMsgToPuppet);
                Thread t_d = new Thread(td);
                t_d.Start();
            }
        }

        public void sendMsgToPuppet()
        {
            var date = DateTime.Now;
            remotePM = (PuppetMasterInterface)Activator.GetObject(typeof(PuppetMasterInterface),"tcp://localhost:10000/RemotePMService");
            previousLog = "OPERATOR " + name + "->" + date.Hour + "h:" + date.Minute + "m:" + date.Second + "s" + "\r\n" + "RESULT: " + computation;
            remotePM.sendMsgToMaster(previousLog);
        }
        

        //-----------------------------------------------------------FUNCTIONS-----------------------------------------------------------------//

        public string processOperator()
        {
            operation = operator_spec.Split(',');
            if (inputContent == "NO_RESULT")
            {
                computation = inputContent;
                return computation;
            }
            if (operation.Length > 0)
            {
                switch (operation[0].Split('_')[0])
                {
                    case "UNIQ":
                        Console.WriteLine("I'm UNIQ");
                        computation = UNIQ(operation[0].Split('_')[1]); 
                        break;
                    case "COUNT":
                        Console.WriteLine("I'm COUNT");
                        computation = COUNT(inputContent).ToString();
                        break;
                    case "DUP":
                        Console.WriteLine("I'm DUP");
                        computation = DUP(inputContent);
                        break;
                    case "FILTER":
                        Console.WriteLine("I'm FILTER");
                        computation = FILTER(operator_spec.Split('_')[1]); 
                        break;
                    case "CUSTOM":
                        var fileName = "../../../myLib/bin/Debug/" + operation[0].Split('_')[1];
                        var className = operation[1];
                        var methodName = operation[2];

                        Console.WriteLine("I'm " + operation[0].Split('_')[0]);
                        Console.WriteLine("File " + fileName);
                        Console.WriteLine("Class " + className);
                        Console.WriteLine("Method " + methodName);

                        byte[] code = File.ReadAllBytes(fileName);
                        computation = CUSTOM(code, className, methodName);
                        break;
                    default:
                        Console.WriteLine("I'm NOTHING");
                        break;
                }
            }
            lastone(computation);
            return computation;
        }

        public string DUP(string vasco)
        {
            Console.WriteLine("Result of DUP: "+vasco+"\r\n");
            return vasco;
        }

        public int COUNT(string rui)
        {
            int count = 0;
            int start = 0;
            while ((start = rui.IndexOf('\n', start)) != -1)
            {
                count++;
                start++;
            }
            return count;
        }

        public string UNIQ(string field_number)
        {
            
            int num = Int32.Parse(field_number);
            string input = inputContent;
            //Console.WriteLine("O INPUT CONTENT É: "+inputContent);
            string[] tuples = input.Split('\n');
            if (tuples.Length == 2)
            {
                answer = inputContent;
                return answer;
            }
            tuples = tuples.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            int numberOfCommas = tuples[0].Split(',').Length;
            int j = 1;
            while (j <= numberOfCommas)
            {
                List<string> lst = new List<string>();
                uniqDictionary.Add(j,lst);
                j++;
            }
            string[] parts;
            for (int i = 0; i < tuples.Length; i++)
            {
                parts = tuples[i].Split(',');
                for (int l = 1; l <= parts.Length; l++)
                {
                    List<string> tup = uniqDictionary[l];
                    tup.Add(parts[l - 1]);
                    uniqDictionary[l] = tup;
                }
            }
            List<string> lista=uniqDictionary[Int32.Parse(field_number)];
            List<int> tuplesToBeTaken=new List<int>();
            List<string> lista2 = new List<string>();
            for (j= 0; j < lista.Count; j++)
            {
                if (lista2 != null)
                {
                    if (!lista2.Contains(lista[j]))
                    {
                        lista2.Add(lista[j]);
                        answer += tuples[j] + "\r\n";
                    }
                    else
                    {
                        tuplesToBeTaken.Add(j);
                    }
                }
                else
                    lista2.Add(lista[j]);
            }
            uniqDictionary.Clear();
            return answer;
        }
        

        public string FILTER(string info)
        {
            int field_number;
            string condition;
            string value;
            string[] valueDivider;
            answer="";
            field_number = Int32.Parse(info.Split(',')[0]);
            condition = info.Split(',')[1];
            value = info.Split(',')[2];
            valueDivider = value.Split('.');
            var reader = new StringReader(inputContent);
            string line = reader.ReadLine();
            condition.Trim();
            switch (condition)
            {
                case ("="):
                    while (line != null)
                    {
                        if (line !="" && line.Split(',')[field_number-1].Trim().Contains(value))
                        {
                            answer += line+"\r\n";
                        }
                        line = reader.ReadLine();
                    }
                    break;

                case (">"):
                    while (line != null)
                    {
                        if (line != "" && line[0] != '%')
                        {   
                            string[] divider;
                            string[] divider2;
                            divider = line.Split(',');
                            divider2 = divider[2].Split('.');
                            int comparison = valueDivider[1].CompareTo(divider2[1]);
                            if (comparison == -1)
                            {
                                answer += line + "\r\n";
                            }
                        }
                        line = reader.ReadLine();
                    }
                    break;
                case ("<"):
                    while (line != null)
                    {
                        if (line != "" && line[0] != '%')
                        {
                            string[] divider;
                            string[] divider2;
                            divider = line.Split(',');
                            divider2 = divider[2].Split('.');
                            int comparison = valueDivider[1].CompareTo(divider2[1]);
                            if (comparison == 1)
                            {
                                answer += line + "\r\n";
                            }
                        }
                        line = reader.ReadLine();
                    }
                    break;
            }
            if (answer == "")
                answer = "NO_RESULT";
            return answer;
        }
        public string CUSTOM(byte[] code, string className, string methodName)
        {
            string customResult = "";
            Assembly assembly = Assembly.Load(code);
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsClass == true)
                {
                    if (type.FullName.EndsWith("." + className))
                    {
                        object ClassObj = Activator.CreateInstance(type);
                        string[] input = inputContent.Split('\n');
                        object[] args = new object[] { input };
                        object resultObject = type.InvokeMember(methodName,
                          BindingFlags.Default | BindingFlags.InvokeMethod,
                               null,
                               ClassObj,
                               args);
                        IList<IList<string>> result = (IList<IList<string>>)resultObject;
                        foreach (IList<string> tuple in result)
                        {
                            foreach (string s in tuple)
                            {   
                                if(s!="")
                                    customResult += s + "\r\n";
                            }
                        }
                        return customResult;
                    }
                }
            }
            throw (new System.Exception("could not invoke method"));
        }

        public void lastone(string li)
        {
            if (imlast == "true")
            {
                string mydocpath = "../../../";
                using (StreamWriter outputFile = new StreamWriter(mydocpath + @"/OutputFnal.txt", true))
                {
                    outputFile.WriteLine(li);
                }

            }

        }

        //----------------------------------------------------------------------------------------------------------------------------------------//

        static void Main(string[] args)
        {
            Console.WriteLine("--------------OPERATOR-------------");
            Console.WriteLine("");
            Console.WriteLine("name: " + args[0]);
            Console.WriteLine("input_ops: " + args[1]);
            Console.WriteLine("rep_fact: " + args[2]);
            Console.WriteLine("routing: " + args[3]);
            Console.WriteLine("address: " + args[4]);
            Console.WriteLine("operator_spec: " + args[5]);
            Console.WriteLine("loggingLevel: " + args[6]);
            Console.WriteLine("semantics: " + args[7]);
            Console.WriteLine("previous rep fact: " + args[8]);
            Console.WriteLine("");

            Operator operador = new Operator();
            operador.name = args[0];
            operador.input_ops = args[1];
            operador.rep_fact = args[2];
            operador.routing = args[3];
            operador.address = args[4];
            operador.operator_spec = args[5];
            operador.loggingLevel = args[6];
            operador.semantics = args[7];
            operador.previousRepFact = args[8];
            operador.imlast = args[9];
            operador.MyInterfaceRegistery();
            operador.PreviousOpInterfaceConnection();
            Console.ReadKey();
        }
    }
    public class RemoteOPService : MarshalByRefObject, OperatorInterface
    {
        public DelInputChecker inputCheck;
        public DelProcessCommand processCommand;
        public DellKillCommand crashProcess;
        public DelFreezeCommand setFreezeMode;
        public DelUnfreezeCommand setUnreezeMode;
        public DelExecuteList executeList;
        public DelHashingChecker hashCheck;

        public string getInput() { return inputCheck(); }
        public bool getHashing() { return hashCheck(); }
        public void checkCommand(string command) { processCommand(command); }
        public void ProcessCrash() { crashProcess(); }
        public void ProcessFreeze() { setFreezeMode(); } 
        public void ProcessUnreeze() { setUnreezeMode(); }
        public void ProcessList() { executeList(); }
    }
}
