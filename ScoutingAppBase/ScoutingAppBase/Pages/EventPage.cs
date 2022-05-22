using System.Collections.Generic;
using Xamarin.Forms;
using ScoutingAppBase.Data;

namespace ScoutingAppBase.Pages
{
  public sealed class EventPage : ContentPage
  {
    private readonly EventData EventData;

    private readonly DataManager DataManager;

    /// <summary>
    /// Used for mapping buttons to go to a match with that match's data
    /// </summary>
    private readonly Dictionary<MatchData, Button> MatchViews;

    /// <summary>
    /// Color to show match number in if synced
    /// </summary>
    private static readonly Color SyncedColor = Color.Blue;

    /// <summary>
    /// Color to show match number in if not synced
    /// </summary>
    private static readonly Color NotSyncedColor = Color.Red;

    public EventPage(EventData eventData)
    {
      EventData = eventData;
      DataManager = new DataManager(eventData.Config, OnMatchSync);
      MatchViews = new Dictionary<MatchData, Button>();
      var content = new StackLayout();
      Content = content;

      var newMatchButton = new Button {Text = "+"};
      var settingsButton = new Button {Text = "Settings"};

      NavigationPage.SetTitleView(
        this, new StackLayout
        {
          Orientation = StackOrientation.Horizontal,
          Children =
          {
            new Label {Text = eventData.Config.EventName},
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
        int matchNum = EventData.Matches.Count;
        var match = new MatchData(matchNum, false);
        EventData.Matches.Add(match);

        // Add a button on the event page for going to the match
        var matchButton = new Button {Text = $"{matchNum}", TextColor = NotSyncedColor};
        matchesLayout.Children.Add(matchButton);

        MatchViews[match] = matchButton;

        GoToMatch(match);
      };

      settingsButton.Clicked += (sender, e) => { Navigation.PushModalAsync(new SettingsPage()); };
    }

    protected override void OnAppearing()
    {
      // Update the ids of all the matches
      foreach (var (matchData, matchButton) in MatchViews)
      {
        matchButton.Text = $"{matchData.MatchNum}";
        matchButton.TextColor = matchData.Synced ? SyncedColor : NotSyncedColor;
      }
    }

    private void OnMatchSync(MatchData match)
    {
      MatchViews[match].TextColor = SyncedColor;
    }

    private async void GoToMatch(MatchData matchData)
    {
      await Navigation.PushAsync(
        new MatchPage(matchData, EventData.Config.SpecFieldConfigs, DataManager));
    }
  }
}