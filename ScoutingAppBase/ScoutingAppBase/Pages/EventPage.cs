using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace ScoutingAppBase.Pages
{
  public class EventPage : ContentPage
  {
    public EventPage()
    {
      Content = new StackLayout
      {
        Children = {
                    new Label { Text = "Welcome to Xamarin.Forms!" }
                }
      };
    }

    public void OpenMatch()
    {
      
    }
  }
}