# Reqnroll.Markdown

## Reqnroll.Markdown is a .NET library that:

- Automatically scans for .md files during the dotnet build process

- Extracts only the content wrapped in ```gherkin code blocks

- Generates corresponding .feature files from that content


## Requirements & Features:

- Automatically runs during project build using an MSBuild Task

- Executes before another library (e.g. Reqnroll) that processes .feature files
