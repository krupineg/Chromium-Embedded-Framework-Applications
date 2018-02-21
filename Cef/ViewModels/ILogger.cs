namespace Cef
{
    public interface ILogger
    {
        void ToggleType(string type);
        void EnableType(string type);
        void DisableType(string type);
        void Info(string info, string type = "");
    }
}