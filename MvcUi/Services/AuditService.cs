using AyrshireCollege.Biis.PresentationFormattingLibrary.Date;
using DataAccessLibrary.DataServices;
using DataAccessLibrary.Models;
using SharedViewModelLibrary.Models;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace MvcUi.Services
{
    public class AuditService : IAuditService
    {
        private readonly IStatusService _statusService;

        // Generates an audit trail string describing what changed between original and updated entities
        // Parameters:
        // - original: The entity as it exists before update
        // - updated: The entity after changes have been applied
        // - updatedBy: The username or identifier of the person making the update
        // Returns:
        // - A multiline string summarizing the updated fields with old and new values, including timestamp and user

        public AuditService(IStatusService statusService)
        {
            _statusService = statusService;
        }


        /// <summary>
        /// Generates audit log entries for the creation of a new main form record.
        /// This method inspects all non-null properties of the newly created entity,
        /// resolves any friendly display names (including from lookup tables), and records each as a change from blank (OldValue = "").
        /// </summary>
        /// <param name="newSubmission">The new entity model instance that was created.</param>
        /// <param name="changedBy">The display name or identity of the user who created the record.</param>
        /// <returns>A list of audit log entries indicating the properties that were populated during creation.</returns>
        public Task<List<MainFormAuditLogEntityModel>> GenerateAuditLogEntriesForCreation(MainFormEntityModel newSubmission,
                                                                                          string changedBy)
        {
            var auditEntries = new List<MainFormAuditLogEntityModel>
            {
                new MainFormAuditLogEntityModel
                {
                    MainFormId = newSubmission.Id,
                    PropertyName = "(Creation)",
                    DisplayName = "New Record Created",
                    OldValue = "",
                    NewValue = "Record was created",
                    ChangedBy = changedBy,
                    ChangeDate = DateTime.UtcNow
                }
            };

            return Task.FromResult(auditEntries);
        }



        /// <summary>
        /// <summary>
        /// Generates a list of audit log entries reflecting the differences between two MainFormAdminEntityModel instances.
        /// 
        /// This method:
        /// - Compares scalar properties and list properties (bridging table IDs) to detect changes.
        /// - Uses Display attributes from the ViewModel to get user-friendly property names for audit display.
        /// - Resolves raw values and IDs to friendly strings using async resolver functions (e.g., service lookups).
        /// - Records added and removed items for bridging tables (List<int>) with '+' and '-' prefixes.
        /// 
        /// Notes:
        /// - This method resides in the MVC project because it depends on UI-specific attributes and services.
        /// - To support future additional fields or bridging tables, add new resolvers in valueResolvers or collectionResolvers.
        /// - The method safely handles null collections and skips ignored properties like "DateSubmitted".
        /// </summary>
        public async Task<List<MainFormAuditLogEntityModel>> GenerateAuditLogEntries(MainFormEntityModel original,
                                                                                     MainFormEntityModel updated,
                                                                                     string changedBy)
        {
            var auditEntries = new List<MainFormAuditLogEntityModel>();

            // Get all public instance properties of the entity model
            var props = typeof(MainFormAdminEntityModel).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Get properties of the ViewModel for Display attribute lookup
            var viewModelProps = typeof(MainFormAdminViewModel).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Map property names to friendly display names using [Display(Name = "...")] attribute on ViewModel
            var displayNameMap = viewModelProps
                .Select(p => new
                {
                    Name = p.Name,
                    DisplayName = p.GetCustomAttribute<DisplayAttribute>()?.Name ?? p.Name
                })
                .ToDictionary(x => x.Name, x => x.DisplayName);


            // Manually alias entity property names to corresponding view model display names
            displayNameMap["SampleRadioId"] = displayNameMap.GetValueOrDefault("SelectedSampleRadioId", "SampleRadio");
            displayNameMap["SampleRadioAdminId"] = displayNameMap.GetValueOrDefault("SelectedSampleRadioAdminId", "SampleRadioAdmin");


            // Properties to ignore during audit (e.g., timestamps)
            var ignoredProperties = new[] { "DateSubmitted" };

            // Resolvers to convert scalar property raw values into friendly strings (async for DB/service lookups)
            var valueResolvers = new Dictionary<string, Func<object?, Task<string>>>
            {
                ["StatusId"] = async val =>
                {
                    if (val == null) return "";
                    var id = int.TryParse(val.ToString(), out var parsedId) ? parsedId : 0;
                    return await _statusService.GetStatusTitleById(id) ?? $"(Unknown ID: {val})";
                }


            };

            // Resolvers for bridging table collections (List<int>) that resolve IDs to friendly names
            var collectionResolvers = new Dictionary<string, Func<int, Task<string>>>
            {


            };

            foreach (var prop in props)
            {
                if (ignoredProperties.Contains(prop.Name) || !prop.CanRead)
                    continue;

                var propType = prop.PropertyType;

                // Handle scalar properties (non-collections or strings)
                if (!typeof(System.Collections.IEnumerable).IsAssignableFrom(propType) || propType == typeof(string))
                {
                    var oldValRaw = prop.GetValue(original);
                    var newValRaw = prop.GetValue(updated);

                    // Formats raw property values for audit display (used when no custom valueResolver is provided)
                    string FormatRawValue(object? val, Type propertyType)
                    {
                        // Type-aware null handling
                        if (val == null)
                        {
                            if (propertyType == typeof(decimal) || propertyType == typeof(decimal?))
                                return "£0.00";

                            return "-"; // Use dash for string/null/non-decimal types
                        }

                        return val switch
                        {
                            decimal d => $"£{d:0.00}",
                            DateTime dt => dt.TimeOfDay.TotalSeconds == 0
                                ? dt.ToString("yyyy-MM-dd") // Full datetime if it's likely a date
                                : dt.ToString("HH:mm"), // Assume pure time entry
                            TimeSpan ts => ts.ToString(@"hh\:mm"), // In case you're using TimeSpan for time-only fields
                            string s => s,
                            _ => val.ToString() ?? "-"
                        };


                    }




                    var propertyType = prop.PropertyType;

                    var oldVal = valueResolvers.ContainsKey(prop.Name)
                        ? await valueResolvers[prop.Name](oldValRaw)
                        : FormatRawValue(oldValRaw, propertyType);

                    var newVal = valueResolvers.ContainsKey(prop.Name)
                        ? await valueResolvers[prop.Name](newValRaw)
                        : FormatRawValue(newValRaw, propertyType);




                    if (oldVal != newVal)
                    {
                        auditEntries.Add(new MainFormAuditLogEntityModel
                        {
                            MainFormId = original.Id,
                            PropertyName = prop.Name,
                            DisplayName = displayNameMap.GetValueOrDefault(prop.Name, prop.Name),
                            OldValue = oldVal,
                            NewValue = newVal,
                            ChangedBy = changedBy,
                            ChangeDate = DateTime.UtcNow
                        });
                    }
                }
                // Handle bridging table collections like List<int>
                else if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(List<>)
                    && propType.GetGenericArguments()[0] == typeof(int)
                    && collectionResolvers.ContainsKey(prop.Name))
                {
                    var originalIds = (prop.GetValue(original) as List<int>) ?? new();
                    var updatedIds = (prop.GetValue(updated) as List<int>) ?? new();

                    bool areEqual = originalIds.Count == updatedIds.Count && !originalIds.Except(updatedIds).Any();

                    if (!areEqual)
                    {
                        // Resolve all names in old and new lists
                        var oldNamesTasks = originalIds.Select(id => collectionResolvers[prop.Name](id));
                        var newNamesTasks = updatedIds.Select(id => collectionResolvers[prop.Name](id));

                        var oldNames = (await Task.WhenAll(oldNamesTasks)).ToList();
                        var newNames = (await Task.WhenAll(newNamesTasks)).ToList();

                        var oldVal = string.Join(", ", oldNames);
                        var newVal = string.Join(", ", newNames);

                        auditEntries.Add(new MainFormAuditLogEntityModel
                        {
                            MainFormId = original.Id,
                            PropertyName = prop.Name,
                            DisplayName = displayNameMap.GetValueOrDefault(prop.Name, prop.Name),
                            OldValue = oldVal,
                            NewValue = newVal,
                            ChangedBy = changedBy,
                            ChangeDate = DateTime.UtcNow
                        });
                    }


                }
            }

            return auditEntries;
        }



    }
}
