using Xamarin.Forms;

namespace ScoutingAppBase.Pages
{
  public sealed class SettingsPage : ContentPage
  {
    public SettingsPage()
    {
      var newEventButton = new Button
      {
        Text = "New Event"
      };
      Content = new StackLayout
      {
        Children = {
          newEventButton
        }
      };

      newEventButton.Clicked += async (sender, e) =>
      {
        bool confirm = await DisplayAlert(
          "Create new event",
          "Are you sure you want to create a new event? This will delete the current event data.",
          "Yes",
          "Cancel");
        if (confirm)
        {
          // todo create new event and stuff
        }
      };
    }
  }
}