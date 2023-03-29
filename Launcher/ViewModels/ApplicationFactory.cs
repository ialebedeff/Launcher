namespace Launcher.ViewModels
{
    internal class ApplicationFactory
    { 
        public static ApplicationViewModel Create(string name, string execution, string path)
            => new ApplicationViewModel(name, execution, path);
    }
}
