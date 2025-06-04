namespace GenericRepository.Abstractions
{
    /// <summary>
    /// Base class that provides pagination properties common to paged results.
    /// </summary>
    public abstract class PagedResultBase
    {
        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the total number of pages available.
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// Gets or sets the size of each page (i.e., the number of records per page).
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the total number of rows across all pages.
        /// </summary>
        public int RowCount { get; set; }

        /// <summary>
        /// Calculates the index of the first row on the current page.
        /// </summary>
        public int FirstRowOnPage
        {

            get { return (CurrentPage - 1) * PageSize + 1; }
        }

        /// <summary>
        /// Calculates the index of the last row on the current page.
        /// </summary>
        public int LastRowOnPage
        {
            get { return Math.Min(CurrentPage * PageSize, RowCount); }
        }
    }

    /// <summary>
    /// Represents the paged result for a collection of items of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the items in the paged result.</typeparam>
    public sealed class PagedResult<T> : PagedResultBase
    {
        /// <summary>
        /// Gets or sets the list of items for the current page.
        /// </summary>
        public IList<T> Results { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedResult{T}"/> class.
        /// </summary>
        public PagedResult()
        {
            Results = [];
        }
    }
}
