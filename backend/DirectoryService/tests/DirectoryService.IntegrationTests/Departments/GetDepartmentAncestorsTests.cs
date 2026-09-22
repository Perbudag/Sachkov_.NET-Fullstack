using DirectoryService.Contracts.Departments;
using DirectoryService.IntegrationTests.TestData;
using Shared;
using System.Net;
using System.Net.Http.Json;

namespace DirectoryService.IntegrationTests.Departments;

public class GetDepartmentAncestorsTests : DirectoryBaseTests
{
    public GetDepartmentAncestorsTests(DirectoryTestWebFactory factory) : base(factory) { }

    [Fact]
    public async Task Get_department_ancestors_With_nested_department_Should_return_ancestors()
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var rootId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Компания",
            "company",
            null,
            cancellationToken));
        var parentId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Администрация",
            "administration",
            rootId,
            cancellationToken));
        var childId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Продажи",
            "sales",
            parentId,
            cancellationToken));

        // act
        var response = await HttpClient.GetAsync(
            $"api/Departments/{childId}/ancestors?page=1&pageSize=10&sortBy=Depth&sortDir=asc",
            cancellationToken);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<PageResult<DepartmentTreeNodeDto[]>>>(cancellationToken);

        // assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(envelope?.IsError);
        Assert.NotNull(envelope?.Result);
        Assert.Equal(2, envelope.Result.TotalCount);
        Assert.Contains(envelope.Result.Value, department => department.Id == rootId);
        Assert.Contains(envelope.Result.Value, department => department.Id == parentId);
    }

    [Fact]
    public async Task Get_department_ancestors_With_empty_id_Should_return_validation_error()
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var id = Guid.Empty;

        // act
        var response = await HttpClient.GetAsync(
            $"api/Departments/{id}/ancestors?page=1&pageSize=10&sortBy=Depth&sortDir=asc",
            cancellationToken);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<PageResult<DepartmentTreeNodeDto[]>>>(cancellationToken);

        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(envelope?.IsError);
        Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "departments.validation.error", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Get_department_ancestors_With_invalid_sort_direction_Should_return_validation_error()
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var departmentId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Компания",
            "company",
            null,
            cancellationToken));

        // act
        var response = await HttpClient.GetAsync(
            $"api/Departments/{departmentId}/ancestors?sortBy=Depth&sortDir=invalid",
            cancellationToken);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<PageResult<DepartmentTreeNodeDto[]>>>(cancellationToken);

        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(envelope?.IsError);
        Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "departments.validation.error", StringComparison.Ordinal));
    }
}
