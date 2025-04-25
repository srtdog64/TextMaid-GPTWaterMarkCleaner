using System.Configuration;
using System.Data;
using System.Globalization;
using System.Windows;

namespace TextMaid
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // 시스템 언어 설정 반영 (예: 한국어 환경이면 ko-KR)
            CultureInfo culture = CultureInfo.CurrentUICulture;
            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;

            base.OnStartup(e);
        }
    }

}
