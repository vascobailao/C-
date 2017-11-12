using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharedLibService;
using System.Threading;

namespace DADSTORM
{
    class PuppetMaster
    {
        public MasterForm form;
        public List<ConfigFileLine> pcs;
        public TcpChannel channel;
        ProcessCreationInterface server;
        RemotePMService remotePMservice;
        public List<ConfigFileLine> primaryReplicas;
        public List<ConfigFileLine> replicas;
        public List<string> crashList;
        //List<OperatorInterface> connectedOperators;
        public Dictionary<string,List<ConfigFileLine>> opReplicas; //tem as replicas existentes sem contar com a primaria
        public Dictionary<string, OperatorInterface> connectedOperators;
        string previousCommand;
        public string opToStop;
        public string timeToStop;
        List<string> configCommands;
        Random rnd;
        int j = 0;
        int flagFileParser=0;

        public PuppetMaster()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //--------------------------------PM Interface creation-----------------------------------//

            remotePMservice = new RemotePMService();
            
            channel = new TcpChannel(10000);

            ChannelServices.RegisterChannel(channel, false);
            RemotingServices.Marshal(remotePMservice, "RemotePMService",
                typeof(RemotePMService));

            //--------------------------------PCS Interface conection-----------------------------------//

            server = (ProcessCreationInterface)Activator.GetObject(typeof(ProcessCreationInterface),
                   "tcp://localhost:10001/remotePCS");
         
 
            //-----------------------------------------------------------------------------------------//

            form = new MasterForm();
            pcs = new List<ConfigFileLine>();
            crashList = new List<string>();
            rnd = new Random();
            primaryReplicas = new List<ConfigFileLine>();
            configCommands = new List<string>();
            connectedOperators = new Dictionary<string, OperatorInterface>();
            opReplicas = new Dictionary<string, List<ConfigFileLine>>();

            form.formDelegateAddToLog += new DelAddMsgToLog(addMsgToLog);
            form.formDelegateExecutaScript += new DelExecutaScprit(executaScriptConfig);
            form.sendCommand += new DelAddMsgToLog(CommandReceived);
            remotePMservice.remoteDelegateAddMsgToLog += new DelAddMsgToLog(addMsgToLog);
        }

        [STAThread]
        static void Main()
        {
            PuppetMaster pmaster = new PuppetMaster();
            Application.Run(pmaster.form);
        }

        public void addMsgToLog(string message)
        {
            form.Invoke(new DelAddMsgToLog(form.sendMessageToLog), message);
        }


        public void executaScriptConfig(string name,int mode)
        {
            if (mode == 0)
            {
                FileParser(name);
                server.OperatorsCreator(pcs, mode);
                if (configCommands != null)
                {
                    foreach (var c in configCommands)
                    {
                        CommandReceived(c); //PASSAR ISTO PARA RECEBER LISTA DE COMANDOS!!!!!!!!!!!!!!!
                    }
                }
            }
            if (mode == 1)
            {
                if (flagFileParser == 0) { 
                    FileParser(name);
                    flagFileParser = 1;
                }
                while (j < pcs.Count)
                {
                    server.OperatorsCreator(pcs, mode);
                    j++;
                    break;
                }
                if (j == pcs.Count)
                {
                    int i = 0;
                    if (configCommands != null)
                    {
                        foreach (var c in configCommands)
                        {
                            CommandReceived(c);
                            configCommands.Remove(c);
                            break;
                        }
                    }
                }
            }
        }

