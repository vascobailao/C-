using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using SharedLibService;

namespace DADSTORM
{
    class Pcs
    {
        public TcpChannel channel;
        private List<Process> operatorProcesses;
        public ProcessCreationService remotePCS;
        List<ProcessStartInfo> infos;
        int i;

        public Pcs()
        {
            infos = new List<ProcessStartInfo>();
            //--------------------------------PM Interface creation-----------------------------------//
            remotePCS = new ProcessCreationService();
            channel = new TcpChannel(10001);

            ChannelServices.RegisterChannel(channel, false);
            RemotingServices.Marshal(remotePCS, "remotePCS",
                typeof(ProcessCreationService));

            //---------------------------------------------------------------------------------------//

            operatorProcesses = new List<Process>();
            remotePCS.createOperators += new DelCreateOperator(ProcessCreator);      

        }

        public void ProcessCreator(List<ConfigFileLine> lista, int mode)
        {
           
            while(i < lista.Count)
            {
                var item = lista[i];
                if (i == lista.Count-1)
                {
                    item.imlast = "true";
                }

                Console.WriteLine("--------------LINE-------------");
                Console.WriteLine("");
                foreach (var element in item.address)
                {
                    Console.WriteLine("address: " + element);
                }
                Console.WriteLine("imput_ops: " + item.input_ops);
                Console.WriteLine("name: " + item.name);
                Console.WriteLine("operator_spec: " + item.operator_spec);
                Console.WriteLine("rep_fact: " + item.rep_fact);
                Console.WriteLine("routing: " + item.routing);
                Console.WriteLine("semantics: " + item.semantics);
                Console.WriteLine("loggingLevel: " + item.loggingLevel);
                Console.WriteLine("next address: " + item.previousRepFact);
                Console.WriteLine("Am I the last one?" + item.imlast);
                Console.WriteLine("");

                string endereços = string.Join("", item.address.ToArray());
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = "..\\..\\..\\Operator\\bin\\Debug\\Operator.exe";
                
                info.Arguments = item.name + " " + item.input_ops + " " + item.rep_fact + " " + item.routing + " " + endereços + " " + item.operator_spec + " " + item.loggingLevel + " " + item.semantics + " " + item.previousRepFact +" "+ item.imlast;
                if (mode == 1)
                {
                    startProcess(info);
                    i += 1;
                    break;
                }
                infos.Add(info);

                int n = Int32.Parse(item.rep_fact);
                i++;
            }
            if (mode == 0)
                startProcesses(infos);
            
        }

        public void startProcess(ProcessStartInfo info)
        {
            Process.Start(info);
        }

        public void startProcesses(List<ProcessStartInfo> infos)
        {
            foreach (var info in infos)
            {
                Process p = Process.Start(info);
                operatorProcesses.Add(p);
            }
        }

        static void Main(string[] args)
        {
            Pcs pcs = new Pcs();
            Console.ReadKey();
        }

        public class ProcessCreationService : MarshalByRefObject, ProcessCreationInterface
        {
            public DelCreateOperator createOperators;

            public ProcessCreationService() {}
            public void OperatorsCreator(List<ConfigFileLine> lista, int mode) { createOperators(lista,mode); }
        }
    }
}
