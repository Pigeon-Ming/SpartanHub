using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace SpartanHub.Models
{
    /// <summary>
    /// 将比赛结果（Outcome）转换为对应的颜色
    /// Outcome: 1=获胜 (绿色), 0=平局 (黄色), 2=战败 (红色)
    /// </summary>
    public class MatchOutcomeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int outcome)
            {
                switch (outcome)
                {
                    case 1: // Victory
                        return new SolidColorBrush(Colors.Green);
                    case 0: // Tie
                        return new SolidColorBrush(Colors.Yellow);
                    case 2: // Defeat
                        return new SolidColorBrush(Colors.Red);
                    default:
                        return new SolidColorBrush(Colors.Gray);
                }
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
