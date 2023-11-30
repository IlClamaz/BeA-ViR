using UnityEngine;
using System.IO;

namespace Beavir.Businesslogic.Utilities
{
    public class WriteDebugToFile : MonoBehaviour
    {
        private string filename = "User";

        private void OnEnable()
        {
            Application.logMessageReceived += Log;
        }
        // Start is called before the first frame update
        void Start()
        {
            filename = Application.dataPath + "/Logs/" + filename + "_" + (Random.Range(0, 1000)) + ".csv";
        }

        public void Log(string logString, string stackTrace, LogType type)
        {
            TextWriter tw = new StreamWriter(filename, true);

            tw.WriteLine(logString);

            tw.Close();
        }
    }
}
