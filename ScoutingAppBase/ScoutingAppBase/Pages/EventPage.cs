using System.Collections.Generic;

using Xamarin.Forms;

using ScoutingAppBase.Data;

namespace ScoutingAppBase.Pages
{
  public sealed class EventPage : ContentPage
  {
    public EventPage(EventData eventData)
    {
      EventData = eventData;
      MatchViews = new Dictionary<Button, MatchData>();
      var content = new StackLayout();
      Content = content;

      var syncButton = new Button { Text = "Sync" };
      var newMatchButton = new Button { Text = "+" };
      var settingsButton = new Button { Text = "Settings" };

      NavigationPage.SetTitleView(
        this, new StackLayout
        {
          Orientation = StackOrientation.Horizontal,
          Children =
          {
            new Label
            {
              Text = eventData.Config.EventName
            },
            syncButton,
            newMatchButton,
            settingsButton
          }
        });

      var matchesLayout = new StackLayout();

      content.Children.Add(newMatchButton);
      content.Children.Add(matchesLayout);

      newMatchButton.Clicked += (sender, e) =>
      {
        // Its name starts off as its index in the list of matches
        var matchId = $"{EventData.Matches.Count}";
        var match = new MatchData { Id = matchId };
        EventData.Matches.Add(match);

        // Add a button on the event page for going to the match
        var matchButton = new Button { Text = matchId };
        matchesLayout.Children.Add(matchButton);

        GoToMatch(match);
      };

      syncButton.Clicked += (sender, e) =>
      {
        // todo sync with server
      };

      settingsButton.Clicked += (sender, e) =>
      {
        Navigation.PushModalAsync(new SettingsPage());
      };
    }

    private readonly EventData EventData;

    /// <summary>
    /// Used for mapping buttons to go to a match with that match's data
    /// </summary>
    private readonly Dictionary<Button, MatchData> MatchViews;

    override protected void OnAppearing()
    {
      // Update the ids of all the matches
      foreach (var (matchButton, matchData) in MatchViews)
      {
        matchButton.Text = matchData.Id ?? matchButton.Text;
      }
    }

    private async void GoToMatch(MatchData matchData)
    {
      await Navigation.PushAsync(new MatchPage(matchData, EventData.Config.SpecFieldConfigs));
    }
  }
}
