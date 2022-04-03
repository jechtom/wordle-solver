# wordle-solver

## Commands

### Filter 5 Letter Words

```
dotnet run --project .\src\WordleSolver\WordleSolver.csproj -- filter-by-length 5 ".\data\words_alpha.txt" ".\data\words_alpha-length-5.txt"
```

### Score Start Words

```
dotnet run --project .\src\WordleSolver\WordleSolver.csproj -- start-word-score 5 ".\data\words_alpha-length-5.txt" ".\data\words_alpha-length-5-start-scores.txt"
```

### Sort Start Words Scoring

```
dotnet run --project .\src\WordleSolver\WordleSolver.csproj -- start-word-score-sort ".\data\words_alpha-length-5-start-scores.txt"
```


## List Of English Words

I used data files from [dwyl/english-words](https://github.com/dwyl/english-words) repo. 