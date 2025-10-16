namespace Csdsa.Domain.Factories;

public class AlgorithmFactory
{
    public Services.Sorting.ComparisonSortService CreateComparisonSortService()
    {
        return new Services.Sorting.ComparisonSortService();
    }

    public Services.Searching.GraphSearchService CreateGraphSearchService()
    {
        return new Services.Searching.GraphSearchService();
    }
}
