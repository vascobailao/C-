using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibService
{
    public interface ProcessCreationInterface
    {
        void OperatorsCreator(List<ConfigFileLine> lista,int mode);
    }

    public interface PuppetMasterInterface
    {
        void sendMsgToMaster(string msg);
    }

    public interface OperatorInterface
    {
        string getInput();
        bool getHashing();
        void checkCommand(string command);
        void ProcessCrash();
        void ProcessFreeze();
        void ProcessUnreeze();
        void ProcessList();

    }

    [Serializable]
    public class ConfigFileLine
    {
        public string name;
        public string input_ops;
        public string rep_fact;
        public string routing;
        public List<string> address;
        public string operator_spec;
        public string semantics;
        public string loggingLevel;
        public string previousRepFact = "only_one";
        public string imlast="false";
    }

    public delegate void DelAddMsgToLog(string msg);
    public delegate void DelExecutaScprit(string name, int mode);
    public delegate void DelCreateOperator(List<ConfigFileLine> lista,int mode);
    public delegate string DelInputChecker();
    public delegate bool DelHashingChecker();
    public delegate void DelProcessCommand(string command);
    public delegate void DellKillCommand();
    public delegate void DelUnfreezeCommand();
    public delegate void DelFreezeCommand();
    public delegate void DelExecuteList();
}