        public void FileParser(string name)
        {
            try
            {
                string[] fileLines = File.ReadAllLines("../../../" + name);
                string[] words;
                int operator_index = 0;
                string semantics = "";
                string loggingLevel = "";
                for (int i = 0; i < fileLines.Length; i++)
                {
                    words = fileLines[i].Split();
                    if (fileLines[i].Contains("Semantics"))
                    {
                        semantics = words[1];
                    }
                    if (fileLines[i].Contains("LoggingLevel"))
                    {
                        loggingLevel = words[1];
                    }

                    if (fileLines[i].Contains("input ops") && !fileLines[i].Contains("%") && !fileLines[i].Contains("%%"))
                    {
                        ConfigFileLine op = new ConfigFileLine();
                        op.name = words[0];
                        op.input_ops = words[3];
          
                        op.rep_fact = words[6];
                        op.routing = words[8];
                        op.address = new List<string>();
                        op.semantics = semantics;
                        op.loggingLevel = loggingLevel; 
                        if (words[10].Contains(','))
                        {
                            int num = 10;
                            while (num < words.Length)
                            {
                                if (words[num].Equals("operator"))
                                {
                                    operator_index = num;
                                    break;
                                }
                                op.address.Add(words[num]);
                                num++;
                            }
                            op.operator_spec = words[operator_index + 2] + "_" + words[operator_index + 3];
                        }
                        else
                        {
                            op.address.Add(words[10]);
                            int num = 13;
                            while (num < words.Length)
                            {
                                op.operator_spec += words[num] + " ";
                                num++;
                            }
                        }
                        pcs.Add(op);
                        primaryReplicas.Add(op);
                        for (var j = 1; j < pcs.Count; j++)
                        {
                            if (pcs[j - 1].rep_fact!=null)
                            {
                                pcs[j].previousRepFact = pcs[j - 1].rep_fact;
                            }
                        }
                        checkReplicas(op); //checka a existencia de replicas
                    }
                    if (fileLines[i].Contains("Start") && !fileLines[i].Contains("%") && !fileLines[i].Contains("%%"))
                    {
                        string comando = string.Join(" ", words);
                        configCommands.Add(comando);
                    }
                    if (fileLines[i].Contains("Interval") && !fileLines[i].Contains("%") && !fileLines[i].Contains("%%"))
                    {
                        string comando = string.Join(" ", words);
                        configCommands.Add(comando);
                    }
                    if (fileLines[i].Contains("Status") && !fileLines[i].Contains("%") && !fileLines[i].Contains("%%"))
                    {
                        string comando = string.Join(" ", words);
                        configCommands.Add(comando);
                    }
                    if (fileLines[i].Contains("Wait") && !fileLines[i].Contains("%") && !fileLines[i].Contains("%%"))
                    {
                        string comando = string.Join(" ", words);
                        configCommands.Add(comando);
                    }
                    if (fileLines[i].Contains("Freeze") && !fileLines[i].Contains("%") && !fileLines[i].Contains("%%"))
                    {
                        string comando = string.Join(" ", words);
                        configCommands.Add(comando);
                    }
                    if (fileLines[i].Contains("Unfreeze") && !fileLines[i].Contains("%") && !fileLines[i].Contains("%%"))
                    {
                        string comando = string.Join(" ", words);
                        configCommands.Add(comando);
                    }
                    if (fileLines[i].Contains("Crash") && !fileLines[i].Contains("%") && !fileLines[i].Contains("%%"))
                    {
                        string comando = string.Join(" ", words);
                        configCommands.Add(comando);
                    }
                }
            }
            catch (Exception)
            {
                addMsgToLog("Invalid File!");
            }
        }

        public void checkReplicas(ConfigFileLine op)
        {
            int m = 1;
            int n = Int32.Parse(op.rep_fact);
            if (n > 1)
            {
                replicas = new List<ConfigFileLine>();
                while (m < n)
                {
                    ConfigFileLine replica = new ConfigFileLine();
                    replica.name = op.name + "_R" + (m);
                    replica.operator_spec = op.operator_spec;
                    replica.input_ops = op.input_ops;
                    replica.rep_fact = op.rep_fact;
                    replica.routing = op.routing;
                    replica.address = op.address;
                    replica.semantics = op.semantics;
                    replica.loggingLevel = op.loggingLevel;
                    replica.previousRepFact = op.previousRepFact;
                    replicas.Add(replica);
                    pcs.Add(replica); //lista para o pcs criar
                    m++;
                }
                opReplicas.Add(op.name, replicas); 
            }
        }

