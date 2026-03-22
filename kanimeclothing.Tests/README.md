# Kanimé Clothing - Unit Tests

This project contains unit tests for the Kanimé Clothing application using **xUnit** and **Moq**.

## Project Structure

```
kanimeclothing.Tests/
├── Services/
│   ├── ProductServiceTests.cs
│   └── JsonLocalizationServiceTests.cs
├── Controllers/
│   └── HomeControllerTests.cs
└── README.md
```

## Running Tests

### From Terminal
```powershell
# Run all tests
dotnet test

# Run tests with verbose output
dotnet test --verbosity detailed

# Run tests for a specific test class
dotnet test --filter FullyQualifiedName~ProductServiceTests

# Run tests and generate a coverage report
dotnet test /p:CollectCoverage=true
```

### From Visual Studio
1. Open **Test Explorer** (Test → Test Explorer)
2. Click "Run All Tests" or select specific tests
3. View results in the Test Explorer window

## Test Coverage

### ProductServiceTests
- ✅ Loading products from JSON file
- ✅ Handling missing product files
- ✅ Product caching mechanism
- ✅ Retrieving product by ID
- ✅ Handling invalid product IDs

### JsonLocalizationServiceTests
- ✅ Retrieving localized strings by key and language
- ✅ Fallback behavior for missing keys
- ✅ Support for multiple languages (en, es, fr)
- ✅ Handling unsupported languages

### HomeControllerTests
- ✅ Index page returns view
- ✅ Privacy page returns view
- ✅ Categories page returns view
- ✅ Shop with and without filters
- ✅ Product details retrieval
- ✅ Handling invalid product IDs

## Adding More Tests

To add tests for additional classes:

1. Create a new test file in the appropriate folder (Services/, Controllers/, etc.)
2. Follow the naming convention: `[ClassName]Tests.cs`
3. Use xUnit's `[Fact]` for simple tests and `[Theory]` for parameterized tests
4. Use Moq to mock dependencies

### Example Test Template

```csharp
[Fact]
public async Task MethodName_WithValidInput_ReturnsExpectedResult()
{
    // Arrange
    var mock = new Mock<IDependency>();
    mock.Setup(m => m.Method()).Returns(expectedValue);
    
    var sut = new ServiceUnderTest(mock.Object);

    // Act
    var result = await sut.MethodUnderTest();

    // Assert
    Assert.Equal(expectedValue, result);
    mock.Verify(m => m.Method(), Times.Once);
}
```

## Test Best Practices

- **Naming**: Use `MethodName_Scenario_ExpectedResult` pattern
- **Arrange-Act-Assert**: Structure tests with clear sections
- **Isolation**: Each test should be independent
- **Mocking**: Mock external dependencies (services, file systems, databases)
- **Assertions**: Use specific assertions (not just `Assert.True`)
- **Cleanup**: Clean up resources (temporary files, temp directories)

## Dependencies

- **xunit** - Testing framework
- **xunit.runner.visualstudio** - Visual Studio test explorer integration
- **Moq** - Mocking library
- **Microsoft.NET.Test.Sdk** - Test SDK

## Continuous Integration

These tests can be integrated into CI/CD pipelines (GitHub Actions, Azure Pipelines, etc.):

```yaml
- name: Run Tests
  run: dotnet test --logger "trx" --results-directory "./TestResults"
```

## Troubleshooting

### Tests not discovered
- Ensure the test project is properly configured with `<IsTestProject>true</IsTestProject>`
- Verify test methods are public and decorated with `[Fact]` or `[Theory]`

### File path issues
- Tests use temporary directories for file operations
- Ensure proper cleanup in tests to avoid leftover temp files

### Mock setup issues
- Verify mock return types match the expected interface
- Use `It.IsAny<T>()` for flexible matching
