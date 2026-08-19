using DirectoryService.Contracts.Departments;
using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.Positions;
using DirectoryService.IntegrationTests.TestData;
using Microsoft.EntityFrameworkCore;
using Shared;
using System.Net;
using System.Net.Http.Json;

namespace DirectoryService.IntegrationTests.Departments;

public class DeleteDepartmentTests : DirectoryBaseTests
{
    public DeleteDepartmentTests(DirectoryTestWebFactory factory) : base(factory) { }

    [Fact]
    public async Task Delete_department_With_existing_id_Should_succeed()
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var id = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(db, "Администрация", "administration", null, cancellationToken));

        // act
        var response = await HttpClient.DeleteAsync($"api/Departments/{id}", cancellationToken);

        // assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var getResponse = await HttpClient.GetAsync($"api/Departments/{id}", cancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        var departmentIsDeleted = await ExecuteInDbAsync(db =>
            db.Departments
            .IgnoreQueryFilters()
            .Where(d => d.Id == id)
            .Select(d => d.IsDeleted)
            .FirstAsync(cancellationToken));

        Assert.True(departmentIsDeleted);

        await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);

        var department = await ExecuteInDbAsync(db =>
            db.Departments
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken));

        Assert.Null(department);
    }

    [Fact]
    public async Task Delete_department_With_has_children_Should_succeed()
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var id = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(db, "Бухгалтерия", "accounting", null, cancellationToken));
        await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(db, "Администрация", "administration", id, cancellationToken));

        // act
        var response = await HttpClient.DeleteAsync($"api/Departments/{id}", cancellationToken);

        // assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        var getResponse = await HttpClient.GetAsync($"api/Departments/{id}", cancellationToken);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var departmentIsDeleted = await ExecuteInDbAsync(db =>
            db.Departments
            .IgnoreQueryFilters()
            .Where(d => d.Id == id)
            .Select(d => d.IsDeleted)
            .FirstAsync(cancellationToken));

        Assert.False(departmentIsDeleted);
    }

    [Fact]
    public async Task Delete_department_With_unknown_id_Should_return_not_found()
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var id = Guid.NewGuid();

        // act
        var response = await HttpClient.DeleteAsync($"api/Departments/{id}", cancellationToken);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<object?>>(cancellationToken);

        // assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.True(envelope?.IsError);
        Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "departments.not.found", StringComparison.Ordinal));
    }
}
