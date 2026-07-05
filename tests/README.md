## Tests

Tests have been created for the corresponding modules located in the `\src` folder. They use the xUnit framework and the FluentAssertions library. The coverlet.collector and coverlet.msbuild tools from the [coverlet](https://github.com/coverlet-coverage/coverlet) are used to determine the percentage of code coverage, and [ReportGenerator](https://reportgenerator.io/) is used for visualization.

### Usage

To run the tests, execute the following command in the terminal:

```bash
dotnet test
```

It is important to run this command via the cli, as it runs the tests with code coverage. After executing the command and completing the tests, a code coverage report will be generated and located in the directory  `...\tests\TestResults\Coverage`. Next, open the `index.html` file, which will display the full analysis of test coverage.