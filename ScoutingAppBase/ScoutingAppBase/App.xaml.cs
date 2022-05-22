using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.CommunityToolkit;
using ScoutingAppBase.Data;
using ScoutingAppBase.Pages;

#nullable enable

namespace ScoutingAppBase
{
  public partial class App : Application
  {
    /// <summary>
    /// The folder where all event data will be stored
    /// </summary>
    public static readonly string StorageFolder =
      Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    public App()
    {
      InitializeComponent();

      var eventConfig = new EventConfig
      {
        EventName = "dcmp",
        OurTeam = 449,
        SpecFieldConfigs =
        {
          new FieldConfig
          {
            Name = "foo",
            Type = FieldType.Num,
            Min = 0,
            Max = 100,
            Inc = 1
          }
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