        public void CommandReceived(string command)  
        {
            string[] words = command.Split(' ');
            int count = 0;
            if (words[0] == "Wait")
            {
                var date = DateTime.Now;
                addMsgToLog(date.Hour + "h:" + date.Minute + "m:" + date.Second + "s" + ": " + "PuppetMaster will wait " + words[1] + " ms");
                Thread.Sleep(Int32.Parse(words[1]));
                date = DateTime.Now;
                addMsgToLog(date.Hour + "h:" + date.Minute + "m:" + date.Second + "s" + ": " + "PuppetMaster waited " + words[1] + " ms!");

            }
            if ((words[0] == "Status"))
            {   
                foreach(var i in pcs)
                {
                    if (!crashList.Contains(i.name))
                    {
                        OperatorInterface l = connectToOP(i.name);
                        l.checkCommand(words[0]);
                    }
                }
            }
            foreach (ConfigFileLine op in primaryReplicas)  
            {
                if (words[0] == "Crash")
                {
                    try
                    {
                        if (words[2] == "0")
                        {
                            OperatorInterface remoteOP = connectToOP(words[1]);
                            connectedOperators.Remove(words[1]);
                            if (op.name == words[1])
                                primaryReplicas.Remove(op);
                            crashList.Add(op.name);
                            remoteOP.ProcessCrash();
                            break;
                        }
                        if (words[2] != "0")
                        {
                            List<ConfigFileLine> lista = new List<ConfigFileLine>();
                            bool hasValue = opReplicas.TryGetValue(words[1], out lista);
                            if (hasValue)
                            {
                                for (int i = 0; i < lista.Count; i++)
                                {
                                    if (Int32.Parse(words[2]) - 1 == i)
                                    {
                                        OperatorInterface remoteOP = connectToOP(lista[i].name);
                                        connectedOperators.Remove(lista[i].name);
                                        crashList.Add(lista[i].name);
                                        remoteOP.ProcessCrash();
                                        break;
                                    }
                                }
                            }
                        }

                    }
                    catch (Exception)
                    {
                        break;
                    }

                }
                if (words[0] == "Freeze")
                {
                    if (words[2] == "0")
                    {
                        OperatorInterface remoteOP = connectToOP(words[1]);
                        remoteOP.ProcessFreeze();
                        break;
                    }
                    if (words[2] != "0")
                    {
                        List<ConfigFileLine> lista = new List<ConfigFileLine>();
                        bool hasValue = opReplicas.TryGetValue(words[1], out lista);
                        if (hasValue)
                        {
                            for (int i = 0; i < lista.Count; i++)
                            {
                                if (Int32.Parse(words[2]) - 1 == i)
                                {
                                    OperatorInterface remoteOP = connectToOP(lista[i].name);
                                    remoteOP.ProcessFreeze();
                                    break;
                                }
                            }
                        }

                        break;
                    }
                }
                if (words[0] == "Unfreeze")
                {
                    if (words[2] == "0")
                    {
                        OperatorInterface remoteOP = connectToOP(words[1]);
                        remoteOP.ProcessUnreeze();
                        try
                        {
                            remoteOP.ProcessList();

                        }
                        catch (Exception e) //util para quando a lista do outro lado esta vazia
                        {}
                    }
                    if (words[2] != "0")
                    {
                        List<ConfigFileLine> lista = new List<ConfigFileLine>();
                        bool hasValue = opReplicas.TryGetValue(words[1], out lista);
                        if (hasValue)
                        {
                            for (int i = 0; i < lista.Count; i++)
                            {
                                if (Int32.Parse(words[2]) - 1 == i)
                                {
                                    OperatorInterface remoteOP = connectToOP(lista[i].name);
                                    remoteOP.ProcessUnreeze();

                                    try
                                    {
                                        remoteOP.ProcessList();

                                    }
                                    catch (Exception e)
                                    {}
                                }
                            }
                        }
                    }

                    break;
                }
                if ((words[0] == "Start"))
                {
                    if (op.name == words[1] && (op.input_ops == previousCommand || op.input_ops.Contains(".dat"))) //previousCommand verifica que o start so funciona depois do start do op anterior
                    {
                        if (op.routing == "random")
                        {
                            List<ConfigFileLine> value;  
                            bool hasValue = opReplicas.TryGetValue(op.name, out value);
                            if (hasValue)
                            {
                                int number = rnd.Next(0, value.Count + 1);
                                if (number == 0)
                                {
                                    OperatorInterface remoteOperator = connectToOP(op.name);
                                    remoteOperator.checkCommand(words[0]);
                                    previousCommand = words[1];
                                    break;
                                }
                                else
                                {
                                    for (int i = 0; i < value.Count; i++)
                                    {
                                        if (number - 1 == i)
                                        {
                                            OperatorInterface remoteOperator = connectToOP(value[i].name);
                                            remoteOperator.checkCommand(words[0]);
                                            previousCommand = words[1];
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        if (op.routing == "primary")
                        {
                            OperatorInterface remoteOperator = connectToOP(op.name);
                            remoteOperator.checkCommand(words[0]);
                            previousCommand = words[1];
                            break;
                        }
                        if (op.routing.Contains("hashing"))
                        {
                            List<ConfigFileLine> value;
                            OperatorInterface remoteOperator = connectToOP(op.name);
                            remoteOperator.checkCommand(words[0]);
                            bool hasValue = opReplicas.TryGetValue(op.name, out value);
                            if (hasValue)
                            {
                                foreach (var element in value)
                                {
                                    remoteOperator = connectToOP(element.name);
                                    remoteOperator.checkCommand(words[0]);
                                    previousCommand = words[1];
                                }
                            }

                        }
                    }
                    else
                    {
                        count++;
                        if (count == primaryReplicas.Count)
                            addMsgToLog("wrong operator");
                    }
                }

                if (words[0] == "Interval" && op.name == words[1]) //THREAD AQUI
                {
                    
                    //timeToStop = words[2];
                    List<ConfigFileLine> value;
                    
                    bool hasValue = opReplicas.TryGetValue(words[1], out value);
                    if (hasValue)
                    {
                        foreach(var element in value)
                        {
                            Task.Factory.StartNew(() => stopReplicas(element.name,words[2]));          
                        }
                    }
                    OperatorInterface remoteOperator = connectToOP(op.name);
                    remoteOperator.checkCommand(words[0] + " " + words[2]);
                }
            }
        }

        public void stopReplicas(string name,string time)
        { 
            OperatorInterface remoteReplica = connectToOP(name);
            remoteReplica.checkCommand("Interval" + " " + time);
        }

        public OperatorInterface connectToOP(String name)
        {
            int port;
            if (name.Contains('R'))
            {
                port = 8080 + Int32.Parse((name[2]).ToString()) + Int32.Parse(name[name.Length - 1].ToString()) * 100;
            }
            else
            {
                port = 8080 + Int32.Parse((name[2]).ToString());
            }
            OperatorInterface remoteOperator = (OperatorInterface)Activator.GetObject(typeof(OperatorInterface), "tcp://localhost:" + port + "/op");
            if (!connectedOperators.Keys.Contains(name))
                connectedOperators.Add(name, remoteOperator);
            return remoteOperator;
        }
    }

    public class RemotePMService : MarshalByRefObject, PuppetMasterInterface
    {
        public DelAddMsgToLog remoteDelegateAddMsgToLog;

        public RemotePMService() {}

        public void sendMsgToMaster(string msg) { remoteDelegateAddMsgToLog(msg); }
    }
}
