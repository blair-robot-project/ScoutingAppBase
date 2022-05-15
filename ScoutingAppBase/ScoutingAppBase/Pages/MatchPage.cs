using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

using ScoutingAppBase.Event;

namespace ScoutingAppBase.Pages
{
  public class MatchPage : ContentPage
  {
    public MatchPage(Match match)
    {
      Content = new StackLayout
      {
        Children = {
                    new Label { Text = "Welcome to Xamarin.Forms!" }
                }
      };
    }
  }
}