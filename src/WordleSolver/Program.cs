using System.CommandLine;
using WordleSolver;

var rootCommand = new RootCommand();

var filterByLengthCommand = new Command("filter-by-length", "Filter list of words with given length and writes output to file.");
filterByLengthCommand.AddArgument(new Argument<int>("length"));
filterByLengthCommand.AddArgument(new Argument<string>("input"));
filterByLengthCommand.AddArgument(new Argument<string>("output"));
filterByLengthCommand.SetHandler<int, string, string>(FilterByLength.HandleAsync, filterByLengthCommand.Arguments.ToArray());
rootCommand.AddCommand(filterByLengthCommand);

var startWordScoreCommand = new Command("start-word-score", "Calculate total green/yellow score of starting and writes output to file.");
startWordScoreCommand.AddArgument(new Argument<int>("length"));
startWordScoreCommand.AddArgument(new Argument<string>("input"));
startWordScoreCommand.AddArgument(new Argument<string>("output"));
startWordScoreCommand.SetHandler<int, string, string>(StartWordScore.HandleAsync, startWordScoreCommand.Arguments.ToArray());
rootCommand.AddCommand(startWordScoreCommand);

var startWordScoreSortCommand = new Command("start-word-score-sort", "Sorts given start word score file.");
startWordScoreSortCommand.AddArgument(new Argument<string>("input"));
startWordScoreSortCommand.SetHandler<string>(StartWordScoreSort.HandleAsync, startWordScoreSortCommand.Arguments.ToArray());
rootCommand.AddCommand(startWordScoreSortCommand);

var interactiveCommand = new Command("interactive", "Starts interactive game session.");
interactiveCommand.AddArgument(new Argument<int>("length"));
interactiveCommand.AddArgument(new Argument<string>("input"));
interactiveCommand.SetHandler<int, string>(Interactive.HandleAsync, interactiveCommand.Arguments.ToArray());
rootCommand.AddCommand(interactiveCommand);

var beeSolverCommand = new Command("bee", "Solves spelling bee.");
beeSolverCommand.AddArgument(new Argument<string>("input"));
beeSolverCommand.AddArgument(new Argument<string>("letters"));
beeSolverCommand.SetHandler<string, string>(BeeSolver.HandleAsync, beeSolverCommand.Arguments.ToArray());
rootCommand.AddCommand(beeSolverCommand);

await rootCommand.InvokeAsync(args);