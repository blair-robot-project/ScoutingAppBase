using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

using ScoutingAppBase.Data;

namespace ScoutingAppBase.Pages
{
  public sealed class MatchPage : ContentPage
  {
    public MatchPage(MatchData match, List<FieldConfig> fieldConfigs)
    {
      Match = match;
      var layout = new StackLayout();

      var backButton = new Button { Text = "Back" };
      backButton.Clicked += (sender, e) => Navigation.PopAsync();
      layout.Children.Add(backButton);

      foreach (var fieldConfig in fieldConfigs)
      {
        layout.Children.Add(FieldConfigToElement(fieldConfig));
      }

      Content = layout;
    }

    private readonly MatchData Match;

    private View FieldConfigToElement(FieldConfig config)
    {
      switch (config.Type)
      {
        case FieldType.Num:
          return NumElement(config);
        case FieldType.Bool:
          return BoolElement(config);
        case FieldType.Choice:
          return RadioElement(config);
        default:
          return TextElement(config);
      }
    }

    private View NumElement(FieldConfig config)
    {
      var defaultVal = config.Min;
      Match.Fields[config.Name] = defaultVal.ToString();

      var valueLabel = new Label { Text = defaultVal.ToString() };
      var stepper = new Stepper
      {
        Value = defaultVal,
        Minimum = config.Min,
        Maximum = config.Max,
        Increment = config.Inc
      };
      stepper.ValueChanged += (_, e) =>
      {
        var newVal = e.NewValue.ToString();
        valueLabel.Text = newVal;
        Match.Fields[config.Name] = newVal;
      };

      return new StackLayout
      {
        Orientation = StackOrientation.Horizontal,
        Children = {
          new Label { Text = config.Name },
          valueLabel,
          stepper
        }
      };
    }

    private View BoolElement(FieldConfig config)
    {
      Match.Fields[config.Name] = "false";

      var checkbox = new CheckBox();
      checkbox.CheckedChanged += (_, e) =>
      {
        Match.Fields[config.Name] = e.Value.ToString();
      };

      return new StackLayout
      {
        Orientation = StackOrientation.Horizontal,
        Children = {
          new Label { Text = config.Name },
          checkbox
        }
      };
    }

    private View RadioElement(FieldConfig config)
    {
      var layout = new StackLayout
      {
        Orientation = StackOrientation.Vertical,
        Children = { new Label { Text = config.Name } }
      };

      foreach (var choice in config.Choices)
      {
        var isSelected = config.DefaultChoice == choice;
        Match.Fields[config.Name] = isSelected.ToString();

        var button = new RadioButton
        {
          Content = choice,
          IsChecked = isSelected,
        };
        button.CheckedChanged += (_, e) =>
        {
          Match.Fields[config.Name] = e.Value.ToString();
        };

        layout.Children.Add(button);
      }

      return layout;
    }

    private View TextElement(FieldConfig config)
    {
      Match.Fields[config.Name] = "";

      var text = new Entry();
      text.TextChanged += (_, e) =>
      {
        Match.Fields[config.Name] = e.NewTextValue;
      };

      return new StackLayout
      {
        Orientation = StackOrientation.Horizontal,
        Children = {
          new Label { Text = config.Name },
          text
        }
      };
    }
  }
}
