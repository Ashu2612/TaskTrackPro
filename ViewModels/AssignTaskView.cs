namespace TaskTrackPro.ViewModels;

public class AssignTaskView : ContentView
{
    public AssignTaskView()
    {
        Content = new VerticalStackLayout
        {
            Children = {
                new Label { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, Text = "Welcome to .NET MAUI!"
                }
            }
        };
    }
}