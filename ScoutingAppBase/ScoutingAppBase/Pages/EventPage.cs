using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

using ScoutingAppBase.Event;

namespace ScoutingAppBase.Pages
{
  public class EventPage : ContentPage
  {
    public EventPage(EventData eventData)
    {
      EventData = eventData;

      var newMatchButton = new Button
      {
        Text = "+"
      };
      newMatchButton.Clicked += (sender, e) =>
      {
        var match = new MatchData();
        EventData.Matches.Add(match);
        GoToMatch(match);
      };

      var layout = new StackLayout
      {
        Children = {
          new Label { Text = "Welcome to Xamarin.Forms!" },
          newMatchButton
        }
      };

      Content = layout;
    }

    private readonly EventData EventData;

    private async void GoToMatch(MatchData matchData)
    {
      await Navigation.PushAsync(new MatchPage(matchData, EventData.Config.FieldConfigs));
    }
  }
}