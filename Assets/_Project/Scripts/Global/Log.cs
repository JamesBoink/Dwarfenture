namespace StormPig.Global{
    public class Log {

        private static Log instance = new Log();
        private static Log Instance => instance;

        public static void Create(Log logger) {
            instance = logger;
        }


        public enum Type {
            Trace,
            Debug,
            Warning,
            Error,
            Critical
        }

        public static void Trace(string text) {
#if LOG_TRACE
            Instance.Write(text, Type.Trace);
#endif
        }
        public static void Debug(string text) {
#if LOG_TRACE || LOG_DEBUG
            Instance.Write(text, Type.Debug);
#endif
        }
        public static void Warning(string text) {
#if LOG_TRACE || LOG_DEBUG || LOG_WARNING
            Instance.Write(text, Type.Warning);
#endif
        }
        public static void Error(string text) {
#if LOG_TRACE || LOG_DEBUG || LOG_WARNING || LOG_ERROR
            Instance.Write(text, Type.Error);
#endif
        }
        public static void Critical(string text) {
#if LOG_TRACE || LOG_DEBUG || LOG_WARNING || LOG_ERROR || LOG_CRITICAL
            Instance.Write(text, Type.Critical);
#endif
        }

        protected virtual void Write(string text, Type type) { }
    }


    public class LogConsole : Log {
        protected override void Write(string text, Type type) {
#if DEBUG
            text = text.Replace("[/c]", "</color>").Replace("[c:", "<color=");
            text = text.Replace("[/b]", "</b>").Replace("[b]", "<b>");
            text = text.Replace("[/i]", "</i>").Replace("[i]", "<i>");
            text = text.Replace("]", ">");

            switch (type) {
                case Type.Trace:
                    text = text.Insert(0, "<color=#ffffffff>[Trace]:</color> ");
                    break;
                case Type.Debug:
                    text = text.Insert(0, "<color=#add8e6ff>[Debug]:</color> ");
                    break;
                case Type.Warning:
                    text = text.Insert(0, "<color=#ffa500ff>[Warning]:</color> ");
                    break;
                case Type.Error:
                    text = text.Insert(0, "<b><color=#ff0000ff>[Error]:</color></b> ");
                    break;
                case Type.Critical:
                    text = text.Insert(0, "<b><color=#140000ff>[Critical]:</color></b> ");
                    break;
            }

#if LOG_DEBUG_ERROR
            UnityEngine.Debug.LogError(text);
#elif UNITY_EDITOR || UNITY_GAMECORE
            if (type >= Type.Error) {
                UnityEngine.Debug.LogError(text);
            } else if (type == Type.Warning) {
                UnityEngine.Debug.LogWarning(text);
            } else {
                UnityEngine.Debug.Log(text);
            }
#else
            System.Console.WriteLine(text);
#endif
#endif
        }
    }
}
