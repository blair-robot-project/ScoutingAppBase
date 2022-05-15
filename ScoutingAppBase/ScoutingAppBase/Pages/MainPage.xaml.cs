using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.CommunityToolkit;

namespace ScoutingAppBase.Pages
{
  public partial class MainPage : ContentPage
  {
    public MainPage()
    {
      InitializeComponent();
      Entry numInput = new Entry();
      numInput.Behaviors.Add(new NumericValidationBehavior(1.0, 4.0, false));

      CheckBox checkBox = new CheckBox();
      checkBox.CheckedChanged += (sender, e) => { };

      StackLayout radioButtons = createRadioButtons("Choose", new string[] { "foo", "bar" });

      FieldsLayout.Children.Add(numInput);
      FieldsLayout.Children.Add(checkBox);
      FieldsLayout.Children.Add(radioButtons);
    }

    public StackLayout createRadioButtons(string label, string[] choices)
    {
      StackLayout layout = new StackLayout();
      layout.Children.Add(new Label { Text = label });
      foreach (string choice in choices)
      {
        layout.Children.Add(new RadioButton { Content = choice });
      }
      return layout;
    }
  }
}
