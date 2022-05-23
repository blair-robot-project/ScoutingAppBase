using System.Collections.Generic;
using Xamarin.Forms;
using ScoutingAppBase.Data;
using ScoutingAppBase.Pages;

#nullable enable

namespace ScoutingAppBase
{
  public partial class App : Application
  {
    public App()
    {
      InitializeComponent();

      var eventConfig = new EventConfig
      {
        EventName = "dcmp",
        OurTeam = 449,
        SpecFieldConfigs =
        {
          
        }
      };
      MainPage = new NavigationPage(
        new EventPage(
          new EventData(eventConfig, new List<MatchData>())
        )
      );
    }

    protected override void OnStart()
    {
    }

    protected override void OnSleep()
    {
    }

    protected override void OnResume()
    {
    }
  }
}