namespace Eleks.Demo
{
    public interface IDirService
    {
        string[] GetFiles(string path);
        bool DirectoryExists(string path);
        string[] GetDirectories(string path);
    }
}