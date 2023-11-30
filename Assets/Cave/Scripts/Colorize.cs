public class Colorize
{
    public static Colorize RED = new Colorize("red");
    public static Colorize YELLOW = new Colorize("yellow");
    public static Colorize GREEN = new Colorize("green");
    public static Colorize BLUE = new Colorize("blue");
    public static Colorize CYAN = new Colorize("cyan");
    public static Colorize WHITE = new Colorize("white");
    public static Colorize LIME = new Colorize("lime");
    public static Colorize MAGENTA = new Colorize("magenta");


    public static Colorize LOG_DEFAULT = new Colorize("white");
    public static Colorize LOG_WARNING = new Colorize("yellow");
    public static Colorize LOG_ERROR = new Colorize("red");

    private readonly string _prefix;
    private const string _suffix = "</color>";

    private Colorize(string color)
    {
        this._prefix = "<color=" + color + ">";
    }

    public static string operator %(string text, Colorize color)
    {
#if UNITY_EDITOR
        return color._prefix + text + _suffix;
#else
        return text;
#endif
    }

    // Usage example
    //Debug.Log($"I'm a RED message" % Colorize.RED);
}
