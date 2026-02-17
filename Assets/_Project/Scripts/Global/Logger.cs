using UnityEngine;

namespace StormPig.Global {
    public class Logger : MonoBehaviour {
        private void Awake() {
            Log.Create(new LogConsole());
        }
    }
}