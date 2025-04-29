using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Reqnroll.Markdown
{
    public class MarkdownToFeatureTask : Microsoft.Build.Utilities.Task
    {
        [Required]
        public string SourceDirectory { get; set; } = string.Empty;

        public string OutputDirectory { get; set; }

        public override bool Execute()
        {
            try
            {
                Log.LogMessage(MessageImportance.High, $"MarkdownToFeatureTask started. SourceDirectory={SourceDirectory}, OutputDirectory={OutputDirectory}");

                var mdFiles = Directory.GetFiles(SourceDirectory, "*.md", SearchOption.AllDirectories);
                Log.LogMessage(MessageImportance.High, $"MarkdownToFeatureTask started. Found={string.Join(",", mdFiles)}");
                Log.LogMessage(MessageImportance.Low, $"MarkdownToFeatureTask started. Found={string.Join(",", mdFiles)}");

                foreach (var mdFile in mdFiles)
                {
                    var content = File.ReadAllText(mdFile);

                    var matches = Regex.Matches(content, @"```gherkin\s*(.*?)```", RegexOptions.Singleline);

                    if (matches.Count == 0)
                    {
                        Log.LogMessage(MessageImportance.Low, $"No gherkin blocks found in {mdFile}");
                        continue;
                    }

                    var extractedText = string.Join("\n\n", matches.Cast<Match>().Select(m => m.Groups[1].Value.Trim()));

                    if (string.IsNullOrWhiteSpace(extractedText))
                    {
                        Log.LogMessage(MessageImportance.Low, $"No text inside gherkin blocks in {mdFile}");
                        continue;
                    }

                    var featureFileName = Path.ChangeExtension(Path.GetFileName(mdFile), ".feature");
                    var outputDir = OutputDirectory ?? Path.GetDirectoryName(mdFile);
                    var destPath = Path.Combine(outputDir, featureFileName);

                    Directory.CreateDirectory(Path.GetDirectoryName(destPath));
                    File.WriteAllText(destPath, extractedText);

                    Log.LogMessage(MessageImportance.High, $"Feature file created: {destPath}");
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }
    }
}