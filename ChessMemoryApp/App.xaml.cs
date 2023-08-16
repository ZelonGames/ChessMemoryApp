using System.ComponentModel;

namespace ChessMemoryApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        

        Microsoft.Maui.Handlers.ContentViewHandler.Mapper.AppendToMapping("UseLayoutRounding ", (handler, view) =>
        {
        });


        MainPage = new AppShell();
    }

}
