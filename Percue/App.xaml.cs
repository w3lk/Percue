using ControlzEx.Theming;
using Octokit;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Percue
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Set the application theme to Dark.Green
            ThemeManager.Current.ChangeTheme(this, "Dark.Blue");
            var t = Task.Run(() => CheckGitHubNewerVersion());
        }

        private async Task CheckGitHubNewerVersion()
        {
            //Get all releases from GitHub
            //Source: https://octokitnet.readthedocs.io/en/latest/getting-started/
            GitHubClient client = new GitHubClient(new ProductHeaderValue("Percue-Updater"));
            IReadOnlyList<Release> releases = await client.Repository.Release.GetAll("w3lk", "Percue");

            //Setup the versions
            Regex rx = new Regex(@".*(\d\.\d\.\d)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var matches = rx.Matches(releases[0].TagName);
            var versionString = matches[0].Groups[1].Value;
            
            Version latestGitHubVersion = new Version(versionString);
            Version localVersion = new Version("1.0.6"); //Replace this with your local version. 
                                                         //Only tested with numeric values.

            //Compare the Versions
            //Source: https://stackoverflow.com/questions/7568147/compare-version-numbers-without-using-split-function
            int versionComparison = localVersion.CompareTo(latestGitHubVersion);
            if (versionComparison < 0)
            {
                //The version on GitHub is more up to date than this local release.

            }
            else if (versionComparison > 0)
            {
                //This local version is greater than the release version on GitHub.
            }
            else
            {
                //This local Version and the Version on GitHub are equal.
            }

        }

    }
}
