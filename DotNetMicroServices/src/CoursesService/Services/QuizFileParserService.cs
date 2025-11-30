using System.Text;
using CoursesService.Services;
using CoursesService.Models;

namespace CoursesService.Services;

public class QuizFileParserService : IQuizFileParserService
{
    private readonly ILogger<QuizFileParserService> _logger;

    public QuizFileParserService(ILogger<QuizFileParserService> logger)
    {
        _logger = logger;
    }

    public async Task<List<QuizQuestionData>> ParseFileAsync(Stream fileStream, string fileName)
    {
        var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
        
        return fileExtension switch
        {
            ".csv" => await ParseCsvAsync(fileStream),
            ".xlsx" or ".xls" or ".xlsm" => await ParseExcelAsync(fileStream),
            _ => throw new NotSupportedException($"File type {fileExtension} is not supported. Only CSV and Excel files are supported.")
        };
    }

    private async Task<List<QuizQuestionData>> ParseCsvAsync(Stream fileStream)
    {
        var questions = new List<QuizQuestionData>();
        
        using var reader = new StreamReader(fileStream, Encoding.UTF8);
        var lines = new List<string>();
        
        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            lines.Add(line);
        }

        if (lines.Count < 2)
        {
            throw new InvalidDataException("CSV file must contain at least a header row and one data row.");
        }

        // Skip header row (first line)
        for (int i = 1; i < lines.Count; i++)
        {
            var row = ParseCsvLine(lines[i]);
            
            if (row.Count < 6)
            {
                _logger.LogWarning("Skipping row {RowNumber} - insufficient columns. Expected at least 6 columns.", i + 1);
                continue;
            }

            var question = new QuizQuestionData
            {
                Question = row[0].Trim(),
                Order = i - 1, // Start from 0, skip header
                Options = new List<QuizOptionData>
                {
                    new QuizOptionData { Value = row[1].Trim(), IsCorrect = false },
                    new QuizOptionData { Value = row[2].Trim(), IsCorrect = false },
                    new QuizOptionData { Value = row[3].Trim(), IsCorrect = false },
                    new QuizOptionData { Value = row[4].Trim(), IsCorrect = false }
                }
            };

            // Parse correct answer (column 5, index 5)
            var correctAnswer = row[5].Trim().ToUpperInvariant();
            
            // Mark the correct option
            if (correctAnswer == "A" || correctAnswer == "1")
                question.Options[0].IsCorrect = true;
            else if (correctAnswer == "B" || correctAnswer == "2")
                question.Options[1].IsCorrect = true;
            else if (correctAnswer == "C" || correctAnswer == "3")
                question.Options[2].IsCorrect = true;
            else if (correctAnswer == "D" || correctAnswer == "4")
                question.Options[3].IsCorrect = true;
            else
            {
                _logger.LogWarning("Invalid correct answer '{CorrectAnswer}' in row {RowNumber}. Expected A, B, C, or D.", correctAnswer, i + 1);
                continue;
            }

            questions.Add(question);
        }

        return questions;
    }

    private List<string> ParseCsvLine(string line)
    {
        var fields = new List<string>();
        var currentField = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    // Escaped quote
                    currentField.Append('"');
                    i++; // Skip next quote
                }
                else
                {
                    // Toggle quote state
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                // End of field
                fields.Add(currentField.ToString());
                currentField.Clear();
            }
            else
            {
                currentField.Append(c);
            }
        }

        // Add last field
        fields.Add(currentField.ToString());

        return fields;
    }

    private async Task<List<QuizQuestionData>> ParseExcelAsync(Stream fileStream)
    {
        // For Excel files, we'll use a simple approach by reading as CSV first
        // In production, you might want to use EPPlus or ClosedXML library
        // For now, we'll convert Excel to CSV format in memory
        
        _logger.LogWarning("Excel file parsing is not fully implemented. Please use CSV format for now.");
        throw new NotSupportedException("Excel file parsing requires additional libraries. Please use CSV format or install EPPlus/ClosedXML package.");
        
        // TODO: Implement Excel parsing using EPPlus or ClosedXML
        // Example with EPPlus:
        // using (var package = new ExcelPackage(fileStream))
        // {
        //     var worksheet = package.Workbook.Worksheets[0];
        //     // Parse rows...
        // }
    }
}

