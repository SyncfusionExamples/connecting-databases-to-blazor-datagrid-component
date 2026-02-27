using Microsoft.AspNetCore.Mvc;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Text.Json.Serialization;
using Web_API.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GridController : ControllerBase
    {
        private readonly OrderDbContext _context;

        public GridController(OrderDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns data with search, filter, sort, and paging operations
        /// </summary>
        [HttpGet]
        public object GetOrderData()
        {
            try
            {
                IEnumerable dataSource = _context.Orders.ToList();

                // Extract the query string from the incoming request.
                var queryString = Request.Query;

                // Handling filtering and searching operation.
                string? filterQuery = queryString["$filter"];
                if (!string.IsNullOrEmpty(filterQuery))
                {
                    // Split the filter query into individual conditions using "and" as a delimiter.
                    var filterConditions = filterQuery.Split(new[] { " and " }, StringSplitOptions.RemoveEmptyEntries);
                    List<SearchFilter> searchFilters = new List<SearchFilter>();
                    List<WhereFilter> filters = new List<WhereFilter>();

                    foreach (var condition in filterConditions)
                    {
                        // Check if the condition involves a substring search.
                        if (condition.Contains("substringof"))
                        {
                            var conditionParts = condition.Split('(', ')', '\'');
                            bool isSearch = conditionParts.Count(x => x == "substringof") > 1;

                            var fields = typeof(Order).GetProperties().Select(p => p.Name).ToList();

                            if(isSearch)
                            {
                                var searchValue = conditionParts[3]?.ToLower() ?? "";
                                searchFilters.Add(new SearchFilter {Fields = fields, Key = searchValue, IgnoreCase = true, Operator = "contains" });
                            }
                            else
                            {
                                var searchValue = conditionParts[2]?.ToLower() ?? "";
                                var fieldName = conditionParts[4];
                                
                                filters.Add(new WhereFilter { Field = fieldName, Operator = "contains", value = searchValue, IgnoreCase = true });
                            }
                        }
                        else
                        {
                            // Initialize variables to hold the filter field and value.
                            string filterField = "";
                            string filterValue = "";
                            string filterOperator = "equal";

                            // Split the condition into parts to extract the field and value.
                            var filterParts = condition.Split('(', ')', '\'');

                            // Handle cases where the filter condition has fewer parts.
                            if (filterParts.Length < 6)
                            {
                                var filterValueParts = filterParts[1].Split();
                                filterField = filterValueParts[0];
                                if (filterValueParts.Length > 1)
                                {
                                    filterOperator = filterValueParts[1].ToLower();
                                }
                                filterValue = filterValueParts.Length > 2 ? filterValueParts[2].Trim('\'') : "";
                            }
                            else
                            {
                                filterOperator = filterParts[0].ToLower();
                                filterField = filterParts[2];
                                filterValue = filterParts[4];
                            }

                            filters.Add(new WhereFilter { Field = filterField, Operator = filterOperator, value = filterValue });
                        }
                    }
                    dataSource = DataOperations.PerformSearching(dataSource, searchFilters);
                    dataSource = DataOperations.PerformFiltering(dataSource, filters, filters[0].Operator);
                }

                // Handling Sorting
                string? sort = queryString["$orderby"];
                if (!string.IsNullOrEmpty(sort))
                {
                    // Split the sorting query into individual conditions using commas as delimiters.
                    var sortConditions = sort.Split(',');
                    List<SortedColumn> sorted = new List<SortedColumn>();
                    foreach (var sortCondition in sortConditions)
                    {
                        var sortParts = sortCondition.Trim().Split(' ');
                        var sortBy = sortParts[0];
                        var descending = sortParts.Length > 1 && sortParts[1].ToLower() == "desc";
                        sorted.Add(new SortedColumn { Field = sortBy, Direction = descending ? SortOrder.Descending : SortOrder.Ascending });
                    }
                    dataSource = DataOperations.PerformSorting(dataSource, sorted);
                }

                int totalRecordsCount = dataSource.Cast<Order>().Count();

                // Handling Paging
                int skip = Convert.ToInt32(queryString["$skip"]);
                int take = Convert.ToInt32(queryString["$top"]);
                if (skip != 0)
                {
                    dataSource = DataOperations.PerformSkip(dataSource, skip);
                }
                if (take != 0)
                {
                    dataSource = DataOperations.PerformTake(dataSource, take);
                }

                return new { Items = dataSource, count = totalRecordsCount };
            }
            catch (Exception ex)
            {
                return new { error = ex.Message, innerError = ex.InnerException?.Message };
            }
        }

        /// <summary>
        /// Inserts a new order record
        /// </summary>
        [HttpPost]
        public void Post([FromBody] Order value)
        {
            try
            {
                _context.Orders.Add(value);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inserting order: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing order record
        /// </summary>
        [HttpPut]
        public void Put([FromBody] Order order)
        {
            try
            {
                var existingOrder = _context.Orders.Find(order.OrderID);
                if (existingOrder != null)
                {
                    _context.Entry(existingOrder).CurrentValues.SetValues(order);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating order: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes an order record
        /// </summary>
        [HttpDelete("{id}")]
        public void Delete([FromBody] int id)
        {
            try
            {
                var order = _context.Orders.Find(id);
                if (order != null)
                {
                    _context.Orders.Remove(order);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting order: {ex.Message}");
            }
        }

    }
}
